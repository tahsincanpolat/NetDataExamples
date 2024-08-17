using AdoNetExample.DbService.Abstract;
using AdoNetExample.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Diagnostics;

namespace AdoNetExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDbService _dbService;
        public HomeController(IDbService dbService)
        {
            _dbService = dbService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetData()
        {
            string query = "SELECT FirstName,LastName,Age FROM Students";
            var data = _dbService.ExecuteReader(query);
            return View(data);
        }

        [HttpGet]
        public IActionResult GetCount()
        {
            string query = "SELECT count(*) FROM Students";
            var data = _dbService.ExecuteScalar(query);
            return View(data);
        }


        [HttpPost]
        public IActionResult AddData()
        {
            string query = "INSERT INTO Students (FirstName,LastName,Age) VALUES('Ali','Demir','32')";
            _dbService.ExecuteNonQuery(query);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddDataSecure([FromForm] Student model)
        {
            // Sql sorgusunu parametrik olarak tanýmlar: Sql injection saldýrýlarýna karþý güvenli hale getirir.
            string query = "INSERT INTO Students (FirstName,LastName,Age) VALUES(@FirstName,@LastName,@Age)";
            SqlParameter[] parameters =
            {
                new SqlParameter("@FirstName",model.FirstName),
                new SqlParameter("@LastName",model.LastName),
                new SqlParameter("@Age",model.Age)
            };
            _dbService.ExecuteNonQuery(query,parameters);
            return RedirectToAction("Index");
        }


    }
}
