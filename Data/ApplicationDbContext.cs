using Microsoft.EntityFrameworkCore;
using PetApi.Models;

namespace PetApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Определение DbSet для работы с сущностями
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Конфигурация сущностей
            modelBuilder.Entity<User>().HasKey(u => u.Id);

            // Уникальность имени пользователя
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Другие настройки (если потребуется)
        }
    }
}
