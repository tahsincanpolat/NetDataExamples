using Microsoft.Data.SqlClient;
using System.Data;

namespace DapperORMExample.Data
{
    public class DapperContext
    {
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Veritabanı bağlantısı
        // Bu metot connectionstring i kullanrak yeni bir SqlConnection nesnesi oluşturur ve döndürür.
        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
