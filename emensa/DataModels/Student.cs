using System.Collections.Generic;

namespace emensa.DataModels
{
    public partial class Student
    {
        public Student()
        {
            Comment = new HashSet<Comment>();
        }

        public int MemberId { get; set; }
        public int MatriculationNumber { get; set; }
        public string Major { get; set; }

        public Member Member { get; set; }
        public ICollection<Comment> Comment { get; set; }
    }
}
