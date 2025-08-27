using Microsoft.AspNetCore.Mvc.Rendering;

namespace ABCRetail.ViewModels
{
    public class OrderCreateViewModel
    {
        public string CustomerId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";

        public List<SelectListItem> Customers { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Products { get; set; } = new List<SelectListItem>();
    }
}