using System.Collections.Generic;

namespace emensa.DataModels
{
    public partial class Member
    {
        public Member()
        {
            MemberFacultyRelation = new HashSet<MemberFacultyRelation>();
        }

        public int UserId { get; set; }

        public User User { get; set; }
        public Employee Employee { get; set; }
        public Student Student { get; set; }
        public ICollection<MemberFacultyRelation> MemberFacultyRelation { get; set; }
    }
}
