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
        // Anasayfa i�in t�m ��rencileri listeleyen aksiyon
        public IActionResult Index()
        {
            // Tablodaki t�m ��rencileri al
            var students = _context.Students.ToList();
            return View(students);
        }

        // Belirli bir ��rencinin detay�n� g�steren aksiyon
        public IActionResult Details(int id)
        {
            // ID ye g�re ��renciyi bul
            var student = _context.Students.Find(id);

            if (student == null)
            {
                // ��renci bulunamazsa 404 sayfas�na y�nlendir.
                return NotFound();
            }

            return View(student);
        }

        // Yeni ��renci eklemek i�in view
        public IActionResult Create()
        {
            return View();
        }

        // ��renci olu�turmak i�in Post Aksiyonu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Name,Age,Department")] Student student)
        {
            if (ModelState.IsValid)
            {
                // Model ge�erli ise yeni ��renci ekle
                _context.Add(student);
                _context.SaveChanges();
                // ba�ar�yla eklendiyse anasayfaya y�nlendir.
                return RedirectToAction("Index");
            }
            return View(student);
        }

        // ��renci d�zenlemek i�in formu d�nd�ren aksiyon
        public IActionResult Edit(int id)
        {
            // ID ye g�re ��renciye bul
            var student = _context.Students.Find(id);
            if (student == null)
            {
                // ��renciyi bulamazsa 404 sayfas�na y�nlendir.
                return NotFound();
            }

            return View(student);
        }

        // ��renci d�zenlemek i�emini i�leyen POST aksiyonu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Age,Department")] Student student)
        {
            if (id != student.Id)
            {
                // ��renciyi bulamazsa 404 sayfas�na y�nlendir.
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // ��renci bilgilerini g�ncelle
                    _context.Students.Update(student);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // E�er bir hata olu�ursa ��rencinin olup olmad���n� kontrol.
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
            // id ye g�re ��renciyi bul
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
            // id ye g�re ��renciyi bul
            var student = _context.Students.Find(id);
            if (student != null)
            {
                // ��renciyi veritaban�ndaki tablodan sil
                _context.Students.Remove(student);
                _context.SaveChanges();
            }
            // silme i�lemi ba�ar�l� �ekilde ger�ekle�irse anasayafaya y�nlendir.

            return RedirectToAction("Index");
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(s => s.Id == id);
        }

        // Ya�� 18'den b�y�k olan ��rencileri sorgulayan LINQ sorgusu (QuerySyntax (Sorgu s�z dizimi))
        public IActionResult QuerySyntax()
        {
            var students = (from s in _context.Students
                            where s.Age > 18
                            select s).ToList();

            return View("Index", students);
        }

        // Ya�� 18'den k���k olan ��rencileri sorgulayan LINQ sorgusu (MethodSyntax (Method s�z dizimi))
        public IActionResult MethodSyntax()
        {
            var students = _context.Students
                .Where(s => s.Age < 18)
                .ToList();

            return View("Index", students);
        }

        // ��renci tablosu ile Kurs tablosunu Student Id ye g�re join eden aksiyon
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
            // Custom olu�turdu�umuz extension method ya� aral���na g�re gruplama yapar.
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
            // Sql taraf�nda olu�turudu�umuz procedure � tetikledik.
            var students = _context.Students
                .FromSqlInterpolated($"EXEC GetStudentByDepartment {department}")
                .ToList();
            ViewData["Students"] = students;

            return View();
        }

        [HttpPost("Transaction")]
        public IActionResult AddStudentsWithTransaction([FromBody] List<Student> students) //  [{"Name":"Ali","Age":20,"Department":"Bilgisayar"},{"Name":"Ay�e","Age":25,"Department":"Elektronik"}]
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
                // Hata durumunu d�nd�rebiliriz.
                return StatusCode(500, "��renciler Eklenirken bir hata olu�tu.");

            }
            // Ba�ar�l� durumda 200 d�nd�recek.
            return Ok();
        }

        // Ham Sql (Raw sql) sorgu ile ��rencileri Listeleme

        public IActionResult RawSql()
        {
            var students = _context.Students
                    .FromSqlRaw("SELECT * FROM Students WHERE Age > 18")
                    .ToList();

            return View("Index", students);
        }

        // EfCore BulkExtensions paketi kullanarak toplu ��renci ekleme
        public IActionResult BulkInsert()
        {
            var students = new List<Student>()
            {
                new Student() { Name = "Ahmet", Age = 20, Department = "End�stri" },
                new Student() { Name = "Elif", Age = 25, Department = "Mekatronik" }
            };

            _context.BulkInsert(students);

            return RedirectToAction("Index");
        }
    }
}
