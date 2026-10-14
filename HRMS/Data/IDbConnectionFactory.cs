using Npgsql;

namespace HRMS.Data
{
    public interface IDbConnectionFactory
    {
        NpgsqlConnection CreateConnection();
    }
}
