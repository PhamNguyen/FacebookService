namespace FacebookSDK.Models
{
    public class LikeCompletedArgs
    {
        public string Message { get; private set; }
        public bool IsSuccess { get; private set; }

        public LikeCompletedArgs(bool isSuccess, string message = null)
        {
            Message = message;
            IsSuccess = isSuccess;
        }
    }
}
