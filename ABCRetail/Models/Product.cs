using Azure;
using Azure.Data.Tables;

namespace ABCRetail.Models
{
    public class Product : ITableEntity
    {
        public string ProductId { get; set; } = Guid.NewGuid().ToString();
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockAvailable { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        // ITableEntity implementation
        public string PartitionKey { get; set; } = "Product";
        public string RowKey { get => ProductId; set => ProductId = value; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; } = ETag.All;

        public Product() { }

        public Product(string productName, string description, decimal price, int stockAvailable, string imageUrl = "")
        {
            ProductName = productName;
            Description = description;
            Price = price;
            StockAvailable = stockAvailable;
            ImageUrl = imageUrl;
            PartitionKey = "Product";
            RowKey = ProductId;
        }
    }
}