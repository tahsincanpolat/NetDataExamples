using EFCore.BulkExtensions;
using EntityFrameworkExample.Data;
using EntityFrameworkExample.Extensions;
using EntityFrameworkExample.Models;
using EntityFrameworkExample.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EntityFrameworkExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly SchoolContext _context;

        public HomeController(SchoolContext context)
        {
            _context = context;
        }
        // Anasayfa için tüm öðrencileri listeleyen aksiyon
        public IActionResult Index()
        {
            // Tablodaki tüm öðrencileri al
            var students = _context.Students.ToList();
            return View(students);
        }

        // Belirli bir öðrencinin detayýný gösteren aksiyon
        public IActionResult Details(int id)
        {
            // ID ye göre öðrenciyi bul
            var student = _context.Students.Find(id);

            if (student == null)
            {
                // Öðrenci bulunamazsa 404 sayfasýna yönlendir.
                return NotFound();
            }

            return View(student);
        }

        // Yeni öðrenci eklemek için view
        public IActionResult Create()
        {
            return View();
        }

        // Öðrenci oluþturmak için Post Aksiyonu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Name,Age,Department")] Student student)
        {
            if (ModelState.IsValid)
            {
                // Model geçerli ise yeni öðrenci ekle
                _context.Add(student);
                _context.SaveChanges();
                // baþarýyla eklendiyse anasayfaya yönlendir.
                return RedirectToAction("Index");
            }
            return View(student);
        }

        // Öðrenci düzenlemek için formu döndüren aksiyon
        public IActionResult Edit(int id)
        {
            // ID ye göre öðrenciye bul
            var student = _context.Students.Find(id);
            if (student == null)
            {
                // öðrenciyi bulamazsa 404 sayfasýna yönlendir.
                return NotFound();
            }

            return View(student);
        }

        // Öðrenci düzenlemek iþemini iþleyen POST aksiyonu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Age,Department")] Student student)
        {
            if (id != student.Id)
            {
                // öðrenciyi bulamazsa 404 sayfasýna yönlendir.
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Öðrenci bilgilerini güncelle
                    _context.Students.Update(student);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Eðer bir hata oluþursa öðrencinin olup olmadýðýný kontrol.
                    if (!StudentExists(student.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            // id ye göre öðrenciyi bul
            var student = _context.Students.Find(id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // id ye göre öðrenciyi bul
            var student = _context.Students.Find(id);
            if (student != null)
            {
                // Öðrenciyi veritabanýndaki tablodan sil
                _context.Students.Remove(student);
                _context.SaveChanges();
            }
            // silme iþlemi baþarýlý þekilde gerçekleþirse anasayafaya yönlendir.

            return RedirectToAction("Index");
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(s => s.Id == id);
        }

        // Yaþý 18'den büyük olan öðrencileri sorgulayan LINQ sorgusu (QuerySyntax (Sorgu söz dizimi))
        public IActionResult QuerySyntax()
        {
            var students = (from s in _context.Students
                            where s.Age > 18
                            select s).ToList();

            return View("Index", students);
        }

        // Yaþý 18'den küçük olan öðrencileri sorgulayan LINQ sorgusu (MethodSyntax (Method söz dizimi))
        public IActionResult MethodSyntax()
        {
            var students = _context.Students
                .Where(s => s.Age < 18)
                .ToList();

            return View("Index", students);
        }

        // Öðrenci tablosu ile Kurs tablosunu Student Id ye göre join eden aksiyon
        public IActionResult Join()
        {
            var stundentCourses = (from student in _context.Students
                                   join course in _context.Courses on student.Id equals course.StudentId
                                   select new
                                   {
                                       StudentName = student.Name,
                                       CourseTitle = course.Title
                                   }).ToList();

            return View(stundentCourses);
        }

        public IActionResult GroupByDepartment()
        {
            var groupedStudents = _context.Students
                .GroupBy(s => s.Department)
                .Select(g => new GroupedStudentViewModel
                {
                    Department = g.Key,
                    Students = g.ToList()
                })
                .ToList();

            return View(groupedStudents);
        }

        public IActionResult CustomExtensionMethod()
        {
            var students = _context.Students.ToList();
            // Custom oluþturduðumuz extension method yaþ aralýðýna göre gruplama yapar.
            var groupedStudentByAge = students.GroupByAgeRange();
            return View(groupedStudentByAge);
        }

        public IActionResult GetStudentByDepartment()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetStudentByDepartment(string department)
        {
            // Sql tarafýnda oluþturuduðumuz procedure ü tetikledik.
            var students = _context.Students
                .FromSqlInterpolated($"EXEC GetStudentByDepartment {department}")
                .ToList();
            ViewData["Students"] = students;

            return View();
        }

        [HttpPost("Transaction")]
        public IActionResult AddStudentsWithTransaction([FromBody] List<Student> students) //  [{"Name":"Ali","Age":20,"Department":"Bilgisayar"},{"Name":"Ayþe","Age":25,"Department":"Elektronik"}]
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.Students.AddRange(students);
                _context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                // Hata durumunu döndürebiliriz.
                return StatusCode(500, "Öðrenciler Eklenirken bir hata oluþtu.");

            }
            // Baþarýlý durumda 200 döndürecek.
            return Ok();
        }

        // Ham Sql (Raw sql) sorgu ile Öðrencileri Listeleme

        public IActionResult RawSql()
        {
            var students = _context.Students
                    .FromSqlRaw("SELECT * FROM Students WHERE Age > 18")
                    .ToList();

            return View("Index", students);
        }

        // EfCore BulkExtensions paketi kullanarak toplu öðrenci ekleme
        public IActionResult BulkInsert()
        {
            var students = new List<Student>()
            {
                new Student() { Name = "Ahmet", Age = 20, Department = "Endüstri" },
                new Student() { Name = "Elif", Age = 25, Department = "Mekatronik" }
            };

            _context.BulkInsert(students);

            return RedirectToAction("Index");
        }
    }
}
