using AdoNetExample.Models;
using System.Data.SqlClient;

namespace AdoNetExample.DbService.Abstract
{
    public interface IDbService
    {
        void ExecuteNonQuery(string query);
        void ExecuteNonQuery(string query, SqlParameter[] parameters);
        List<Student> ExecuteReader(string query);
        object ExecuteScalar(string query);
    }
}
