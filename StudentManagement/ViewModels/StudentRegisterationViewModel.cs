using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace StudentManagement.ViewModels
{
    public class StudentRegistrationViewModel
    {
        public string Name { get; set; }

        public string FatherName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string BloodGroup { get; set; }

        public string Nationality { get; set; }

        public string CNIC { get; set; }

        public int DepartmentId { get; set; }

        public IFormFile ProfileImage { get; set; }
    }
}