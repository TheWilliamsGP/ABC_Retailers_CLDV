using ABC_Retailers_CLDV.Models;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;

namespace ABC_Retailers_CLDV.Controllers
{
    public class InventoryController : Controller
    {
        private readonly TableClient _productsTable;
        private readonly BlobService _blobService;
        public InventoryController(TableService tableService, BlobService blobService)
        {
            // Reference your Products Table
            _productsTable = tableService.GetTable("Products");
            _blobService = blobService;
        }

        // GET: Inventory
        public IActionResult Index()
        {
            var products = _productsTable.Query<Product>().ToList();
            return View(products);
        }

        // GET: Inventory/Edit/{id}
        public IActionResult Edit(string id)
        {
            var product = _productsTable.GetEntity<Product>("Product", id).Value;
            return View(product);
        }

        // POST: Inventory/Edit
        [HttpPost]
        public async Task<IActionResult> Edit(Product product, IFormFile image)
        {
            product.PartitionKey = "Product";

            // Preserve old image if no new upload
            var existing = _productsTable.GetEntity<Product>("Product", product.RowKey).Value;

            if (image != null)
            {
                var imageUrl = await _blobService.UploadFileAsync(image, "product-images");
                product.ImageUrl = imageUrl;
            }
            else
            {
                product.ImageUrl = existing.ImageUrl;
            }

            _productsTable.UpdateEntity(product, Azure.ETag.All, TableUpdateMode.Replace);

            return RedirectToAction("Index");
        }

        // GET: Inventory/Delete/{id}
        public IActionResult Delete(string id)
        {
            _productsTable.DeleteEntity("Product", id);
            return RedirectToAction("Index");
        }
    }
}
