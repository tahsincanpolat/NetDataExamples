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
    }
}
