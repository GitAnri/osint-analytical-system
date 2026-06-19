using Business.Services;
using Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Command;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JwtService _jwtService;

        public AuthController(IUserService userService, JwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { Message = "Username and password are required." });

            try
            {
                var user = await _userService.RegisterAsync(dto.Username, dto.Password);

                return Ok(new
                {
                    user.Id,
                    user.Username,
                    user.Role
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { Message = "Username and password are required." });

            User user;
            try
            {
                user = await _userService.AuthenticateAsync(dto.Username, dto.Password);
            }
            catch
            {
                return Unauthorized(new { Message = "Invalid username or password." });
            }

            var token = _jwtService.GenerateToken(user.Id.ToString(), user.Role);

            return Ok(new
            {
                Token = token,
                User = new { user.Id, user.Username, user.Role }
            });
        }

        [HttpGet("whoami")]
        [Authorize]
        public async Task<IActionResult> WhoAmI()
        {
            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(idClaim) || !int.TryParse(idClaim, out int userId))
                return Unauthorized(new { Message = "Invalid token claims." });

            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
                return NotFound(new { Message = "User not found in database." });

            return Ok(new
            {
                user.Id,
                user.Username,
                user.Role
            });
        }

        [HttpPut("users/{userId}/role")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateUserRole(int userId, [FromBody] UpdateRoleDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Role))
                return BadRequest(new { Message = "Role cannot be empty." });

            await _userService.UpdateRoleAsync(userId, dto.Role);
            return Ok(new { Message = $"User {userId} role successfully updated to {dto.Role}." });
        }
    }
}
