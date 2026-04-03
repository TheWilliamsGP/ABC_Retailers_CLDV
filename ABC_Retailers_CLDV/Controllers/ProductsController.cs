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
        public IActionResult Edit(string id)
        {
            var product = _tableClient.GetEntity<Product>("Product", id).Value;
            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Product product, IFormFile image)
        {
            product.PartitionKey = "Product";

            // 1️⃣ Get the existing entity from Table
            var existing = _tableClient.GetEntity<Product>("Product", product.RowKey).Value;

            // 2️⃣ Preserve ImageUrl if no new image uploaded
            if (image != null)
            {
                // Upload new image to Blob
                var imageUrl = await _blobService.UploadFileAsync(image, "product-images");
                product.ImageUrl = imageUrl;
            }
            else
            {
                // Keep the old image
                product.ImageUrl = existing.ImageUrl;
            }

            // 3️⃣ Update Table
            _tableClient.UpdateEntity(product, Azure.ETag.All, TableUpdateMode.Replace);

            return RedirectToAction("Index");
        }

        public IActionResult Delete(string id)
        {
            _tableClient.DeleteEntity("Product", id);
            return RedirectToAction("Index");
        }
    }
}
