namespace emensa.DataModels
{
    public partial class FriendRelation
    {
        public int FollowerId { get; set; }
        public int FollowedId { get; set; }

        public User Follower { get; set; }
        public User Followed { get; set; }
    }
}
