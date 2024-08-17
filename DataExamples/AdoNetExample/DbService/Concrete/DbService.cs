using AdoNetExample.DbService.Abstract;
using AdoNetExample.Models;
using System.Data.SqlClient;

namespace AdoNetExample.DbService.Concrete
{
    public class DbService : IDbService
    {
        private readonly string _connectionString;

        public DbService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // INSERT INTO Students (FirstName,LastName,Age) VALUES('Tahsin','Canpolat','32')
        public void ExecuteNonQuery(string query)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        // eğer sorgunuz insert,update,delete işlemlerini içeriyorsa ExecuteNonQuery kullanılır. Geriye int döndürür
        public void ExecuteNonQuery(string query, SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    if(parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        // eğer sorgunuz geriye tablo döndürüyorsa kullanılır (SELECT * FROM Students)
        public List<Student> ExecuteReader(string query)
        {
            var result = new List<Student>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var model = new Student()
                            {
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Age = Convert.ToInt32(reader["Age"])
                            };

                            result.Add(model);
                        }
                    }
                }
            }

            return result;
        }

        // Eğer sorguda geriye tek bir değer dönüyorsa kullanılır.Geriye object döndürür. select count(*) from Students
        public object ExecuteScalar(string query)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    return command.ExecuteScalar();
                }
            }
        }
    }
}
