using Azure;
using Azure.Data.Tables;

namespace ABC_Retailers_CLDV.Models
{
    public class Customer : ITableEntity
    {
        public string PartitionKey { get; set; } = "Customer";
        public string RowKey { get; set; } // unique ID
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime AddedDate { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}
