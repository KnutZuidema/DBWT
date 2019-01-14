namespace emensa.ViewModels
{
    public class Login
    {
        public Login()
        {
            IsActivated = true;
            UserExists = true;
            IsPasswordValid = true;
        }
        public bool IsActivated { get; set; }
        public bool UserExists { get; set; }
        public bool IsPasswordValid { get; set; }
    }
}