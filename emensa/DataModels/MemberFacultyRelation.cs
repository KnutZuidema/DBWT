using System;
using System.Collections.Generic;

namespace emensa.DataModels
{
    public partial class MemberFacultyRelation
    {
        public int MemberId { get; set; }
        public int FacultyId { get; set; }

        public Faculty Faculty { get; set; }
        public Member Member { get; set; }
    }
}
