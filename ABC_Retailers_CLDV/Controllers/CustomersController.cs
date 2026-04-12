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

        //Displays a list of all customers
        public IActionResult Index()
        {
            try
            {
                //Retrieve all customers from Azure Table Storage
                var customers = _tableClient.Query<Customer>().ToList();
                return View(customers);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading customers: " + ex.Message;
                return View(new List<Customer>());
            }
        }
        //Displays the Create Customer form
        public IActionResult Create() => View();

        //Handles form submission for creating a new customer
        [HttpPost]
        public IActionResult Create(string name, string email)
        {
            try
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
            catch (Exception ex)
            {
                TempData["Error"] = "Error creating customer: " + ex.Message;
                return View();
            }

        }
    }
}