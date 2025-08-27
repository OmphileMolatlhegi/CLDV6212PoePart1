using Microsoft.AspNetCore.Mvc;
using ABCRetail.Services;
using ABCRetail.Models;

namespace ABCRetail.Controllers
{
    public class ProductController : Controller
    {
        private readonly IAzureStorageService _storageService;

        public ProductController(IAzureStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _storageService.GetEntitiesAsync<Product>();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    product.ImageUrl = await _storageService.UploadFileAsync(imageFile, "product-images");
                }

                await _storageService.AddEntityAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["ErrorMessage"] = "Product ID is required.";
                    return RedirectToAction(nameof(Index));
                }

                Console.WriteLine($"Loading product for edit: {id}");

                var product = await _storageService.GetEntityAsync<Product>("Product", id);
                if (product == null)
                {
                    Console.WriteLine($"Product not found: {id}");
                    TempData["ErrorMessage"] = $"Product not found.";
                    return RedirectToAction(nameof(Index));
                }

                Console.WriteLine($"Product found: {product.ProductName}");
                return View(product);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading edit page: {ex.Message}");
                TempData["ErrorMessage"] = "Error loading product for editing.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Product product, IFormFile imageFile)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    product.ImageUrl = await _storageService.UploadFileAsync(imageFile, "product-images");
                }

                await _storageService.UpdateEntityAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]


        public async Task<IActionResult> Delete(string id)
        {
            await _storageService.DeleteEntityAsync<Product>("Product", id);
            return RedirectToAction(nameof(Index));
        }
    }
}