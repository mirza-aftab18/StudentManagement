using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using StudentManagement.Data;
using StudentManagement.Models;

namespace StudentManagement.Services
{
    public class StudentExcelService
    {
        public byte[] GenerateExcel(List<Student> students)
        {
            using MemoryStream stream = new MemoryStream();
            using (SpreadsheetDocument document =
            SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();

                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

                SheetData sheetData = new SheetData();

                worksheetPart.Worksheet = new Worksheet(sheetData);

                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

                Sheet sheet = new Sheet()
                {
                    Id = workbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "Students"
                };

                sheets.Append(sheet);
                Row headerRow = new Row();

                headerRow.Append(
                    CreateCell("ID"),
                    CreateCell("Name"),
                    CreateCell("Father Name"),
                    CreateCell("Gender"),
                    CreateCell("Department"),
                    CreateCell("Blood Group"),
                    CreateCell("Email"),
                    CreateCell("Phone")
                );

                sheetData.Append(headerRow);
                foreach (var student in students)
                {
                    Row row = new Row();

                    row.Append(
                        CreateCell(student.Id.ToString()),
                        CreateCell(student.Name),
                        CreateCell(student.FatherName),
                        CreateCell(student.Gender),
                        CreateCell(student.Department?.Name ?? ""),
                        CreateCell(student.BloodGroup),
                        CreateCell(student.Email),
                        CreateCell(student.PhoneNumber)
                    );

                    sheetData.Append(row);
                }
            }
            return stream.ToArray();

        }
        private Cell CreateCell(string text)
        {
            return new Cell
            {
                DataType = CellValues.String,
                CellValue = new CellValue(text)
            };
        }
    }
}