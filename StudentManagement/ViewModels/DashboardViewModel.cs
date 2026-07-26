namespace StudentManagement.ViewModels
{
    public class DashboardViewModel
    {
        public string SchoolName { get; set; }

        public string Campus { get; set; }

        public string Location { get; set; }

        public int TotalStudents { get; set; }

        public int TotalDepartments { get; set; }

        public int MaleStudents { get; set; }

        public int FemaleStudents { get; set; }

        public DashboardFilterViewModel Filter { get; set; } = new();

        public List<string> DepartmentNames { get; set; } = new();

        public List<int> DepartmentStudentCounts { get; set; } = new();

        public int MaleCount { get; set; }

        public int FemaleCount { get; set; }
    }
}
