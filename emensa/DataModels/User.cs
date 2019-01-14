using System;
using System.Collections.Generic;

namespace emensa.DataModels
{
    public partial class User
    {
        public User()
        {
            Following = new HashSet<FriendRelation>();
            Followers = new HashSet<FriendRelation>();
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Salt { get; set; }
        public string Hash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Created { get; set; }
        public byte Active { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime? LastLogin { get; set; }
        public int? Age { get; set; }

        public Guest Guest { get; set; }
        public Member Member { get; set; }
        public ICollection<FriendRelation> Following { get; set; }
        public ICollection<FriendRelation> Followers { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
