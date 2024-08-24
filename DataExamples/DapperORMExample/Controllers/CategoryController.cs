using Microsoft.AspNetCore.Mvc;

namespace DapperORMExample.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
