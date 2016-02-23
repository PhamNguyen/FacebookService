
using System.Collections.Generic;

namespace FacebookSDK.Models
{
    public class GetInvitableFriendsCompletedArgs
    {
        public bool IsSuccess { get; private set; }
        public List<FBUser> Friends { get; private set; }

        public GetInvitableFriendsCompletedArgs(bool isSuccess, List<FBUser> friends=null)
        {
            Friends = friends;
            IsSuccess = isSuccess;
        }
    }
}