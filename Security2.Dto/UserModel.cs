namespace Security2.Dto.Models
{
    public class UserModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }
        
        public string PasswordHash { get; set; }

        /// <inheritdoc />
        public UserModel(string email, string password, string name, string passwordHash, string key)
        {
            Email = email;
            Password = password;
            Name = name;
            PasswordHash = passwordHash;
        }
    }
}