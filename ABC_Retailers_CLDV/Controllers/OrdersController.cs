using ABC_Retailers_CLDV.Models;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;

namespace ABC_Retailers_CLDV.Controllers
{
    public class OrdersController : Controller
    {
        private readonly TableClient _tableClient;
        private readonly TableClient _productsTable;

        public OrdersController(TableService tableService)
        {
            _tableClient = tableService.GetTable("Orders");
            _productsTable = tableService.GetTable("Products"); // for price lookup
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            // Load all products for dropdown
            var products = _productsTable.Query<Product>().ToList();
            ViewBag.Products = products;
            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        public IActionResult Create(string customerName, string productName, int quantity)
        {
            // Get product price
            var product = _productsTable.Query<Product>(p => p.Name == productName).FirstOrDefault();
            if (product == null)
            {
                TempData["Error"] = "Product not found!";
                return RedirectToAction("Create");
            }

            double totalAmount = product.Price * quantity;

            var order = new Order
            {
                RowKey = Guid.NewGuid().ToString(),
                CustomerName = customerName,
                ProductName = productName,
                Quantity = quantity,
                TotalAmount = totalAmount,
                Status = "Placed",
                OrderDateTime = DateTime.UtcNow
            };

            _tableClient.AddEntity(order);

            return RedirectToAction("Index");
        }

        // GET: Orders/Index
        public IActionResult Index()
        {
            var orders = _tableClient.Query<Order>().OrderByDescending(o => o.OrderDateTime).ToList();
            return View(orders);
        }

        // Optional: Update status to Delivered
        public IActionResult Deliver(string id)
        {
            var order = _tableClient.GetEntity<Order>("Order", id).Value;
            order.Status = "Delivered";
            _tableClient.UpdateEntity(order, Azure.ETag.All, TableUpdateMode.Replace);
            return RedirectToAction("Index");
        }
    }
}

