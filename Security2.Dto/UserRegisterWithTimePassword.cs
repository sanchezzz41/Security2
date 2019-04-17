namespace Security2.Dto.Models
{
    public class UserRegisterWithTimePassword
    {
        public string Email { get; set; }

        public string Name { get; set; }

        public TimePasswordInfo TimePasswords { get; set; }

        public UserRegisterWithTimePassword()
        {
        }
        
        public UserRegisterWithTimePassword(string email, string name, TimePasswordInfo timePasswords)
        {
            Email = email;
            Name = name;
            TimePasswords = timePasswords;
        }

    }
}