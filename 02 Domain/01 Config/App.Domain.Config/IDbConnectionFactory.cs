using System.Data;

namespace App.Domain.Config;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
