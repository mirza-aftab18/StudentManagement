using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Data;
using StudentManagement.Models;
using StudentManagement.Services;
using StudentManagement.ViewModels;
using System.Diagnostics;


namespace StudentManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly StudentPdfService _studentPdfService;
        private readonly StudentExcelService _studentExcelService;

        public HomeController(ApplicationDbContext context,
                      IWebHostEnvironment webHostEnvironment,
                      StudentPdfService studentPdfService,
                      StudentExcelService studentExcelService)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _studentPdfService = studentPdfService;
            _studentExcelService = studentExcelService;
        }
        public IActionResult Index()
        {
            DashboardViewModel dashboard = new DashboardViewModel();

            dashboard.SchoolName = "GGPS Lodi Jajja Tehsil Pasrur District Sialkot";
            dashboard.EMISCode = "34320564";

            dashboard.TotalStudents = _context.Students.Count();
            dashboard.TotalDepartments = _context.Departments.Count();
            dashboard.MaleStudents = _context.Students.Count(s => s.Gender == "Male");
            dashboard.FemaleStudents = _context.Students.Count(s => s.Gender == "Female");
            dashboard.MaleCount = dashboard.MaleStudents;
            dashboard.FemaleCount = dashboard.FemaleStudents;

            // Chart Data
            dashboard.DepartmentNames = _context.Departments
                .Select(d => d.Name)
                .ToList();

            dashboard.DepartmentStudentCounts = _context.Departments
                .Select(d => d.Student.Count)
                .ToList();

            // Dashboard Filters
            dashboard.Filter.Departments = _context.Departments
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                })
                .ToList();

            return View(dashboard);
        }


        [HttpGet]
        public IActionResult RegisterStudent()
        {
            if (TempData["CNIC"] == null || TempData["Nationality"] == null)
            {
                return RedirectToAction("VerifyStudent");
            }

            ViewBag.Departments = new SelectList(_context.Departments, "Id", "Name");
            LoadBloodGroups();

            StudentRegistrationViewModel model = new StudentRegistrationViewModel
            {
                CNIC = TempData["CNIC"].ToString(),
                Nationality = TempData["Nationality"].ToString()
            };

            return View(model);
        }
        [HttpPost]
        public IActionResult RegisterStudent(StudentRegistrationViewModel model)
        {
            if (_context.Students.Any(s => s.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email already exists.");
            }

            if (ModelState.IsValid)
            {
                string fileName = null;

                if (model.ProfileImage != null)
                {
                    string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "students");

                    fileName = Guid.NewGuid().ToString() +
                               Path.GetExtension(model.ProfileImage.FileName);

                    string filePath = Path.Combine(uploadFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.ProfileImage.CopyTo(fileStream);
                    }
                }

                Student student = new Student
                {
                    Name = model.Name,
                    FatherName = model.FatherName,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    BloodGroup = model.BloodGroup,
                    Nationality = model.Nationality,
                    CNIC = model.CNIC,
                    DepartmentId = model.DepartmentId,
                    ProfileImage = fileName
                };

                _context.Students.Add(student);
                _context.SaveChanges();

                return RedirectToAction("StudentsList");
            }

            ViewBag.Departments = new SelectList(_context.Departments, "Id", "Name", model.DepartmentId);
            LoadBloodGroups();

            return View(model);
        }
        private void LoadBloodGroups()
        {
            ViewBag.BloodGroups = new List<string>
            {
                "A+",
                "A-",
                "B+",
                "B-",
                "AB+",
                "AB-",
                "O+",
                "O-"
            };
        }
        private void LoadNationalities()
        {
            ViewBag.Nationalities = new List<string>
        {
            "Pakistan",
            "India",
            "Saudi Arabia"
        };
        }

        public IActionResult StudentsList(
            string search,
            int? departmentId,
            string gender,
            string bloodGroup,
            string nationality,
            string sortOrder,
            int page = 1)
        {
            int pageSize = 10;

            var students = _context.Students
                                   .Include(s => s.Department)
                                   .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                students = students.Where(s =>
                    s.Name.Contains(search) ||
                    s.FatherName.Contains(search) ||
                    s.Email.Contains(search) ||
                    s.PhoneNumber.Contains(search) ||
                    s.CNIC.Replace("-", "").Contains(search.Replace("-", "")) ||
                    s.Department.Name.Contains(search));
            }

            // Department Filter
            if (departmentId.HasValue)
            {
                students = students.Where(s => s.DepartmentId == departmentId.Value);
            }

            // Gender Filter
            if (!string.IsNullOrWhiteSpace(gender))
            {
                students = students.Where(s => s.Gender == gender);
            }

            // Blood Group Filter
            if (!string.IsNullOrWhiteSpace(bloodGroup))
            {
                students = students.Where(s => s.BloodGroup == bloodGroup);
            }

            // Nationality Filter
            if (!string.IsNullOrWhiteSpace(nationality))
            {
                students = students.Where(s => s.Nationality == nationality);
            }

            // Sorting Links
            ViewBag.NameSort = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.DepartmentSort = sortOrder == "department" ? "department_desc" : "department";
            ViewBag.DobSort = sortOrder == "dob" ? "dob_desc" : "dob";
            ViewBag.CurrentSort = sortOrder;

            // Sorting Arrows
            ViewBag.NameArrow = "";
            ViewBag.DepartmentArrow = "";
            ViewBag.DobArrow = "";

            switch (sortOrder)
            {
                case "name":
                    ViewBag.NameArrow = "▲";
                    students = students.OrderBy(s => s.Name);
                    break;

                case "name_desc":
                    ViewBag.NameArrow = "▼";
                    students = students.OrderByDescending(s => s.Name);
                    break;

                case "department":
                    ViewBag.DepartmentArrow = "▲";
                    students = students.OrderBy(s => s.Department.Name);
                    break;

                case "department_desc":
                    ViewBag.DepartmentArrow = "▼";
                    students = students.OrderByDescending(s => s.Department.Name);
                    break;

                case "dob":
                    ViewBag.DobArrow = "▲";
                    students = students.OrderBy(s => s.DateOfBirth);
                    break;

                case "dob_desc":
                    ViewBag.DobArrow = "▼";
                    students = students.OrderByDescending(s => s.DateOfBirth);
                    break;

                default:
                    students = students.OrderBy(s => s.Id);
                    break;
            }

            // Pagination
            int totalStudents = students.Count();

            students = students
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            ViewBag.Search = search;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalStudents / pageSize);

            ViewBag.DepartmentId = departmentId;
            ViewBag.Gender = gender;
            ViewBag.BloodGroup = bloodGroup;
            ViewBag.Nationality = nationality;

            return View(students.ToList());
        }

        public IActionResult StudentDetails(int id)
        {
            var student = _context.Students
                                  .Include(s => s.Department)
                                  .FirstOrDefault(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpGet]
        public IActionResult EditStudent(int id)
        {
            var student = _context.Students.Find(id);

            if (student == null)
            {
                return NotFound();
            }

            StudentRegistrationViewModel model = new StudentRegistrationViewModel
            {
                Name = student.Name,
                FatherName = student.FatherName,
                DateOfBirth = student.DateOfBirth,
                Gender = student.Gender,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber,
                Address = student.Address,
                BloodGroup = student.BloodGroup,
                Nationality = student.Nationality,
                CNIC = student.CNIC,
                DepartmentId = student.DepartmentId
            };

            ViewBag.StudentId = student.Id;
            ViewBag.ProfileImage = student.ProfileImage;

            ViewBag.Departments = new SelectList(
                _context.Departments,
                "Id",
                "Name",
                student.DepartmentId);

            LoadBloodGroups();

            return View(model);
        }

        [HttpPost]
        public IActionResult EditStudent(int id, StudentRegistrationViewModel model)
        {
            var student = _context.Students.Find(id);

            if (student == null)
            {
                return NotFound();
            }

            if (_context.Students.Any(s => s.Email == model.Email && s.Id != id))
            {
                ModelState.AddModelError("Email", "Email already exists.");
            }

            if (ModelState.IsValid)
            {
                // Upload New Image
                if (model.ProfileImage != null)
                {
                    string uploadFolder = Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        "images",
                        "students");

                    // Delete Old Image
                    if (!string.IsNullOrEmpty(student.ProfileImage))
                    {
                        string oldImagePath = Path.Combine(uploadFolder, student.ProfileImage);

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Save New Image
                    string fileName = Guid.NewGuid().ToString() +
                                      Path.GetExtension(model.ProfileImage.FileName);

                    string filePath = Path.Combine(uploadFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.ProfileImage.CopyTo(fileStream);
                    }

                    student.ProfileImage = fileName;
                }

                // Update Student Data
                student.Name = model.Name;
                student.FatherName = model.FatherName;
                student.DateOfBirth = model.DateOfBirth;
                student.Gender = model.Gender;
                student.Email = model.Email;
                student.PhoneNumber = model.PhoneNumber;
                student.Address = model.Address;
                student.BloodGroup = model.BloodGroup;
                student.Nationality = model.Nationality;
                student.CNIC = model.CNIC;
                student.DepartmentId = model.DepartmentId;

                _context.SaveChanges();

                return RedirectToAction("StudentsList");
            }

            ViewBag.ProfileImage = student.ProfileImage;

            ViewBag.Departments = new SelectList(
                _context.Departments,
                "Id",
                "Name",
                model.DepartmentId);

            LoadBloodGroups();

            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteStudent(int id)
        {
            var student = _context.Students.Find(id);

            if (student == null)
            {
                return RedirectToAction("StudentsList");
            }

            // Delete Profile Image
            if (!string.IsNullOrEmpty(student.ProfileImage))
            {
                string imagePath = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "images",
                    "students",
                    student.ProfileImage);

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Students.Remove(student);
            _context.SaveChanges();

            return RedirectToAction("StudentsList");
        }
        [HttpPost]
        public JsonResult CheckCNIC(string cnic)
        {
            bool exists = _context.Students.Any(s => s.CNIC == cnic);

            return Json(new
            {
                exists = exists
            });
        }
        [HttpGet]
        public IActionResult VerifyStudent()
        {
            LoadNationalities();
            return View();
        }

        [HttpPost]
        public IActionResult VerifyStudent(StudentVerificationViewModel model)
        {
            if (_context.Students.Any(s => s.CNIC == model.CNIC))
            {
                ModelState.AddModelError("CNIC", "CNIC already exists.");
            }

            if (ModelState.IsValid)
            {
                TempData["CNIC"] = model.CNIC;
                TempData["Nationality"] = model.Nationality;

                return RedirectToAction("RegisterStudent");
            }

            LoadNationalities();
            return View(model);
        }

        public IActionResult DownloadPdf(int id)
        {
            var student = _context.Students
                                  .Include(s => s.Department)
                                  .FirstOrDefault(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            var pdf = _studentPdfService.GeneratePdf(student);

            return File(pdf, "application/pdf", $"{student.Name}.pdf");
        }

        public IActionResult ExportToExcel()
        {
            var students = _context.Students
                .Include(s => s.Department)
                .ToList();

            byte[] file = _studentExcelService.GenerateExcel(students);

            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Students_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
