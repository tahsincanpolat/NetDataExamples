using Dapper;
using DapperORMExample.Data;
using DapperORMExample.Models;
using Microsoft.AspNetCore.Mvc;

namespace DapperORMExample.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DapperContext _context;

        public CategoryController(DapperContext context)
        {
            _context = context;
        }

        // Tüm kategorileri listeleyen metot
        public async Task<IActionResult> Index()
        {
            // tüm kategoriler listeyelen sorgu
            var query = "select * from Categories";
            using (var connection = _context.CreateConnection())
            {
                // Dapper ile kategorileri sorgulama
                var categories = await connection.QueryAsync<Category>(query);
                return View(categories.ToList());
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            var query = "INSERT INTO Categories (Name) VALUES (@Name)";
            using (var connection = _context.CreateConnection())
            {
                // Dapper ile ürün ekleme
                await connection.ExecuteAsync(query, category);
                return RedirectToAction(nameof(Index)); // işlem tamamlandığında anasayfaya yönlendir.
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var query = "SELECT * FROM Categories WHERE CategoryId = @Id";

            using (var connection = _context.CreateConnection())
            {
                // Dapper ile tek bir ürünü getirme 
                var category = await connection.QuerySingleOrDefaultAsync<Category>(query, new { Id = id });
                if (category == null)
                {
                    return NotFound();
                }
                return View(category);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            var query = "update Categories set Name = @Name Where CategoryId = @CategoryId";

            using (var connection = _context.CreateConnection())
            {
                // Dapper ile ürünü güncelleme
                await connection.ExecuteAsync(query, category);

                return RedirectToAction("Index"); // işlem tamamlandığında anasayfaya yönlendir.
            }
        }

        // Kategorilerin detayı
        public async Task<IActionResult> Details(int id)
        {
            var query = "select * from Categories where CategoryId = @Id";

            using (var connection = _context.CreateConnection())
            {
                // Dapper ile bir categoryi sorgulama
                var category = await connection.QuerySingleOrDefaultAsync<Category>(query, new { Id = id });
                if(category == null)
                {
                    return NotFound();
                }
                return View(category);
            }
            
        }

        public async Task<IActionResult> Delete(int id)
        {
            var query = "select * from Categories where CategoryId = @Id";
            using (var connection = _context.CreateConnection()){
                var category = await connection.QuerySingleOrDefaultAsync<Category>(query, new { Id = id });
                if(category == null)
                {
                    return NotFound();
                }
                return View(category);
            }
        }

        [HttpPost,ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var queryProduct = "select * from Products where CategoryId = @Id";



            // belirli bir kategoriyi silme sorgusu
            var query = "delete from Categories where CategoryId = @Id";
            using (var connection = _context.CreateConnection())
            {
                var product = await connection.QuerySingleOrDefaultAsync<Product>(queryProduct, new { Id = id });
                if(product != null) // eğer bu categorye ait bir ürün varsa silme işlemini gerçekleştirme
                {
                    return View();
                }
                await connection.ExecuteAsync(query,new { Id = id});
                return RedirectToAction("Index"); // silme gerçekleştiğinde anasayfaya yönlendir.
            }
        }
    }
}
