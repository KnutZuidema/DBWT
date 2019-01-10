using System;
using System.Collections.Generic;

namespace emensa.DataModels
{
    public partial class User
    {
        public User()
        {
            FriendRelationInitiatorNavigation = new HashSet<FriendRelation>();
            FriendRelationReceiverNavigation = new HashSet<FriendRelation>();
            Order = new HashSet<Order>();
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
        public ICollection<FriendRelation> FriendRelationInitiatorNavigation { get; set; }
        public ICollection<FriendRelation> FriendRelationReceiverNavigation { get; set; }
        public ICollection<Order> Order { get; set; }
    }
}
