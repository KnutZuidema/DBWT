namespace emensa.Models
{
    public class Student : Member
    {
        public uint MatriculationNumber;
        public Major Major;

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