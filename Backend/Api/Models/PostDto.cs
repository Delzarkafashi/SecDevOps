namespace Api.Models;

public class PostDto
{
    public long Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public DateTimeOffset Created_At { get; set; }
    public DateTimeOffset? Updated_At { get; set; }
    public long Created_By { get; set; }

    public string CreatedByEmail { get; set; } = "";
}