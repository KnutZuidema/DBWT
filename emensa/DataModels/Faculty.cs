using System;
using System.Collections.Generic;

namespace emensa.DataModels
{
    public partial class Faculty
    {
        public Faculty()
        {
            MemberFacultyRelation = new HashSet<MemberFacultyRelation>();
        }

        public int Id { get; set; }
        public string Website { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public ICollection<MemberFacultyRelation> MemberFacultyRelation { get; set; }
    }
}
