namespace emensa.Models
{
    public class Employee : Member
    {
        public string Office { get; set; }
        public string PhoneNumber { get; set; }

        public Employee()
        {
        }

        public Employee(User user)
        {
            Username = user.Username;
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Birthday = user.Birthday;
        }
    }
}