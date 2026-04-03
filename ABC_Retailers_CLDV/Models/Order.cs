using Azure;
using Azure.Data.Tables;

namespace ABC_Retailers_CLDV.Models
{
    public class Order : ITableEntity
    {
        public string PartitionKey { get; set; } = "Order"; // All orders
        public string RowKey { get; set; } // Unique order ID

        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double TotalAmount { get; set; }

        public string Status { get; set; } = "Placed"; // Placed or Delivered
        public DateTime OrderDateTime { get; set; } = DateTime.Now;

        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }

    }
}
