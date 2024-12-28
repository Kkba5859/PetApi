using Microsoft.AspNetCore.Mvc;
using PetApi.Models;
using PetApi.Data;
using Microsoft.EntityFrameworkCore;

namespace PetApi.Controllers
{
    [Route("api/Auth/register")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RegisterController> _logger;

        public RegisterController(ApplicationDbContext context, ILogger<RegisterController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Метод регистрации пользователя
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserDto request)
        {
            try
            {
                // Проверка существования пользователя с использованием асинхронного запроса
                var existingUser = await _context.Users
                    .Where(u => u.Username == request.Username)
                    .FirstOrDefaultAsync();

                if (existingUser != null)
                {
                    _logger.LogWarning("User already exists: {Username}", request.Username);
                    return BadRequest("Пользователь уже существует.");
                }

                // Создание пользователя с хэшированным паролем
                var user = new User
                {
                    Username = request.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User registered successfully: {Username}", request.Username);

                return Ok("Пользователь зарегистрирован.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for user: {Username}", request.Username);
                return StatusCode(500, "Произошла ошибка при регистрации.");
            }
        }
    }
}
