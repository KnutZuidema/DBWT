using System;
using System.Collections.Generic;

namespace emensa.DataModels
{
    public partial class FriendRelation
    {
        public int Initiator { get; set; }
        public int Receiver { get; set; }

        public User InitiatorNavigation { get; set; }
        public User ReceiverNavigation { get; set; }
    }
}
