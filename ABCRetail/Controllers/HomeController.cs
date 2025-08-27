using Microsoft.AspNetCore.Mvc;
using ABCRetail.Services;
using ABCRetail.ViewModels;
using ABCRetail.Models;

namespace ABCRetail.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAzureStorageService _storageService;

        public HomeController(IAzureStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _storageService.GetEntitiesAsync<Customer>();
            var products = await _storageService.GetEntitiesAsync<Product>();
            var orders = await _storageService.GetEntitiesAsync<Order>();

            var featuredProducts = products.Take(5).ToList();

            var viewModel = new HomeViewModel
            {
                FeaturedProducts = featuredProducts,
                CustomerCount = customers.Count,
                ProductCount = products.Count,
                OrderCount = orders.Count
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}