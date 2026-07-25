using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Student>? Student { get; set; }
    }
}
