using Azure;
using Azure.Data.Tables;

public class Product : ITableEntity
{
    public string PartitionKey { get; set; } = "Product";
    public string RowKey { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }

    public string Category { get; set; }
    public double Price { get; set; }
    public int Stock { get; set; }
    public bool IsAvailable { get; set; }

    public string ImageUrl { get; set; }

    public ETag ETag { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
}