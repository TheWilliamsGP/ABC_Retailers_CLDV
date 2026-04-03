using ABC_Retailers_CLDV.Models;
using Azure.Data.Tables;
using Azure.Storage.Queues.Models;
using Microsoft.AspNetCore.Mvc;

namespace ABC_Retailers_CLDV.Controllers
{
    public class OrdersController : Controller
    {
        private readonly TableClient _tableClient;
        private readonly TableClient _productsTable;
        private readonly QueueService _queueService;
        private readonly FileService _fileService;

        public OrdersController(TableService tableService, QueueService queueService, FileService fileService)
        {
            _tableClient = tableService.GetTable("Orders");
            _productsTable = tableService.GetTable("Products");
            _queueService = queueService;
            _fileService = fileService;
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            var products = _productsTable.Query<Product>().ToList();
            ViewBag.Products = products ?? new List<Product>(); // prevent null
            return View();
        }

        // POST: Orders/Create

        [HttpPost]
        public async Task<IActionResult> Create(string customerName, string productName, int quantity)
        {
            if (string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(productName))
            {
                TempData["Error"] = "Missing data";
                return RedirectToAction("Create");
            }

            var product = _productsTable.Query<Product>(p => p.Name == productName).FirstOrDefault();
            if (product == null)
            {
                TempData["Error"] = "Product not found!";
                return RedirectToAction("Create");
            }

            var orderId = Guid.NewGuid().ToString();
            double totalAmount = product.Price * quantity;

            var order = new Order
            {
                PartitionKey = "Orders",
                RowKey = orderId,
                CustomerName = customerName,
                ProductName = productName,
                Quantity = quantity,
                TotalAmount = totalAmount,
                Status = "Placed",
                OrderDateTime = DateTime.UtcNow
            };

            _tableClient.AddEntity(order);

            // Queue message
            string message = $"OrderId:{orderId}, Product:{productName}, Qty:{quantity}, Customer:{customerName}";
            _queueService.SendMessage("orderqueue", message);

            // ✅ Write log to File Share (async)
            string logContent = $"OrderId:{orderId}, Customer:{customerName}, Product:{productName}, Quantity:{quantity}, Total:{totalAmount}, Date:{DateTime.UtcNow}";
            await _fileService.UploadLogAsync("logs", $"order-{orderId}.txt", logContent);

            return RedirectToAction("Index");
        }

        // GET: Orders/Index
        public IActionResult Index()
        {
            var orders = _tableClient.Query<Order>()
                .OrderByDescending(o => o.OrderDateTime)
                .ToList();

            return View(orders);
        }
    }
}