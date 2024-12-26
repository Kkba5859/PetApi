using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetApi.Data;
using System.Text;

namespace PetApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Добавляем сервисы в контейнер
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Настройка базы данных SQLite
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            // Настройка аутентификации JWT
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
                        )
                    };
                });

            // Настройка CORS (разрешаем запросы с клиента)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()  // Разрешаем запросы с любых источников
                          .AllowAnyHeader()  // Разрешаем любые заголовки
                          .AllowAnyMethod(); // Разрешаем любые методы HTTP (GET, POST, PUT, DELETE)
                });
            });

            // Настройка приложения
            var app = builder.Build();

            // Включаем Swagger для разработки
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Применяем редирект на HTTPS
            app.UseHttpsRedirection();

            // Включаем CORS
            app.UseCors("AllowAll");

            // Аутентификация и авторизация
            app.UseAuthentication();
            app.UseAuthorization();

            // Маршруты контроллеров
            app.MapControllers();

            // Запуск приложения
            app.Run();
        }
    }
}
