using Azure;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Azure.Storage.Files.Shares;
using Azure.Storage.Sas;
using ABCRetail.Models;
using System.Text;

namespace ABCRetail.Services
{
    public class AzureStorageService : IAzureStorageService
    {
        private readonly string _connectionString;
     
        private readonly TableServiceClient _tableServiceClient;

        public AzureStorageService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AzureStorage");
            _tableServiceClient = new TableServiceClient(_connectionString);
            InitializeAsync().GetAwaiter().GetResult();
        }

        private async Task InitializeAsync()
        {
            try
            {
                // Initialize tables
                var tableServiceClient = new TableServiceClient(_connectionString);
            await tableServiceClient.CreateTableIfNotExistsAsync("Customers");
            await tableServiceClient.CreateTableIfNotExistsAsync("Products");
            await tableServiceClient.CreateTableIfNotExistsAsync("Orders");

            // Initialize blob containers
            var blobServiceClient = new BlobServiceClient(_connectionString);
            await blobServiceClient.GetBlobContainerClient("product-images").CreateIfNotExistsAsync();
            await blobServiceClient.GetBlobContainerClient("payment-proofs").CreateIfNotExistsAsync();

            // Initialize queues
            var queueServiceClient = new QueueServiceClient(_connectionString);
            await queueServiceClient.GetQueueClient("order-queue").CreateIfNotExistsAsync();
            await queueServiceClient.GetQueueClient("notification-queue").CreateIfNotExistsAsync();

            // Initialize file shares
            var shareServiceClient = new ShareServiceClient(_connectionString);
            await shareServiceClient.GetShareClient("reports").CreateIfNotExistsAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Initialization error: {ex.Message}");
            }
        }

        #region Table Operations

        public async Task<T> GetEntityAsync<T>(string partitionKey, string rowKey) where T : class, ITableEntity, new()
        {
            try
            {
                var tableName = GetTableName<T>();
                var tableClient = _tableServiceClient.GetTableClient(tableName);

                var response = await tableClient.GetEntityAsync<T>(partitionKey, rowKey);
                return response.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                Console.WriteLine($"Entity not found: {ex.Message}");
                return null;
            }
        }

        public async Task<List<T>> GetEntitiesAsync<T>(string partitionKey = null) where T : class, ITableEntity, new()
        {
            try
            {
                var tableName = GetTableName<T>();
                var tableClient = _tableServiceClient.GetTableClient(tableName);

                var entities = new List<T>();

                if (partitionKey != null)
                {
                    await foreach (var entity in tableClient.QueryAsync<T>(e => e.PartitionKey == partitionKey))
                    {
                        entities.Add(entity);
                    }
                }
                else
                {
                    await foreach (var entity in tableClient.QueryAsync<T>())
                    {
                        entities.Add(entity);
                    }
                }

                return entities;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting entities: {ex.Message}");
                return new List<T>();
            }
        }

        public async Task<T> AddEntityAsync<T>(T entity) where T : class, ITableEntity
        {
            try
            {
                var tableName = GetTableName<T>();
                var tableClient = _tableServiceClient.GetTableClient(tableName);

                await tableClient.AddEntityAsync(entity);
                return entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding entity: {ex.Message}");
                throw;
            }
        }

        public async Task<T> UpdateEntityAsync<T>(T entity) where T : class, ITableEntity
        {
            try
            {
                var tableName = GetTableName<T>();
                var tableClient = _tableServiceClient.GetTableClient(tableName);

                await tableClient.UpdateEntityAsync(entity, entity.ETag);
                return entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating entity: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteEntityAsync<T>(string partitionKey, string rowKey) where T : class, ITableEntity, new()
        {
            try
            {
                var tableName = GetTableName<T>();
                var tableClient = _tableServiceClient.GetTableClient(tableName);

                await tableClient.DeleteEntityAsync(partitionKey, rowKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting entity: {ex.Message}");
            }
        }

        private string GetTableName<T>()
        {
            var typeName = typeof(T).Name;
            return typeName + "s"; // Customers, Products, Orders
        }

        #endregion

        #region Blob Operations

        public async Task<string> UploadFileAsync(IFormFile file, string containerName)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Ensure container exists
            await blobContainerClient.CreateIfNotExistsAsync();

            var blobClient = blobContainerClient.GetBlobClient(Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));

            await using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, true);

            return blobClient.Uri.ToString();
        }

        public async Task<bool> DeleteFileAsync(string fileName, string containerName)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(fileName);

            return await blobClient.DeleteIfExistsAsync();
        }

        #endregion

        #region Queue Operations

        public async Task SendMessageAsync(string queueName, string message)
        {
            var queueClient = new QueueClient(_connectionString, queueName);

            // Ensure queue exists
            await queueClient.CreateIfNotExistsAsync();

            await queueClient.SendMessageAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes(message)));
        }

        public async Task<string> ReceiveMessageAsync(string queueName)
        {
            var queueClient = new QueueClient(_connectionString, queueName);

            // Ensure queue exists
            await queueClient.CreateIfNotExistsAsync();

            var response = await queueClient.ReceiveMessageAsync();

            if (response.Value == null) return null;

            var message = Encoding.UTF8.GetString(Convert.FromBase64String(response.Value.MessageText));
            await queueClient.DeleteMessageAsync(response.Value.MessageId, response.Value.PopReceipt);

            return message;
        }

        public async Task<int> GetQueueLengthAsync(string queueName)
        {
            var queueClient = new QueueClient(_connectionString, queueName);

            // Ensure queue exists
            await queueClient.CreateIfNotExistsAsync();

            var properties = await queueClient.GetPropertiesAsync();
            return properties.Value.ApproximateMessagesCount;
        }

        #endregion

        #region File Share Operations

        public async Task UploadFileToShareAsync(IFormFile file, string shareName, string directoryPath = "")
        {
            var shareClient = new ShareClient(_connectionString, shareName);

            // Ensure share exists
            await shareClient.CreateIfNotExistsAsync();

            var directoryClient = shareClient.GetDirectoryClient(directoryPath);

            // Ensure directory exists
            await directoryClient.CreateIfNotExistsAsync();

            var fileClient = directoryClient.GetFileClient(file.FileName);

            await using var stream = file.OpenReadStream();
            await fileClient.CreateAsync(stream.Length);
            await fileClient.UploadRangeAsync(new HttpRange(0, stream.Length), stream);
        }

        public async Task<Stream> DownloadFileFromShareAsync(string fileName, string shareName, string directoryPath = "")
        {
            var shareClient = new ShareClient(_connectionString, shareName);
            var directoryClient = shareClient.GetDirectoryClient(directoryPath);
            var fileClient = directoryClient.GetFileClient(fileName);

            var response = await fileClient.DownloadAsync();
            return response.Value.Content;
        }

        #endregion
    }
}