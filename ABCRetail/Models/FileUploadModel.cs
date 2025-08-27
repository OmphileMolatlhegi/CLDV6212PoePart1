using System.ComponentModel.DataAnnotations;

namespace ABCRetail.Models
{
    public class FileUploadModel
    {
        [Required(ErrorMessage = "Please select a file")]
        [Display(Name = "Proof of Payment")]
        public IFormFile ProofOfPayment { get; set; }

        [Display(Name = "Order ID")]
        [MaxLength(50, ErrorMessage = "Order ID cannot exceed 50 characters")]
        public string OrderId { get; set; } = string.Empty;

        [Display(Name = "Customer Name")]
        [MaxLength(100, ErrorMessage = "Customer name cannot exceed 100 characters")]
        public string CustomerName { get; set; } = string.Empty;
    }
}