using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly PostRepository _posts;

    public PostsController(PostRepository posts)
    {
        _posts = posts;
    }

    // viewer, staff, admin
    [HttpGet]
    [Authorize(Roles = "viewer,staff,admin")]
    public async Task<ActionResult<List<PostDto>>> GetAll()
    {
        var rows = await _posts.GetAllAsync();
        return Ok(rows);
    }

    // staff, admin
    [HttpPost]
    [Authorize(Roles = "staff,admin")]
    public async Task<ActionResult<PostDto>> Create([FromBody] PostCreateRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Title) || string.IsNullOrWhiteSpace(req.Content))
            return BadRequest("title och content kravs");

        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var created = await _posts.CreateAsync(req.Title.Trim(), req.Content.Trim(), userId.Value);
        return Ok(created);
    }

    // admin only
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(long id)
    {
        var ok = await _posts.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }

    // staff, admin – uppdatera
    [HttpPut("{id:long}")]
    [Authorize(Roles = "staff,admin")]
    public async Task<ActionResult<PostDto>> Update(long id, [FromBody] PostUpdateRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Title) || string.IsNullOrWhiteSpace(req.Content))
            return BadRequest("title och content kravs");

        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var updated = await _posts.UpdateAsync(id, req.Title.Trim(), req.Content.Trim(), userId.Value);
        if (updated == null) return NotFound();

        return Ok(updated);
    }

    private long? GetUserId()
    {
        var v =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? User.FindFirstValue("userId");

        return long.TryParse(v, out var id) ? id : null;
    }
}