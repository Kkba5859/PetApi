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

            // ��������� ������� � ���������
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ��������� ���� ������ SQLite
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            // ��������� �������������� JWT
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

            // ��������� CORS (��������� ������� � �������)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()  // ��������� ������� � ����� ����������
                          .AllowAnyHeader()  // ��������� ����� ���������
                          .AllowAnyMethod(); // ��������� ����� ������ HTTP (GET, POST, PUT, DELETE)
                });
            });

            // ��������� ����������
            var app = builder.Build();

            // �������� Swagger ��� ����������
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // ��������� �������� �� HTTPS
            app.UseHttpsRedirection();

            // �������� CORS
            app.UseCors("AllowAll");

            // �������������� � �����������
            app.UseAuthentication();
            app.UseAuthorization();

            // �������� ������������
            app.MapControllers();

            // ������ ����������
            app.Run();
        }
    }
}
