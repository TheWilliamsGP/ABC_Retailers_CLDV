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

        // Displays the Create Order form
        public IActionResult Create()
        {
            try
            {
                var products = _productsTable.Query<Product>().ToList();
                ViewBag.Products = products ?? new List<Product>();
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading products: " + ex.Message;
                return View(new List<Product>());
            }
        }

        // Handles form submission for creating a new order
        [HttpPost]
        public async Task<IActionResult> Create(string customerName, string productName, int quantity)
        {
            try
            {
                if (string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(productName))
                {
                    TempData["Error"] = "Missing data";
                    return RedirectToAction("Create");
                }
                // Retrieve the selected product from Products table
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

                // Add order to Azure Table Storage
                _tableClient.AddEntity(order);

                // Create a queue message for order processing
                string message = $"OrderId:{orderId}, Product:{productName}, Qty:{quantity}, Customer:{customerName}";
                _queueService.SendMessage("orderqueue", message);

                // Create log content for file storage
                string logContent = $"OrderId:{orderId}, Customer:{customerName}, Product:{productName}, Quantity:{quantity}, Total:{totalAmount}, Date:{DateTime.UtcNow}";
                await _fileService.UploadLogAsync("logs", $"order-{orderId}.txt", logContent);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error creating order: " + ex.Message;
                return RedirectToAction("Create");
            }
        }

        // Displays a list of all orders
        public IActionResult Index()
        {
            try
            {
                var orders = _tableClient.Query<Order>()
                .OrderByDescending(o => o.OrderDateTime)
                .ToList();

                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading orders: " + ex.Message;
                return View(new List<Order>());
            }
        }
    }
}