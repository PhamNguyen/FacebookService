namespace FacebookSDK.Models
{
    public class FeedCompletedArgs
    {
        public string PostId { get; private set; }
        public string FacebookUserId { get; private set; }
        public bool IsSuccess { get; private set; }

        public FeedCompletedArgs(bool isSuccess, string facebookUserId = null, string postId = null)
        {
            PostId = postId;
            FacebookUserId = facebookUserId;
            IsSuccess = isSuccess;
        }
    }
}
