using ABC_Retailers_CLDV.Models;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;

namespace ABC_Retailers_CLDV.Controllers
{
    public class ProductsController : Controller
    {
        // Azure Table Storage client for interacting with the Products table
        private readonly TableClient _tableClient;
        // Custom Blob service for handling image uploads
        private readonly BlobService _blobService;

        public ProductsController(TableService tableService, BlobService blobService)
        {

            _tableClient = tableService.GetTable("Products");
            _blobService = blobService;
        }

        // Displays a list of all products

        public IActionResult Index()
        {
            try
            {
            var products = _tableClient.Query<Product>().ToList();
            return View(products);

            }
            catch (Exception ex)
            {
                
                ViewBag.Error = "Error loading products: " + ex.Message;
                return View(new List<Product>());
            }
        }

        // Displays the Create Product form
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(string name, string description, string category, double price, int stock, bool isAvailable, IFormFile image)
        {
            try
            {
                string imageUrl = null;
            // If an image is uploaded, store it in Blob Storage
            if (image != null)
                imageUrl = await _blobService.UploadFileAsync(image, "product-images");

            var product = new Product
            {
                RowKey = Guid.NewGuid().ToString(),
                Name = name,
                Description = description,
                Category = category,
                Price = price,
                Stock = stock,
                IsAvailable = isAvailable,
                ImageUrl = imageUrl
            };
            // Add the product to Azure Table Storage
            _tableClient.AddEntity(product);

            return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error creating product: " + ex.Message;
                return View();
            }
        }  
    }
}
