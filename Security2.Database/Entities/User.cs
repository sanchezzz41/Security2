using System;
using System.ComponentModel.DataAnnotations;

namespace Security2.Database.Entities
{
    public class User
    {
        [Key]
        public Guid UserGuid { get; set; }
        
        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string Key { get; set; }

        public string Name { get; set; }

        /// <inheritdoc />
        public User(string email, string passwordHash, string key, string name)
        {
            UserGuid = Guid.NewGuid();
            Email = email;
            PasswordHash = passwordHash;
            Key = key;
            Name = name;
        }

        protected User()
        {
        }
    }
}