namespace emensa.Models
{
    public class LoginModel
    {
        public string Username;
        public string Password;
        public string Hash;
        public string Salt;
        public bool UserError;
        public bool PasswordError;
    }
}