using Azure;
using Azure.Data.Tables;

namespace ABCRetail.Models
{
    public class Order : ITableEntity
    {
        public string OrderId { get; set; } = Guid.NewGuid().ToString();
        public string CustomerId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Pending";

        // ITableEntity implementation - FIXED
        public string PartitionKey { get; set; } = "Order";
        public string RowKey { get => OrderId; set => OrderId = value; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; } = ETag.All;

        // Constructor for easy creation
        public Order() { }

        public Order(string customerId, string username, string productId, string productName,
                    int quantity, decimal unitPrice, string status = "Pending")
        {
            CustomerId = customerId;
            Username = username;
            ProductId = productId;
            ProductName = productName;
            Quantity = quantity;
            UnitPrice = unitPrice;
            TotalPrice = quantity * unitPrice;
            Status = status;
            OrderDate = DateTime.UtcNow;
            PartitionKey = "Order";
            RowKey = OrderId;
        }
    }
}