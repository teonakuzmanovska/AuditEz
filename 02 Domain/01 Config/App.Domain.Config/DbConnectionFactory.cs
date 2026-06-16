using System.Data;
using Microsoft.Data.SqlClient;

namespace App.Domain.Config;

public class DbConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        return new SqlConnection(connectionString);
    }
}