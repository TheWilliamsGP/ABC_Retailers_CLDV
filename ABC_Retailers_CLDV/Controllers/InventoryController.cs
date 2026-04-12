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
            _productsTable = tableService.GetTable("Products");
            _blobService = blobService;
        }

        //Displays a list of all products in inventory
        public IActionResult Index()
        {
            try
            {
                var products = _productsTable.Query<Product>().ToList();
                return View(products);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading inventory: " + ex.Message;
                return View(new List<Product>());
            }
        }

        //Displays the Edit form for a specific product
        public IActionResult Edit(string id)
        {
            try
            {
                var product = _productsTable.GetEntity<Product>("Product", id).Value;
                return View(product);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading product: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        //Handles form submission for updating product details
        [HttpPost]
        public async Task<IActionResult> Edit(Product product, IFormFile image)
        {
            try
            {
                product.PartitionKey = "Product";

                //Keep old image if no new upload
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
            catch (Exception ex)
            {
                TempData["Error"] = "Error updating product: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        //Deletes a product from inventory
        public IActionResult Delete(string id)
        {
            try
            {
                _productsTable.DeleteEntity("Product", id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting product: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
