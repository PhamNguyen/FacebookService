namespace FacebookSDK.Models
{
    public class AppRequestsCompletedArgs
    {
        public bool IsSuccess { get; private set; }

        public AppRequestsCompletedArgs(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }
    }
}
