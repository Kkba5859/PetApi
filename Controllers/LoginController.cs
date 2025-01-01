using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PetApi.Models;
using PetApi.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace PetApi.Controllers
{
    [Route("api/Auth/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginController> _logger;
        private readonly IMapper _mapper;

        public LoginController(ApplicationDbContext context, IConfiguration configuration, ILogger<LoginController> logger, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
        }

        // Метод для входа пользователя
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserDto request)
        {
            try
            {
                var user = await _context.Users
                    .Where(u => u.Username == request.Username)
                    .FirstOrDefaultAsync();

                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Invalid login attempt for user: {Username}", request.Username);
                    return BadRequest("Неверные учетные данные.");
                }

                // Установить IsLoggedIn = true
                user.IsLoggedIn = true;
                await _context.SaveChangesAsync();

                var claims = new[] { new Claim(ClaimTypes.Name, user.Username) };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: creds
                );

                _logger.LogInformation("User logged in successfully: {Username}", request.Username);

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user: {Username}", request.Username);
                return StatusCode(500, "Произошла ошибка при входе.");
            }
        }

    }
}
