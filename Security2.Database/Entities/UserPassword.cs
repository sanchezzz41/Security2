using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Security2.Database.Entities
{
    public class UserPassword
    {
        [ForeignKey(nameof(User))]
        public Guid UserGuid { get; set; }

        public string Password { get; set; }

        public DateTime Date { get; set; }

        public bool IsUsed { get; set; }

        public virtual User User { get; set; }

        public UserPassword()
        {
        }

        public UserPassword(Guid userGuid, string password, DateTime date)
        {
            UserGuid = userGuid;
            Password = password;
            Date = date;
            IsUsed = false;
        }
    }
}