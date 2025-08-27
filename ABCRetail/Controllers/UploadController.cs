using Microsoft.AspNetCore.Mvc;
using ABCRetail.Services;
using ABCRetail.Models;

namespace ABCRetail.Controllers
{
    public class UploadController : Controller
    {
        private readonly IAzureStorageService _storageService;

        public UploadController(IAzureStorageService storageService)
        {
            _storageService = storageService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(FileUploadModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                if (model.ProofOfPayment == null || model.ProofOfPayment.Length == 0)
                {
                    ModelState.AddModelError("ProofOfPayment", "Please select a file to upload.");
                    return View(model);
                }

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
                var extension = Path.GetExtension(model.ProofOfPayment.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("ProofOfPayment", "Only JPG, PNG, and PDF files are allowed.");
                    return View(model);
                }

                // Validate file size (10MB max)
                const int maxFileSize = 10 * 1024 * 1024; // 10MB
                if (model.ProofOfPayment.Length > maxFileSize)
                {
                    ModelState.AddModelError("ProofOfPayment", "File size must be less than 10MB.");
                    return View(model);
                }

                // Upload to blob storage
                var fileUrl = await _storageService.UploadFileAsync(model.ProofOfPayment, "payment-proofs");

                TempData["SuccessMessage"] = $"File uploaded successfully! File URL: {fileUrl}";

                // Clear the form
                ModelState.Clear();
                return View(new FileUploadModel());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file: {ex.Message}");
                TempData["ErrorMessage"] = "Error uploading file. Please try again.";
                return View(model);
            }
        }
    }
}