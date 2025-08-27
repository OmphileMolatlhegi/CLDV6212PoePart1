using ABCRetail.Models;
using Azure.Data.Tables;

namespace ABCRetail.Services
{
    public interface IAzureStorageService
    {
        // Table operations
        Task<T> GetEntityAsync<T>(string partitionKey, string rowKey) where T : class, ITableEntity, new();
        Task<List<T>> GetEntitiesAsync<T>(string partitionKey = null) where T : class, ITableEntity, new();
        Task<T> AddEntityAsync<T>(T entity) where T : class, ITableEntity;
        Task<T> UpdateEntityAsync<T>(T entity) where T : class, ITableEntity;
        Task DeleteEntityAsync<T>(string partitionKey, string rowKey) where T : class, ITableEntity, new();

        // Blob operations
        Task<string> UploadFileAsync(IFormFile file, string containerName);
        Task<bool> DeleteFileAsync(string fileName, string containerName);

        // Queue operations
        Task SendMessageAsync(string queueName, string message);
        Task<string> ReceiveMessageAsync(string queueName);
        Task<int> GetQueueLengthAsync(string queueName);

        // File Share operations
        Task UploadFileToShareAsync(IFormFile file, string shareName, string directoryPath = "");
        Task<Stream> DownloadFileFromShareAsync(string fileName, string shareName, string directoryPath = "");
    }
}