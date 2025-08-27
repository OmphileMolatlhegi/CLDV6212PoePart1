using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ABCRetail.Services;
using ABCRetail.Models;
using ABCRetail.ViewModels;

namespace ABCRetail.Controllers
{
    public class OrderController : Controller
    {
        private readonly IAzureStorageService _storageService;

        public OrderController(IAzureStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var orders = await _storageService.GetEntitiesAsync<Order>();
                return View(orders ?? new List<Order>());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading orders: {ex.Message}");
                TempData["ErrorMessage"] = "Error loading orders. Please check your Azure Storage connection.";
                return View(new List<Order>());
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                var customers = await _storageService.GetEntitiesAsync<Customer>();
                var products = await _storageService.GetEntitiesAsync<Product>();

                var viewModel = new OrderCreateViewModel
                {
                    Customers = customers?.Select(c => new SelectListItem
                    {
                        Value = c.CustomerId,
                        Text = $"{c.Name} {c.Surname}"
                    }).ToList() ?? new List<SelectListItem>(),

                    Products = products?.Select(p => new SelectListItem
                    {
                        Value = p.ProductId,
                        Text = p.ProductName
                    }).ToList() ?? new List<SelectListItem>()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading create form: {ex.Message}");
                TempData["ErrorMessage"] = "Error loading order creation form.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderCreateViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return await PopulateViewModelAndReturn(viewModel);
                }

                var product = await _storageService.GetEntityAsync<Product>("Product", viewModel.ProductId);
                var customer = await _storageService.GetEntityAsync<Customer>("Customer", viewModel.CustomerId);

                if (product == null || customer == null)
                {
                    ModelState.AddModelError("", "Invalid product or customer selected.");
                    return await PopulateViewModelAndReturn(viewModel);
                }

                var order = new Order(
                    viewModel.CustomerId,
                    customer.Username,
                    viewModel.ProductId,
                    product.ProductName,
                    viewModel.Quantity,
                    product.Price,
                    viewModel.Status
                );

                await _storageService.AddEntityAsync(order);

                TempData["SuccessMessage"] = $"Order #{order.OrderId} created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating order: {ex.Message}");
                TempData["ErrorMessage"] = "Error creating order. Please try again.";
                return await PopulateViewModelAndReturn(viewModel);
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["ErrorMessage"] = "Order ID is required.";
                    return RedirectToAction(nameof(Index));
                }

                var order = await _storageService.GetEntityAsync<Order>("Order", id);
                if (order == null)
                {
                    TempData["ErrorMessage"] = $"Order #{id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(order);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading edit form: {ex.Message}");
                TempData["ErrorMessage"] = "Error loading order for editing.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Order order)
        {
            try
            {
                if (id != order.OrderId)
                {
                    TempData["ErrorMessage"] = "Order ID mismatch.";
                    return RedirectToAction(nameof(Index));
                }

                if (!ModelState.IsValid)
                {
                    return View(order);
                }

                await _storageService.UpdateEntityAsync(order);
                TempData["SuccessMessage"] = $"Order #{order.OrderId} updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating order: {ex.Message}");
                TempData["ErrorMessage"] = "Error updating order. Please try again.";
                return View(order);
            }
        }

        public async Task<IActionResult> Details(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["ErrorMessage"] = "Order ID is required.";
                    return RedirectToAction(nameof(Index));
                }

                Console.WriteLine($"Loading details for order: {id}");

                var order = await _storageService.GetEntityAsync<Order>("Order", id);
                if (order == null)
                {
                    Console.WriteLine($"Order not found: {id}");
                    TempData["ErrorMessage"] = $"Order #{id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                Console.WriteLine($"Order found: {order.OrderId}");

                // Get customer and product details
                Customer customer = null;
                Product product = null;

                try
                {
                    customer = await _storageService.GetEntityAsync<Customer>("Customer", order.CustomerId);
                    if (customer == null)
                    {
                        Console.WriteLine($"Customer not found: {order.CustomerId}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading customer: {ex.Message}");
                }

                try
                {
                    product = await _storageService.GetEntityAsync<Product>("Product", order.ProductId);
                    if (product == null)
                    {
                        Console.WriteLine($"Product not found: {order.ProductId}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading product: {ex.Message}");
                }

                ViewBag.Customer = customer;
                ViewBag.Product = product;

                return View(order);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading order details: {ex.Message}");
                TempData["ErrorMessage"] = "Error loading order details. Please check your Azure Storage connection.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["ErrorMessage"] = "Order ID is required.";
                    return RedirectToAction(nameof(Index));
                }

                await _storageService.DeleteEntityAsync<Order>("Order", id);
                TempData["SuccessMessage"] = "Order deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting order: {ex.Message}");
                TempData["ErrorMessage"] = "Error deleting order. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetProductPrice(string productId)
        {
            try
            {
                if (string.IsNullOrEmpty(productId))
                {
                    return Json(new { success = false, message = "Product ID is required" });
                }

                var product = await _storageService.GetEntityAsync<Product>("Product", productId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found" });
                }

                return Json(new
                {
                    success = true,
                    price = product.Price,
                    stock = product.StockAvailable
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting product price: {ex.Message}");
                return Json(new { success = false, message = "Error retrieving product information" });
            }
        }

        private async Task<IActionResult> PopulateViewModelAndReturn(OrderCreateViewModel viewModel)
        {
            try
            {
                var customers = await _storageService.GetEntitiesAsync<Customer>();
                var products = await _storageService.GetEntitiesAsync<Product>();

                viewModel.Customers = customers?.Select(c => new SelectListItem
                {
                    Value = c.CustomerId,
                    Text = $"{c.Name} {c.Surname}"
                }).ToList() ?? new List<SelectListItem>();

                viewModel.Products = products?.Select(p => new SelectListItem
                {
                    Value = p.ProductId,
                    Text = p.ProductName
                }).ToList() ?? new List<SelectListItem>();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error populating view model: {ex.Message}");
                TempData["ErrorMessage"] = "Error loading form data.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}