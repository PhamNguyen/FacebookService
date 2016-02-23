namespace FacebookSDK.Models
{
    public class LoginCompletedArgs
    {
        public bool IsSuccess { get; private set; }
        public AccessTokenData AccessTokenData { get; private set; }

        public FBUser UserInfo { get; set; }

        public LoginCompletedArgs(bool isSuccess, AccessTokenData accessTokenData = null, FBUser userInfo=null)
        {
            AccessTokenData = accessTokenData;
            IsSuccess = isSuccess;
            UserInfo = userInfo;
        }
    }
}
