using ABC_Retailers_CLDV.Models;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;

namespace ABC_Retailers_CLDV.Controllers
{
    public class CustomersController : Controller
    {
        private readonly TableClient _tableClient;

        public CustomersController(TableService tableService)
        {
            _tableClient = tableService.GetTable("Customers");
        }

        public IActionResult Index()
        {
            var customers = _tableClient.Query<Customer>().ToList();
            return View(customers);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(string name, string email)
        {
            var customer = new Customer
            {
                RowKey = Guid.NewGuid().ToString(),
                Name = name,
                Email = email,
                AddedDate = DateTime.UtcNow
            };
            _tableClient.AddEntity(customer);
            return RedirectToAction("Index");
        }
    }
}