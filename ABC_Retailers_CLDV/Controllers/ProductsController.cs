using ABC_Retailers_CLDV.Models;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;

namespace ABC_Retailers_CLDV.Controllers
{
    public class ProductsController : Controller
    {
        private readonly TableClient _tableClient;
        private readonly BlobService _blobService;

        public ProductsController(TableService tableService, BlobService blobService)
        {
            _tableClient = tableService.GetTable("Products");
            _blobService = blobService;
        }

        public IActionResult Index()
        {
            var products = _tableClient.Query<Product>().ToList();
            return View(products);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(string name, string description, string category, double price, int stock, bool isAvailable, IFormFile image)
        {
            string imageUrl = null;

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

            _tableClient.AddEntity(product);

            return RedirectToAction("Index");
        }
    }
}
