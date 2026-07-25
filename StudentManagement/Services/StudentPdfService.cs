using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using StudentManagement.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace StudentManagement.Services
{
    public class StudentPdfService
    {

        private readonly IWebHostEnvironment _webHostEnvironment;
        public StudentPdfService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public byte[] GeneratePdf(Student student)
        {
            int age = DateTime.Today.Year - student.DateOfBirth.Year;

            if (student.DateOfBirth.Date > DateTime.Today.AddYears(-age))
                age--;

            byte[]? imageBytes = null;

            if (!string.IsNullOrEmpty(student.ProfileImage))
            {
                var imagePath = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "images",
                    "students",
                    student.ProfileImage);

                if (File.Exists(imagePath))
                {
                    imageBytes = File.ReadAllBytes(imagePath);
                }
            }
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header()
                        .Column(column =>
                        {
                            column.Item().AlignCenter().Text("GGPS Lodi Jajja")
                                .FontSize(22)
                                .Bold();

                            column.Item().AlignCenter().Text("Tehsil Pasrur, District Sialkot");

                            column.Item().AlignCenter().Text("EMIS Code: 34320564");

                            column.Item().PaddingTop(10);

                            column.Item().LineHorizontal(1);

                            column.Item().PaddingTop(10);

                            column.Item().AlignCenter().Text("STUDENT PROFILE")
                                .Bold()
                                .FontSize(18);
                            column.Item()
                                .AlignCenter()
                                .PaddingTop(5)
                                .Text($"Student ID: {student.Id}")
                                .FontSize(11)
                                .SemiBold();
                        });

                    page.Content()
    .PaddingVertical(20)
    .Column(column =>
    {
        // Student Photo
        if (imageBytes != null)
        {
            column.Item()
                .AlignCenter()
                .Width(150)
                .Height(150)
                .Image(imageBytes);
        }

        column.Item().PaddingVertical(15);

        // Student Information Table
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(140);
                columns.RelativeColumn();
            });

            void Row(string title, string value)
            {
                table.Cell()
                     .Border(1)
                     .BorderColor(Colors.Grey.Lighten2)
                     .Background(Colors.Grey.Lighten4)
                     .Padding(8)
                     .Text(title)
                     .Bold();

                table.Cell()
                     .Border(1)
                     .BorderColor(Colors.Grey.Lighten2)
                     .Padding(8)
                     .Text(value);
            }

            Row("Name", student.Name);
            Row("Father Name", student.FatherName);
            Row("Gender", student.Gender);
            Row("Date Of Birth", student.DateOfBirth.ToString("dd MMM yyyy"));
            Row("Age", age + " Years");
            Row("Department", student.Department.Name);
            Row("Blood Group", student.BloodGroup);
            Row("Email", student.Email);
            Row("Phone", student.PhoneNumber);
            Row("CNIC", student.CNIC);
            Row("Nationality", student.Nationality);
            Row("Address", student.Address);
        });
    });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Generated on ");
                            x.Span(DateTime.Now.ToString("dd MMM yyyy HH:mm"));
                        });
                });
            });

            return document.GeneratePdf();
        }
    }
}