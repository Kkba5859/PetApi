using Microsoft.AspNetCore.Mvc;
using PetApi.Data;
using Microsoft.EntityFrameworkCore;

namespace PetApi.Controllers
{
    [Route("api/Auth/logout")]
    [ApiController]
    public class LogoutController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LogoutController> _logger;

        public LogoutController(ApplicationDbContext context, ILogger<LogoutController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username))
                {
                    return BadRequest("Поле username обязательно.");
                }

                var user = await _context.Users
                    .Where(u => u.Username == request.Username)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    _logger.LogWarning("Logout attempted for non-existing user: {Username}", request.Username);
                    return NotFound("Пользователь не найден.");
                }

                // Установить IsLoggedIn = false
                user.IsLoggedIn = false;
                await _context.SaveChangesAsync();

                _logger.LogInformation("User logged out successfully: {Username}", request.Username);

                return Ok("Вы успешно вышли из системы.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for user: {Username}", request.Username);
                return StatusCode(500, "Произошла ошибка при выходе.");
            }
        }
        public class LogoutRequest
        {
            public string Username { get; set; } = string.Empty;
        }


    }

}
