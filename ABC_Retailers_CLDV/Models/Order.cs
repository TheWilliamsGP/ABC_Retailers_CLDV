using Azure;
using Azure.Data.Tables;

namespace ABC_Retailers_CLDV.Models
{
    public class Order : ITableEntity
    {
        public string PartitionKey { get; set; } = "Orders";
        public string RowKey { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double TotalAmount { get; set; }
        public string Status { get; set; } // Placed / Delivered
        public DateTime OrderDateTime { get; set; }

        // Required for Table Storage
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }

    }
}
