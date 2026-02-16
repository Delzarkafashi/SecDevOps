using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Api.Data;

public class Db
{
    private readonly IConfiguration _config;

    public Db(IConfiguration config)
    {
        _config = config;
    }

    public NpgsqlConnection CreateConnection()
    {
        var cs = _config.GetConnectionString("Default");
        return new NpgsqlConnection(cs);
    }
}