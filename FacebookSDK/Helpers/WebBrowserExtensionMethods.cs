namespace FacebookSDK.Helpers
{
    using FacebookSDK.Models;
    using Microsoft.Phone.Controls;
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using System.Windows.Navigation;

    public static class WebBrowserExtensionMethods
    {
        private static FacebookSDKState currentState;
        
        public static Task<NavigationEventArgs> WebBrowerNavigated(this WebBrowser browser)
        {
            var tcs = new TaskCompletionSource<NavigationEventArgs>();

            EventHandler<NavigationEventArgs> onNavigated = null;
            onNavigated = (object sender, NavigationEventArgs e) =>
            {
                browser.Navigated -= onNavigated;
                tcs.SetResult(e);
            };
            browser.Navigated += onNavigated;
            return tcs.Task;
        }

        public static Task<LoginCompletedArgs> LoginCompleted(this WebBrowser browser, string fbAppId, string extendedPermissions)
        {
            currentState = FacebookSDKState.Login;

            var tcs = new TaskCompletionSource<LoginCompletedArgs>();

            EventHandler<NavigationEventArgs> onLoginCompleted = null;
            onLoginCompleted = (object sender, NavigationEventArgs e) =>
            {
                LoginCompletedArgs result;
                LoginNavigationCallBack(e, fbAppId, extendedPermissions, out result);
                if (result != null)
                {
                    browser.Navigated -= onLoginCompleted;
                    tcs.SetResult(result);
                }
            };

            NavigationFailedEventHandler onLoginFailed = null;
            onLoginFailed = (object sender, NavigationFailedEventArgs e) =>
            {
                browser.NavigationFailed -= onLoginFailed;
                tcs.SetResult(new LoginCompletedArgs(false));
            };

            browser.Navigated += onLoginCompleted;
            browser.NavigationFailed += onLoginFailed;

            return tcs.Task;
        }

        public static Task<FeedCompletedArgs> FeedCompleted(this WebBrowser browser)
        {
            currentState = FacebookSDKState.FeedDialog;

            var tcs = new TaskCompletionSource<FeedCompletedArgs>();

            EventHandler<NavigationEventArgs> onFeedCompleted = null;
            onFeedCompleted = (object sender, NavigationEventArgs e) =>
            {
                FeedCompletedArgs result;
                FeedNavigationCallBack(e, out result);
                if (result != null)
                {
                    browser.Navigated -= onFeedCompleted;
                    tcs.SetResult(result);
                }
            };

            NavigationFailedEventHandler onFeedFailed = null;
            onFeedFailed = (object sender, NavigationFailedEventArgs e) =>
            {
                browser.NavigationFailed -= onFeedFailed;
                tcs.SetResult(new FeedCompletedArgs(false));
            };

            browser.Navigated += onFeedCompleted;
            browser.NavigationFailed += onFeedFailed;
            return tcs.Task;
        }

        public static Task<AppRequestsCompletedArgs> AppRequestCompleted(this WebBrowser browser)
        {
            currentState = FacebookSDKState.AppRequestDialog;

            var tcs = new TaskCompletionSource<AppRequestsCompletedArgs>();

            EventHandler<NavigationEventArgs> onRequestCompleted = null;
            onRequestCompleted = (object sender, NavigationEventArgs e) =>
            {
                AppRequestsCompletedArgs result;
                RequestNavigationCallBack(e, out result);
                if (result != null)
                {
                    browser.Navigated -= onRequestCompleted;
                    tcs.SetResult(result);
                }
            };

            NavigationFailedEventHandler onRequestFailed = null;
            onRequestFailed = (object sender, NavigationFailedEventArgs e) =>
            {
                browser.NavigationFailed -= onRequestFailed;
                tcs.SetResult(new AppRequestsCompletedArgs(false));
            };

            browser.Navigated += onRequestCompleted;
            browser.NavigationFailed += onRequestFailed;

            return tcs.Task;
        }

        #region Private Methods
        private static void LoginNavigationCallBack(NavigationEventArgs e, string fbAppId, string extendedPermissions, out LoginCompletedArgs loginResult)
        {
            if (currentState == FacebookSDKState.Login)
                loginResult = RetrieveLoginResponse(e, fbAppId, extendedPermissions);
            else
                loginResult = null;
        }

        private static void FeedNavigationCallBack(NavigationEventArgs e, out FeedCompletedArgs feedResult)
        {
            if (currentState == FacebookSDKState.FeedDialog)
                feedResult = RetrieveFeedResponse(e);
            else
                feedResult = null;
        }

        private static void RequestNavigationCallBack(NavigationEventArgs e, out AppRequestsCompletedArgs requestResult)
        {
            if (currentState == FacebookSDKState.AppRequestDialog)
                requestResult = RetrieveAppRequestResponse(e);
            else
                requestResult = null;
        }

        private static LoginCompletedArgs RetrieveLoginResponse(NavigationEventArgs e, string fbAppId, string extendedPermissions)
        {
            if (!e.Uri.AbsoluteUri.StartsWith(Constants.RedirectUriLogin))
                return null;
            string str1 = HttpUtility.HtmlDecode(e.Uri.Fragment).TrimStart('#');

            var accessTokenData = new AccessTokenData();
            accessTokenData.CurrentPermissions = extendedPermissions.Split(',');
            accessTokenData.AppId = fbAppId;
            foreach (string str2 in str1.Split('&'))
            {
                string[] strArray = str2.Split('=');
                if (strArray.Length == 2)
                {
                    switch (strArray[0])
                    {
                        case "access_token":
                            accessTokenData.AccessToken = strArray[1];
                            break;
                        case "expires_in":
                            DateTime expired;
                            if (DateTime.TryParse(strArray[1], out expired))
                                accessTokenData.Expires = expired;
                            break;
                        case "state":
                            accessTokenData.State = strArray[1];
                            break;
                        case "issued":
                            DateTime issued;
                            if (DateTime.TryParse(strArray[1], out issued))
                                accessTokenData.Issued = issued;
                            break;
                    }
                }
            }
            return new LoginCompletedArgs(true, accessTokenData);
        }

        private static FeedCompletedArgs RetrieveFeedResponse(NavigationEventArgs e)
        {
            if (!e.Uri.AbsoluteUri.StartsWith(Constants.RedirectUriFeed))
                return null;
            bool isPostSuccess = false;
            string postId = string.Empty;
            string facebookUserId = string.Empty;//chỗ này fb có trả về userId nên cứ lấy ra để đây cũng được

            string result = HttpUtility.HtmlDecode(e.Uri.ToString());
            result = result.Remove(result.IndexOf(e.Uri.Fragment)).Remove(0, Constants.RedirectUriFeed.Length);
            if (!string.IsNullOrEmpty(result) && result.Contains("?"))
                result = result.Remove(result.IndexOf('?'), 1);
            foreach (string item in result.Split('&'))
            {
                string[] strArray = item.Split('=');
                if (strArray.Length == 2)
                {
                    if (strArray[0] == "post_id")
                    {
                        string[] splitValue = strArray[1].Split('_');

                        if (splitValue.Length == 2)
                        {
                            facebookUserId = splitValue[0];
                            postId = splitValue[1];

                            isPostSuccess = true;
                            break;
                        }
                    }
                }
            }
            return new FeedCompletedArgs(isPostSuccess, facebookUserId, postId);
        }

        private static AppRequestsCompletedArgs RetrieveAppRequestResponse(NavigationEventArgs e)
        {
            bool isRequestSuccessful = false;
            if (e.Uri.ToString().StartsWith(Constants.RedirectUriRequestError))
            {
                isRequestSuccessful = false;
            }
            else if (!e.Uri.AbsoluteUri.StartsWith("https://m.facebook.com/?request"))
                return null;
            else if (e.Uri.ToString().StartsWith(Constants.RedirectUriAppRequest))
            {
                isRequestSuccessful = true;
            }
            return new AppRequestsCompletedArgs(isRequestSuccessful);
        }
        #endregion
        private enum FacebookSDKState
        {
            Login,
            Logout,
            FeedDialog,
            AppInviteDialog,
            AppRequestDialog,
            MessageDialog
        }
    }
}
