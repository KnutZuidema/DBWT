namespace emensa.Models
{
    public class Student : Member
    {
        public uint MatriculationNumber { get; set; }
        public Major Major { get; set; }

        public Student()
        {
        }

        public Student(User user)
        {
            Username = user.Username;
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Birthday = user.Birthday;
        }
    }
}