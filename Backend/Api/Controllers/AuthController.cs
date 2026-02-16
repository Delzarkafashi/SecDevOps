using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserRepository _users;
        private readonly IConfiguration _config;

        public AuthController(UserRepository users, IConfiguration config)
        {
            _users = users;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var identifier = (request.Identifier ?? "").Trim();
            var password = request.Password ?? "";

            if (identifier.Length == 0 || password.Length == 0)
                return BadRequest("identifier and password required");

            var user = await _users.GetByIdentifierAsync(identifier);

            if (user is null)
                return Unauthorized("invalid credentials");

            if (!user.Is_Active)
                return Forbid("inactive account");

            var now = DateTimeOffset.UtcNow;

            if (user.Locked_Until.HasValue && user.Locked_Until.Value > now)
                return Forbid("account locked");

            var ok = BCrypt.Net.BCrypt.Verify(password, user.Password_Hash);

            if (!ok)
            {
                await _users.IncrementFailedAsync(user.Id);

                var newCount = user.Failed_Login_Count + 1;

                if (newCount >= 5)
                {
                    await _users.LockAsync(user.Id, now.AddMinutes(15));
                    return Forbid("account locked");
                }

                return Unauthorized("invalid credentials");
            }

            await _users.ResetFailedAsync(user.Id);
            await _users.UpdateLastLoginAsync(user.Id);

            var token = CreateJwt(user.Id, user.Role);

            return Ok(new
            {
                token,
                role = user.Role
            });
        }

        private string CreateJwt(long userId, string role)
        {
            var key = _config["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(key))
                throw new Exception("Jwt:Key saknas i User Secrets");

            var creds = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256
            );

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var jwt = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}