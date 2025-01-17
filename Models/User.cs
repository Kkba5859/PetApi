﻿namespace PetApi.Models
{
    public class User
    {
        public int Id { get; set; } // Уникальный идентификатор пользователя

        public string? Username { get; set; } // Имя пользователя

        public string? PasswordHash { get; set; } // Хэшированный пароль

        public bool IsLoggedIn { get; set; }

       
    }
}
