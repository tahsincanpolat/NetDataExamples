using Dapper;
using DapperORMExample.Data;
using DapperORMExample.Models;
using Microsoft.AspNetCore.Mvc;

namespace DapperORMExample.Controllers
{
    public class ProductController : Controller
    {
        private readonly DapperContext _context;

        public ProductController(DapperContext context)
        {
            _context = context;
        }

        // Tüm ürünleri ve kategorileri listeleyen anasayfa metodu
        public async Task<IActionResult> Index()
        {
            // ürünleri ve kategorileri birleştirerek seçen sql sorgusu
            var query = "select * from Products join Categories on Products.CategoryId = Categories.CategoryId";

            // Dapper ile çoklu tablo sorgusu
            using (var connection  = _context.CreateConnection())
            {
                var products = await connection.QueryAsync<Product, Category, Product>(
                 query,
                 (product, category) =>
                 {
                     product.Category = category;
                     return product;
                 },
                 splitOn: "CategoryId" // Kategorilere ayırmak için kullanılır
                );

                return View(products.ToList());

            }

        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            var query = "INSERT INTO Products (Name,Price,CategoryId) VALUES (@Name,@Price,@CategoryId)";
            using (var connection = _context.CreateConnection())
            {
                // Dapper ile ürün ekleme
                await connection.ExecuteAsync(query,product);
                return RedirectToAction(nameof(Index)); // işlem tamamlandığında anasayfaya yönlendir.
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var query = "SELECT * FROM Products WHERE ProductId = @Id";

            using (var connection = _context.CreateConnection())
            {
                // Dapper ile tek bir ürünü getirme 
                var product = await connection.QuerySingleOrDefaultAsync<Product>(query,new { Id = id});
                if(product == null)
                {
                    return NotFound();
                }
                return View(product);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            var query = "update Products set Name = @Name, Price = @Price, CategoryId = @CategoryId Where ProductId = @ProductId";

            using (var connection = _context.CreateConnection())
            {
                // Dapper ile ürünü güncelleme
                await connection.ExecuteAsync(query,product);

                return RedirectToAction("Index"); // işlem tamamlandığında anasayfaya yönlendir.
            }
        }

        // Ürün silme sayfasında ürünün id sine göre ürünün gelmesi
        public async Task<IActionResult> Delete(int id)
        {
            var query = "SELECT * FROM Products WHERE ProductId = @Id";

            using (var connection = _context.CreateConnection())
            {
                // Dapper ile tek bir ürünü getirme 
                var product = await connection.QuerySingleOrDefaultAsync<Product>(query, new { Id = id });
                if (product == null)
                {
                    return NotFound("Product Not Found");
                }
                return View(product);
            }
        }

        [HttpPost,ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Ürünü silmek için query
            var query = "Delete from Products Where ProductId = @Id";

            using (var connection = _context.CreateConnection())
            {
                // Dapper ile ürün silme
                var result = await connection.ExecuteAsync(query, new { Id = id });
                if (result > 0)
                {
                    ViewBag.Message = "Product Deleted Successfully";
                }
                else
                {
                    ViewBag.Message = "Product Deletion failed";
                }

                return View("DeleteResult");
            }
        }

        // ürürün detaylarını görüntüleyen sayfa aksiyonu
        public async Task<IActionResult> Details(int id)
        {
            var query = "SELECT * FROM Products WHERE ProductId = @Id";

            using (var connection = _context.CreateConnection())
            {
                // Dapper ile tek bir ürünü getirme 
                var product = await connection.QuerySingleOrDefaultAsync<Product>(query, new { Id = id });
                if (product == null)
                {
                    return NotFound("Product Not Found");
                }
                return View(product);
            }
        }
    }
}
