using Api.Models;
using Dapper;

namespace Api.Data;

public class PostRepository
{
    private readonly Db _db;

    public PostRepository(Db db)
    {
        _db = db;
    }

    public async Task<List<PostDto>> GetAllAsync()
    {
        using var conn = _db.CreateConnection();

        var sql = """
        SELECT
          p.id,
          p.title,
          p.content,
          p.created_at,
          p.updated_at,
          p.created_by,
          u.email AS created_by_email
        FROM posts p
        JOIN users u ON u.id = p.created_by
        ORDER BY p.id DESC;
        """;

        var rows = await conn.QueryAsync<PostDto>(sql);
        return rows.ToList();
    }

    public async Task<PostDto> CreateAsync(string title, string content, long userId)
    {
        using var conn = _db.CreateConnection();

        var sql = """
        INSERT INTO posts (title, content, created_by, updated_by)
        VALUES (@title, @content, @userId, @userId)
        RETURNING id, title, content, created_at, updated_at, created_by;
        """;

        var created = await conn.QuerySingleAsync<PostDto>(sql, new { title, content, userId });

        var emailSql = "SELECT email FROM users WHERE id = @userId;";
        created.CreatedByEmail = await conn.QuerySingleAsync<string>(emailSql, new { userId });

        return created;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        using var conn = _db.CreateConnection();

        var sql = "DELETE FROM posts WHERE id = @id;";
        var affected = await conn.ExecuteAsync(sql, new { id });
        return affected > 0;
    }

    // Ny metod: UpdateAsync
    public async Task<PostDto?> UpdateAsync(long id, string title, string content, long userId)
    {
        using var conn = _db.CreateConnection();

        // Kontrollera om admin eller staff ska uppdatera sina egna
        var isAdmin = await conn.ExecuteScalarAsync<bool>("SELECT role='admin' FROM users WHERE id=@userId", new { userId });

        var sql = @"
                    UPDATE posts
                    SET title=@title,
                        content=@content,
                        updated_by=@userId,
                        updated_at=now()
                    WHERE id=@id
                    ";

        if (!isAdmin)
        {
            // Staff får bara uppdatera sina egna posts
            sql += " AND created_by=@userId";
        }

        sql += " RETURNING id, title, content, created_at, updated_at, created_by;";

        var updated = await conn.QuerySingleOrDefaultAsync<PostDto>(sql, new { id, title, content, userId });

        if (updated != null)
        {
            var emailSql = "SELECT email FROM users WHERE id=@userId";
            updated.CreatedByEmail = await conn.QuerySingleAsync<string>(emailSql, new { userId });
        }

        return updated;
    }
}