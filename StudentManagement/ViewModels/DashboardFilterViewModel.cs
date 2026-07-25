using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudentManagement.ViewModels
{
    public class DashboardFilterViewModel
    {
        public string? Search { get; set; }

        public int? DepartmentId { get; set; }

        public string? Gender { get; set; }

        public string? BloodGroup { get; set; }

        public string? Nationality { get; set; }

        public List<SelectListItem>? Departments { get; set; }
    }
}
