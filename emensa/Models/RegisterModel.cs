namespace emensa.Models
{
    public class RegisterModel
    {
        public bool PasswordError { get; set; }
        public bool UsernameError { get; set; }
        public bool EmailError { get; set; }
        public bool MatriculationNumberError { get; set; }
        public bool PhoneNumberError { get; set; }
        public bool OfficeError { get; set; }
        public bool RoleError { get; set; }
    }
}