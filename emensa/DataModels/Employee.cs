using System;
using System.Collections.Generic;

namespace emensa.DataModels
{
    public partial class Employee
    {
        public int MemberId { get; set; }
        public string Office { get; set; }
        public string PhoneNumber { get; set; }

        public Member Member { get; set; }
    }
}
