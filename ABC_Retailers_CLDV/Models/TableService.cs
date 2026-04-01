using Azure.Data.Tables;

namespace ABC_Retailers_CLDV.Models
{
    public class TableService
    {
        private readonly TableServiceClient _serviceClient;

        public TableService(IConfiguration config)
        {
            _serviceClient = new TableServiceClient(config["AzureStorage:ConnectionString"]);
        }

        public TableClient GetTable(string tableName)
        {
            var table = _serviceClient.GetTableClient(tableName);
            table.CreateIfNotExists();
            return table;
        }
    }
}
