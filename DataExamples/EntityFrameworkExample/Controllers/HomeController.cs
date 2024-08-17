using EntityFrameworkExample.Data;
using EntityFrameworkExample.Models;
using Microsoft.AspNetCore.Mvc;
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

            if(student == null)
            {
                // Öðrenci bulunamazsa 404 sayfasýna yönlendir.
                return NotFound();
            }

            return View(student);
        }
    }
}
