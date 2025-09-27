using Microsoft.Data.SqlClient;
using System.Data;

namespace APIPracticaExamen.Data;

public class DbConnectionFactory
{
    private readonly string _conn;
    public DbConnectionFactory(IConfiguration cfg)
        => _conn = cfg.GetConnectionString("AzureSql")
            ?? throw new InvalidOperationException("Falta ConnectionStrings:AzureSql");

    public IDbConnection Create() => new SqlConnection(_conn);
}
