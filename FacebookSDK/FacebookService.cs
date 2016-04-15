using Facebook;
using FacebookSDK.Helpers;
using FacebookSDK.Models;
using FacebookSDK.Views;
using Microsoft.Phone.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace FacebookSDK
{
    public class FacebookService
    {
        #region Fields
        private FBWebBrowserControl webView;

        private FacebookClient facebookClient;

        private Popup popup;

        private CurrentState currentState;

        private string appId;

        private string extendedPermissions;

        private bool isLoginInProgress = false;

        private DispatcherTimer webBrowserNavigatedTimer;

        private const int WebBrowserNavigatedTimeOut = 5;

        private const int CurrentOffet = 0;
        private const int LimitSize = 500;

        private string likePageHtmlContentTemplate = @"<html>
    <body>
        <div id=""fb-root""></div>
        <script>(function(d, s, id)
        {
            var js, fjs = d.getElementsByTagName(s)[0];
            if (d.getElementById(id)) return;
            js = d.createElement(s); js.id = id;
            js.src = ""//connect.facebook.net/en_US/sdk.js#xfbml=1&version=v2.5&appId=915480101870752"";
            fjs.parentNode.insertBefore(js, fjs);
        }(document, 'script', 'facebook-jssdk'));</script>
        <div class=""fb-page"" data-href=""https://www.facebook.com/900160633407437"" data-small-header=""true"" data-adapt-container-width=""true"" data-hide-cover=""false"" data-show-facepile=""true"">
            <div class=""fb-xfbml-parse-ignore"">
                <blockquote cite = ""https://www.facebook.com/900160633407437"" >
                    < a href=""https://www.facebook.com/900160633407437"">ME - Quỷ Hầu Vương</a>
                </blockquote>
            </div>
        </div>
    </body>
</html>";
        //private Uri UrlNavigation;

        //private bool isSetCookie = false;

        public bool IsLogged
        {
            get
            {
                if (AccessTokenData == null || string.IsNullOrEmpty(AccessTokenData.AccessToken) || string.IsNullOrEmpty(AccessTokenData.FacebookId))
                    return false;
                return true;
            }
        }
        #endregion

        #region Properties
        public string AppId
        {
            get
            {
                return appId;
            }

            set
            {
                if (value != appId)
                {
                    appId = value;
                    AccessTokenData.AppId = appId;
                }
            }
        }

        public string ExtendedPermissions
        {
            get
            {
                return extendedPermissions;
            }

            set
            {
                if (value != extendedPermissions)
                {
                    extendedPermissions = value;
                    AccessTokenData.CurrentPermissions = extendedPermissions.Split(',');
                }
            }
        }

        public AccessTokenData AccessTokenData { get; private set; }

        public bool IsPopup
        {
            get { return webView.ButtonClose.Visibility == Visibility.Visible; }
            set { webView.ButtonClose.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        }

        /// <summary>
        /// Sự kiện khi user bấm vào button Close để tắt FacebookSDK
        /// </summary>
        public event EventHandler Closed;
        #endregion

        #region Singleton

        private static readonly object SyncRoot = new object();

        private static volatile FacebookService instance;

        public static FacebookService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new FacebookService();
                        }
                    }
                }
                return instance;
            }
        }

        private FacebookService()
        {
            facebookClient = new FacebookClient();
            AccessTokenData = new AccessTokenData();

            webView = new FBWebBrowserControl();
            webView.WebBrowser.IsScriptEnabled = true;
            webView.WebBrowser.LoadCompleted += WebView_LoadCompleted;
            webView.WebBrowser.Navigated += WebView_Navigated;
            webView.WebBrowser.NavigationFailed += WebView_NavigationFailed;
            webView.ButtonClose.Tap += ButtonClose_Tap;

            IsPopup = true;
            popup = new Popup();

            webBrowserNavigatedTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, WebBrowserNavigatedTimeOut)
            };
            webBrowserNavigatedTimer.Tick += WebBrowserNavigatedTimerTick;
        }
        #endregion

        #region Public Methods
        public async Task<LoginCompletedArgs> LoginAsync()
        {
            ShowLoading(true, "Logging into Facebook...");

            currentState = CurrentState.Login;

            var loginParams = new Dictionary<string, object>();
            loginParams.Add(PostFields.Scope, ExtendedPermissions);
            loginParams.Add(PostFields.ClientId, AppId);
            loginParams.Add(PostFields.RedirectUri, Constants.RedirectUriLogin);
            loginParams.Add(PostFields.Display, "touch");
            loginParams.Add(PostFields.ResponseType, "token");

            var loginUrl = facebookClient.GetLoginUrl(loginParams);
            LoginCompletedArgs loginResult = new LoginCompletedArgs(false);

            if (IsLogged)
            {
                ShowLoading(false);

                HttpWebRequest webRequest = WebRequest.CreateHttp(string.Format("https://graph.facebook.com/me?access_token={0}", AccessTokenData.AccessToken));
                webRequest.Method = "GET";
                using (var response = (HttpWebResponse)(await Task<WebResponse>.Factory.FromAsync(webRequest.BeginGetResponse, webRequest.EndGetResponse, null)))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        NavigateToHide();

                        loginResult = new LoginCompletedArgs(true, AccessTokenData);
                    }
                    else
                    {
                        loginResult = await NavigateToLoginAsync(loginUrl);
                    }
                }
            }
            else
            {
                loginResult = await NavigateToLoginAsync(loginUrl);
            }

            return loginResult;
        }

        public async Task LogoutAsync()
        {
            ShowLoading(true, "Logging out from Facebook");

            currentState = CurrentState.Logout;

            if (AccessTokenData != null)
            {
                AccessTokenData.Reset();
            }
            await webView.WebBrowser.ClearCookiesAsync();
            await webView.WebBrowser.ClearInternetCacheAsync();
            DeleteCookiesFromIsolatedStorage();
            //isSetCookie = false;

            NavigateToHide();
        }

        public async Task<FeedCompletedArgs> FeedDialogAsync(Dictionary<string, object> feedParams)
        {
            ShowLoading(true, "Preparing to post feed");

            currentState = CurrentState.FeedDialog;

            feedParams.Add(PostFields.RedirectUri, Constants.RedirectUriAppRequest);
            feedParams.Add(PostFields.AppId, AppId);
            feedParams.Add(PostFields.Display, "touch");

            var feedDialogUrl = facebookClient.GetDialogUrl(FacebookCommand.Feed, feedParams);

            var result = await NavigateToFeedDialogAsync(feedDialogUrl);

            return result;
        }

        public async Task<AppRequestsCompletedArgs> AppRequestsDialogAsync(Dictionary<string, object> requestParams)
        {
            ShowLoading(true, "Preparing to post request");

            currentState = CurrentState.AppRequestDialog;

            requestParams.Add(PostFields.RedirectUri, Constants.RedirectUriAppRequest);
            requestParams.Add(PostFields.AppId, AppId);
            requestParams.Add(PostFields.Display, "touch");

            var appRequestDialogUrl = GetDialogUrl(FacebookCommand.Apprequests, requestParams);

            var result = await NavigateToAppRequestDialogAsync(appRequestDialogUrl);

            return result;
        }

        public async Task<GetInvitableFriendsCompletedArgs> GetInvitableFriendsAsync()
        {
            if (!IsLogged)
            {
                await LoginAsync();
            }

            //Kiểm tra lần nữa nếu vẫn chưa login thành công thì trả về bên ngoài là "Lấy danh sách bạn bè không thành công"
            if (!IsLogged)
                return new GetInvitableFriendsCompletedArgs(false);

            bool isSuccess = false;

            var graphApi = String.Format("/{0}/{1}", AccessTokenData.FacebookId, FacebookCommand.InvitableFriends);
            var invitableFriendsObject = await facebookClient.GetTaskAsync(graphApi, new { access_token = AccessTokenData.AccessToken, limit = 5000 });
            string invitableFriendsData = invitableFriendsObject.ToString();
            List<FBUser> friends = new List<FBUser>();

            if (!String.IsNullOrEmpty(invitableFriendsData))
            {
                var jsonData = JObject.Parse(invitableFriendsData);
                if (jsonData != null && jsonData["data"] != null)
                {
                    var stringData = jsonData["data"].ToString();

                    friends = JsonConvert.DeserializeObject<List<FBUser>>(stringData);
                }
                isSuccess = true;
            }
            return new GetInvitableFriendsCompletedArgs(isSuccess, friends);
        }

        /// <summary>
        /// Like một post or một page hoặc một đối tượng nào đó có thể like được
        /// </summary>
        /// <param name="objectId">Là id của đối tượng. VD: nếu muốn like 1 post thì objectId chính là postId của post đó</param>
        /// <returns>Nếu like thành công thì trả về true. Ngược lại trả về false.</returns>
        public async Task<LikeCompletedArgs> LikeAsync(string objectId)
        {
            if (!ExtendedPermissions.Contains(Constants.PublishActionsPermission))
                throw new KeyNotFoundException("Bạn phải yêu cầu quyền \"publish_action\" để có thể thực hiện chức năng này.");

            if (!IsLogged)
            {
                await LoginAsync();
            }

            //Kiểm tra lần nữa nếu vẫn chưa login thành công thì trả về bên ngoài là like không thành công
            if (!IsLogged)
                return new LikeCompletedArgs(false, "Người dùng chưa login. Bạn cần login trước khi có thể like!");

            bool isSuccess = false;

            var graphApi = String.Format("/v2.2/{0}/{1}", objectId, FacebookCommand.Like);
            try
            {
                var likeObjectResult = await facebookClient.PostTaskAsync(graphApi, new { access_token = "CAACEdEose0cBAPOQjRpbqOGH3Nml22buiTAg1ZCEjSibZAzpAFj6ZCc6fNlwzlevqSHtcEbq97iTgN4cEA2JFhcT6eKL7iFU0ZBfyGkCKrGLwPJZByZBWLQJqD3UrNYhTa0zEEHeBWUTPPcu9G2K6FY6JnbNDKvf3HNQNDHfto6PgDtPbaKdKsbTM2IPSMDgcef67JNeUYG7CZBgnZCwDCw3K9XLNDIsuHEZD" });
                string likeObjectResultData = likeObjectResult.ToString();
                if (!String.IsNullOrEmpty(likeObjectResultData))
                {
                    if (likeObjectResultData.Contains("true"))
                        isSuccess = true;
                }
                return new LikeCompletedArgs(isSuccess, likeObjectResultData);
            }
            catch (Exception ex)
            {
                string message = string.Format("Like error! Message: {0}", ex.Message);
                Debug.WriteLine(message);
                return new LikeCompletedArgs(false, ex.Message);
            }
        }

        /// <summary>
        /// Lưu ý: Hàm này chỉ show fanpage đó lên chứ không bắt được user có like hay không nha.
        /// </summary>
        /// <param name="pageId">Id của page</param>
        public void LikePage(string pageId)
        {
            webView.WebBrowser.Navigate(new Uri(string.Format("https://m.facebook.com/{0}", pageId), UriKind.RelativeOrAbsolute));
            webView.WebBrowser.Visibility = Visibility.Visible;
            ShowPopup();
            //            webView.WebBrowser.NavigateToString(@"<html>
            //    <body>
            //        <div id=""fb-root""></div>
            //        <script>(function(d, s, id)
            //        {
            //            var js, fjs = d.getElementsByTagName(s)[0];
            //            if (d.getElementById(id)) return;
            //            js = d.createElement(s); js.id = id;
            //            js.src = ""connect.facebook.net/en_US/sdk.js#xfbml=1&version=v2.5&appId=915480101870752"";
            //            fjs.parentNode.insertBefore(js, fjs);
            //        }(document, 'script', 'facebook-jssdk'));</script>
            //        <div class=""fb-page"" data-href=""https://www.facebook.com/900160633407437"" data-small-header=""true"" data-adapt-container-width=""true"" data-hide-cover=""false"" data-show-facepile=""true"">
            //            <div class=""fb-xfbml-parse-ignore"">
            //                <blockquote cite = ""https://www.facebook.com/900160633407437"" >
            //                    < a href=""https://www.facebook.com/900160633407437"">ME - Quỷ Hầu Vương</a>
            //                </blockquote>
            //            </div>
            //        </div>
            //    </body>
            //</html>");
        }

        public async Task<FBUser> GetUserProfile(params string[] parameters)
        {
            while (AccessTokenData == null || string.IsNullOrEmpty(AccessTokenData.AccessToken))
            {
                await LoginAsync();
            }

            string fieldsValue = string.Empty;

            foreach (var item in parameters)
            {
                fieldsValue += string.Format("{0},", item);
            }

            try
            {
                object userInfo;

                if (string.IsNullOrEmpty(fieldsValue))
                    userInfo = await facebookClient.GetTaskAsync(string.Format("/{0}", FacebookCommand.Me), new { access_token = AccessTokenData.AccessToken });
                else
                    userInfo = await facebookClient.GetTaskAsync(string.Format("/{0}", FacebookCommand.Me), new { access_token = AccessTokenData.AccessToken, fields = fieldsValue });

                var fbUser = JsonConvert.DeserializeObject<FBUser>(userInfo.ToString());
                fbUser.ProfilePicture.PictureData.Url = new Uri(String.Format("http://graph.facebook.com/{0}/picture?type=large", fbUser.Id), UriKind.RelativeOrAbsolute);
                return fbUser;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("GetUserProfile exception: {0}", ex.Message));
                return null;
            }
        }

        public async Task<FBFriends> GetUserFriends()
        {
            while (AccessTokenData == null || string.IsNullOrEmpty(AccessTokenData.AccessToken))
            {
                await LoginAsync();
            }

            var fbFriendsResult = await facebookClient.GetTaskAsync(string.Format("/{0}/{1}", FacebookCommand.Me, FacebookCommand.Friends),
                new { access_token = AccessTokenData.AccessToken, limit = LimitSize, offset = CurrentOffet });

            if (fbFriendsResult != null)
            {
                var result = JsonConvert.DeserializeObject<FBFriends>(fbFriendsResult.ToString());
                return result;
            }
            return null;
        }
        /// <summary>
        /// Hàm này để bên ngoài handle khi user sử dụng nút back thì sẽ tắt cái FacebookSDK đi
        /// </summary>
        /// <returns>
        /// Trả về true là cho biết FacebookSDK đang được hiển thị, cần phải tắt đi.
        /// Trả về false là cho biết FacebookSDK đã được tắt, bên ngoài không cần phải tắt nữa.
        /// </returns>
        public bool BackButtonHandler()
        {
            //Nếu FacebookSDK vẫn đang hiển thị
            if (popup.IsOpen)
            {
                ForceClose();
                return true;
            }
            return false;
        }
        #endregion

        #region Private Methods

        private void WebBrowserNavigatedTimerTick(object sender, EventArgs e)
        {
            ShowLoading(false);
            webBrowserNavigatedTimer.Stop();
        }

        private Uri GetDialogUrl(string dialog, Dictionary<string, object> parameters)
        {
            StringBuilder requestUrlBuilder = new StringBuilder();
            requestUrlBuilder.Append("https://m.facebook.com/dialog/");
            requestUrlBuilder.Append(dialog);
            requestUrlBuilder.Append("?");
            foreach (var item in parameters)
            {
                requestUrlBuilder.AppendFormat("&{0}={1}", item.Key, item.Value);
            }

            var url = requestUrlBuilder.ToString().Remove(requestUrlBuilder.ToString().IndexOf('&'), 1);
            var uri = new Uri(url, UriKind.RelativeOrAbsolute);
            return uri;
        }

        private void SaveCookiesToIsolatedStorage(CookieCollection cookies)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            settings.Add(Constants.CookiesSettingKey, cookies);
        }

        /// <summary>
        /// Get cookies that were stored from IsolatedStorage. Return null if not exist "cookies" key
        /// </summary>
        /// <returns></returns>
        private CookieCollection ReadCookiesFromIsolatedStorage()
        {
            CookieCollection cookies = new CookieCollection();
            var settings = IsolatedStorageSettings.ApplicationSettings;

            if (!settings.TryGetValue<CookieCollection>(Constants.CookiesSettingKey, out cookies))
                return null;

            return cookies;
        }

        private void DeleteCookiesFromIsolatedStorage()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            settings.Remove(Constants.CookiesSettingKey);
        }

        private void ShowLoading(bool isShow, string contentLoading = "")
        {
            webView.ShowLoading(isShow, contentLoading);
            if (isShow)
                webBrowserNavigatedTimer.Start();
        }

        private void WebView_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            ForceClose();
        }

        private void WebView_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Uri.Fragment))
            {
                ShowLoading(false);
            }
            else
            {
                switch (currentState)
                {
                    case CurrentState.Login:
                        if (e.Uri.AbsoluteUri.StartsWith(Constants.RedirectUriLogin))
                        {
                            //var cookies = webView.WebBrowser.GetCookies();
                            //if (cookies != null)
                            //    SaveCookiesToIsolatedStorage(cookies);

                            NavigateToHide();
                        }
                        break;
                    case CurrentState.FeedDialog:
                        if (e.Uri.AbsoluteUri.StartsWith(Constants.RedirectUriFeed))
                            NavigateToHide();
                        break;
                    case CurrentState.Logout:
                        break;
                    case CurrentState.AppRequestDialog:
                        NavigateToHide();
                        break;
                    case CurrentState.AppInviteDialog:
                        break;
                    case CurrentState.MessageDialog:
                        break;
                }
            }
        }

        private void SetCookie(string name, string value, string path = "", string domain = "", bool isSecure = false, string expires = "")
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendFormat("document.cookie = \"{0}={1}", name, value);
                if (!String.IsNullOrEmpty(path))
                    sb.AppendFormat("; Path={0}", path);
                if (!String.IsNullOrEmpty(domain))
                    sb.AppendFormat("; Domain={0}", domain);
                if (isSecure)
                    sb.Append(";secure ;\";");
                else
                    sb.Append(" ;\";");

                var cookieJs = sb.ToString();
                Debug.WriteLine(cookieJs);
                webView.WebBrowser.InvokeScript("eval", cookieJs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SetCookie exception: {0}", ex.Message);
            }
        }

        private void WebView_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.Uri.ToString().Contains("login.php"))
            {
                try
                {
                    webView.WebBrowser.InvokeScript("eval", new string[1]
                    {
                       "document.querySelector('div.acb').style.display = 'none';"
                    });
                }
                catch (SystemException ex)
                {
                }
            }

            //webView.WebBrowser.InvokeScript("eval", "( function (){window.external.notify('abc');})();");

            //            < script type = "text/javascript" >
            //     window.console = {
            //                log: function(str) { window.external.notify(str); }
            //            };

            //            // output errors to console log
            //            window.onerror = function(e) {
            //                console.log("window.onerror ::" + JSON.stringify(e));
            //            };

            //            console.log("WP8 WB Log Console samle");
            //</ script >

            //if (!e.Uri.ToString().Contains(Contants.UriNavigateBlankPage) && !isSetCookie)
            //{
            //    if (e.Uri.ToString().Contains("https://m.facebook.com"))
            //    {
            //        isSetCookie = true;
            //        var cookies = ReadCookiesFromIsolatedStorage();
            //        if (cookies != null)
            //        {
            //            foreach (Cookie item in cookies)
            //            {
            //                SetCookie(item.Name, item.Value, item.Path, item.Domain, false, item.Expires.ToString());
            //            }
            //        }
            //        webView.WebBrowser.Navigate(UrlNavigation);
            //    }
            //}
        }

        private void ForceClose()
        {
            NavigateToHide();
            switch (WebBrowserExtensionMethods.CurrentState)
            {
                case WebBrowserExtensionMethods.FacebookSDKState.Login:
                    webView.WebBrowser.CancelLoginFacebook();
                    break;
                case WebBrowserExtensionMethods.FacebookSDKState.FeedDialog:
                    webView.WebBrowser.CancelFeedFacebook();
                    break;
                case WebBrowserExtensionMethods.FacebookSDKState.AppRequestDialog:
                    webView.WebBrowser.CancelAppRequestFacebook();
                    break;
            }

            if (Closed != null)
                Closed(this, new EventArgs());
        }

        private void ButtonClose_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ForceClose();
        }

        private void ShowPopup()
        {
            var currentPage = (Application.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage;

            webView.Height = currentPage.ActualHeight;
            webView.Width = currentPage.ActualWidth;

            if (IsPopup)
            {
                webView.WebBrowser.Margin = new Thickness(24);
            }
            else
            {
                webView.WebBrowser.Margin = new Thickness(0);
            }
            popup.Child = webView;
            popup.IsOpen = true;
        }

        private async Task<LoginCompletedArgs> NavigateToLoginAsync(Uri loginDialogUrl)
        {
            ShowPopup();

            //UrlNavigation = loginDialogUrl;
            //isSetCookie = false;
            //webView.WebBrowser.Navigate(new Uri("https://m.facebook.com", UriKind.RelativeOrAbsolute));

            var result = await webView.WebBrowser.LoginFacebookAsync(loginDialogUrl, AppId, ExtendedPermissions);
            if (result != null && result.IsSuccess)
            {
                if (AccessTokenData != null)
                {
                    AccessTokenData = result.AccessTokenData;

                    var user = await GetUserProfile();

                    if (user != null)
                    {
                        AccessTokenData.FacebookId = user.Id;
                        result.UserInfo = user;
                    }
                }
            }
            return result;
        }

        private async Task<AppRequestsCompletedArgs> NavigateToAppRequestDialogAsync(Uri appRequestDialogUrl)
        {
            ShowPopup();

            //UrlNavigation = appRequestDialogUrl;//Lưu lại link request để sau khi set Cookies xong thì refresh bằng link này.
            //isSetCookie = false;
            //webView.WebBrowser.Navigate(new Uri("https://m.facebook.com", UriKind.RelativeOrAbsolute));

            var result = await webView.WebBrowser.AppRequestFacebookAsync(appRequestDialogUrl);

            return result;
        }

        private async Task<FeedCompletedArgs> NavigateToFeedDialogAsync(Uri feedDialogUrl)
        {
            ShowPopup();

            //UrlNavigation = feedDialogUrl;
            //isSetCookie = false;
            //webView.WebBrowser.Navigate(new Uri("https://m.facebook.com", UriKind.RelativeOrAbsolute));

            var result = await webView.WebBrowser.FeedFacebookAsync(feedDialogUrl);

            return result;
        }

        private void NavigateToHide()
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                popup.IsOpen = false;
                webView.WebBrowser.Navigate(new Uri(Constants.UriNavigateBlankPage, UriKind.RelativeOrAbsolute));
            });
        }
        #endregion
        private enum CurrentState
        {
            Login,
            Logout,
            FeedDialog,
            AppInviteDialog,
            AppRequestDialog,
            MessageDialog
        }

        private static class PostFields
        {
            public const string Scope = "scope";
            public const string ClientId = "client_id";
            public const string AppId = "app_id";
            public const string RedirectUri = "redirect_uri";
            public const string Display = "display";
            public const string ResponseType = "response_type";
        }

        private static class FacebookCommand
        {
            public const string Feed = "feed";
            public const string Apprequests = "apprequests";
            public const string InvitableFriends = "invitable_friends";
            public const string Me = "me";
            public const string Like = "likes";
            public const string Friends = "friends";
        }
    }
}
