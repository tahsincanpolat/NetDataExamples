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

            if(student == null)
            {
                // ��renci bulunamazsa 404 sayfas�na y�nlendir.
                return NotFound();
            }

            return View(student);
        }
    }
}
