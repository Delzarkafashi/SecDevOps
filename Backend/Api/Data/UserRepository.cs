using Dapper;
using Api.Models;

namespace Api.Data;

public class UserRepository
{
    private readonly Db _db;

    public UserRepository(Db db)
    {
        _db = db;
    }

    public async Task<UserAuthRow?> GetByIdentifierAsync(string identifier)
    {
        const string sql = @"
select
  id as Id,
  email as Email,
  username as Username,
  password_hash as Password_Hash,
  role as Role,
  is_active as Is_Active,
  failed_login_count as Failed_Login_Count,
  locked_until as Locked_Until
from users
where email = @identifier or username = @identifier
limit 1;
";
        using var conn = _db.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<UserAuthRow>(sql, new { identifier });
    }

    public async Task IncrementFailedAsync(long userId)
    {
        const string sql = @"
update users
set failed_login_count = failed_login_count + 1
where id = @userId;
";
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(sql, new { userId });
    }

    public async Task ResetFailedAsync(long userId)
    {
        const string sql = @"
update users
set failed_login_count = 0,
    locked_until = null
where id = @userId;
";
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(sql, new { userId });
    }

    public async Task LockAsync(long userId, DateTimeOffset until)
    {
        const string sql = @"
update users
set locked_until = @until
where id = @userId;
";
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(sql, new { userId, until });
    }

    public async Task UpdateLastLoginAsync(long userId)
    {
        const string sql = @"
update users
set last_login_at = now()
where id = @userId;
";
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(sql, new { userId });
    }
}