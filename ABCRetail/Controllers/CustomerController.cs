using Microsoft.AspNetCore.Mvc;
using ABCRetail.Services;
using ABCRetail.Models;

namespace ABCRetail.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IAzureStorageService _storageService;

        public CustomerController(IAzureStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _storageService.GetEntitiesAsync<Customer>();
            return View(customers);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                await _storageService.AddEntityAsync(customer);
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var customer = await _storageService.GetEntityAsync<Customer>("Customer", id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _storageService.UpdateEntityAsync(customer);
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            await _storageService.DeleteEntityAsync<Customer>("Customer", id);
            return RedirectToAction(nameof(Index));
        }
    }
}