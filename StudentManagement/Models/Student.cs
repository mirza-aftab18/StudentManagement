using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Models
{
    public class Student
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email {  get; set; }
        [Required]
        [RegularExpression(@"^\d{5}-\d{7}-\d{1}$",
        ErrorMessage = "CNIC format should be XXXXX-XXXXXXX-X")]
        public string CNIC { get; set; }
        public string Nationality { get; set; }
        public string FatherName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string BloodGroup { get; set; }

        public string ProfileImage { get; set; }
        [Required]
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
    }
}
