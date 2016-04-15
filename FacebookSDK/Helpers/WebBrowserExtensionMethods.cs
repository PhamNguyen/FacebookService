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
        public static FacebookSDKState CurrentState { get; private set; }
        private static TaskCompletionSource<NavigationEventArgs> navigatedTaskCompletionSource;
        private static TaskCompletionSource<LoginCompletedArgs> loginTaskCompletionSource;
        private static TaskCompletionSource<FeedCompletedArgs> feedTaskCompletionSource;
        private static TaskCompletionSource<AppRequestsCompletedArgs> appRequestTaskCompletionSource;
        private static EventHandler<NavigationEventArgs> onLoginCompleted;
        private static NavigationFailedEventHandler onLoginFailed;
        private static EventHandler<NavigationEventArgs> onFeedCompleted;
        private static NavigationFailedEventHandler onFeedFailed;
        private static EventHandler<NavigationEventArgs> onRequestCompleted;
        private static NavigationFailedEventHandler onRequestFailed;

        public static Task<NavigationEventArgs> NavigateAsync(this WebBrowser browser, Uri uri)
        {
            navigatedTaskCompletionSource = new TaskCompletionSource<NavigationEventArgs>();

            EventHandler<NavigationEventArgs> onNavigated = null;
            onNavigated = (object sender, NavigationEventArgs e) =>
            {
                browser.Navigated -= onNavigated;
                navigatedTaskCompletionSource.SetResult(e);
            };
            browser.Navigated += onNavigated;

            browser.Navigate(uri);

            return navigatedTaskCompletionSource.Task;
        }

        public static void CancelLoginFacebook(this WebBrowser browser)
        {
            browser.Navigated -= onLoginCompleted;
            browser.NavigationFailed -= onLoginFailed;
            if (loginTaskCompletionSource != null)
                loginTaskCompletionSource.TrySetResult(new LoginCompletedArgs(false));
        }

        public static Task<LoginCompletedArgs> LoginFacebookAsync(this WebBrowser browser, Uri uri, string fbAppId, string extendedPermissions)
        {
            CurrentState = FacebookSDKState.Login;

            loginTaskCompletionSource = new TaskCompletionSource<LoginCompletedArgs>();

            onLoginCompleted = null;
            onLoginCompleted = (object sender, NavigationEventArgs e) =>
            {
                LoginCompletedArgs result;
                LoginNavigationCallBack(e, fbAppId, extendedPermissions, out result);
                if (result != null)
                {
                    browser.Navigated -= onLoginCompleted;
                    loginTaskCompletionSource.SetResult(result);
                }
            };

            onLoginFailed = null;
            onLoginFailed = (object sender, NavigationFailedEventArgs e) =>
            {
                browser.NavigationFailed -= onLoginFailed;
                loginTaskCompletionSource.SetResult(new LoginCompletedArgs(false));
            };

            browser.Navigated += onLoginCompleted;
            browser.NavigationFailed += onLoginFailed;

            browser.Navigate(uri);

            return loginTaskCompletionSource.Task;
        }

        public static void CancelFeedFacebook(this WebBrowser browser)
        {
            browser.Navigated -= onFeedCompleted;
            browser.NavigationFailed -= onFeedFailed;
            if (feedTaskCompletionSource != null)
                feedTaskCompletionSource.TrySetResult(new FeedCompletedArgs(false));
        }

        public static Task<FeedCompletedArgs> FeedFacebookAsync(this WebBrowser browser, Uri uri)
        {
            CurrentState = FacebookSDKState.FeedDialog;

            feedTaskCompletionSource = new TaskCompletionSource<FeedCompletedArgs>();

            onFeedCompleted = null;
            onFeedCompleted = (object sender, NavigationEventArgs e) =>
            {
                FeedCompletedArgs result;
                FeedNavigationCallBack(e, out result);
                if (result != null)
                {
                    browser.Navigated -= onFeedCompleted;
                    feedTaskCompletionSource.SetResult(result);
                }
            };

            onFeedFailed = null;
            onFeedFailed = (object sender, NavigationFailedEventArgs e) =>
            {
                browser.NavigationFailed -= onFeedFailed;
                feedTaskCompletionSource.SetResult(new FeedCompletedArgs(false));
            };

            browser.Navigated += onFeedCompleted;
            browser.NavigationFailed += onFeedFailed;

            browser.Navigate(uri);

            return feedTaskCompletionSource.Task;
        }

        public static void CancelAppRequestFacebook(this WebBrowser browser)
        {
            browser.Navigated -= onFeedCompleted;
            browser.NavigationFailed -= onFeedFailed;
            if (appRequestTaskCompletionSource != null)
                appRequestTaskCompletionSource.TrySetResult(new AppRequestsCompletedArgs(false));
        }

        public static Task<AppRequestsCompletedArgs> AppRequestFacebookAsync(this WebBrowser browser, Uri uri)
        {
            CurrentState = FacebookSDKState.AppRequestDialog;

            appRequestTaskCompletionSource = new TaskCompletionSource<AppRequestsCompletedArgs>();

            onRequestCompleted = null;
            onRequestCompleted = (object sender, NavigationEventArgs e) =>
            {
                AppRequestsCompletedArgs result;
                RequestNavigationCallBack(e, out result);
                if (result != null)
                {
                    browser.Navigated -= onRequestCompleted;
                    appRequestTaskCompletionSource.SetResult(result);
                }
            };

            onRequestFailed = null;
            onRequestFailed = (object sender, NavigationFailedEventArgs e) =>
            {
                browser.NavigationFailed -= onRequestFailed;
                appRequestTaskCompletionSource.SetResult(new AppRequestsCompletedArgs(false));
            };

            browser.Navigated += onRequestCompleted;
            browser.NavigationFailed += onRequestFailed;

            browser.Navigate(uri);

            return appRequestTaskCompletionSource.Task;
        }

        #region Private Methods
        private static void LoginNavigationCallBack(NavigationEventArgs e, string fbAppId, string extendedPermissions, out LoginCompletedArgs loginResult)
        {
            if (CurrentState == FacebookSDKState.Login)
                loginResult = RetrieveLoginResponse(e, fbAppId, extendedPermissions);
            else
                loginResult = null;
        }

        private static void FeedNavigationCallBack(NavigationEventArgs e, out FeedCompletedArgs feedResult)
        {
            if (CurrentState == FacebookSDKState.FeedDialog)
                feedResult = RetrieveFeedResponse(e);
            else
                feedResult = null;
        }

        private static void RequestNavigationCallBack(NavigationEventArgs e, out AppRequestsCompletedArgs requestResult)
        {
            if (CurrentState == FacebookSDKState.AppRequestDialog)
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
            result = result.Remove(result.IndexOf(e.Uri.Fragment)).Remove(0, Constants.RedirectUriAppRequest.Length);
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
        public enum FacebookSDKState
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
