using System.ComponentModel.DataAnnotations;

namespace StudentManagement.ViewModels
{
    public class StudentVerificationViewModel
    {
        [Required(ErrorMessage = "Nationality is required.")]
        public string Nationality { get; set; }

        [Required(ErrorMessage = "CNIC is required.")]
        [RegularExpression(@"^\d{5}-\d{7}-\d{1}$",
            ErrorMessage = "CNIC format should be XXXXX-XXXXXXX-X")]
        public string CNIC { get; set; }
    }
}
