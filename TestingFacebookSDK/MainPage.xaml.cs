using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TestingFacebookSDK.Resources;
using Microsoft.Phone.Tasks;
using FacebookSDK;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;

namespace TestingFacebookSDK
{
    public partial class MainPage : PhoneApplicationPage
    {
        private CookieCollection cookies;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            cookies = new CookieCollection();
            web2.LoadCompleted += Web2_LoadCompleted;
            FacebookService.Instance.AppId = "282140562117538";
            FacebookService.Instance.ExtendedPermissions = "publish_actions, publish_stream, user_about_me, manage_pages, user_birthday, user_friends, user_status, user_likes, user_location, user_posts";
            FacebookService.Instance.IsPopup = true;
            FacebookService.Instance.Closed += Instance_Closed;
           
        }

        private void Instance_Closed(object sender, EventArgs e)
        {
            MessageBox.Show("Facebook SDK was closed by user!");
        }

        private void Web2_Navigating(object sender, NavigatingEventArgs e)
        {
            if (isClearCookiesWeb2)
                foreach (Cookie item in cookies)
                {
                    SetCookie(item.Name, item.Value, item.Path, item.Domain, false, item.Expires.ToString());
                }
        }

        bool isSetCookie = false;
        private void Web2_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (isClearCookiesWeb2 && !isSetCookie)
            {
                isSetCookie = true;
                foreach (Cookie item in cookies)
                {
                    SetCookie(item.Name, item.Value, item.Path, item.Domain, false, item.Expires.ToString());
                }

                web2.Navigate(new Uri("https://m.facebook.com/dialog/feed?link=http://tinhte.vn&name=Test+feed+dialog+Facebook+SDK&picture=https://tinhte.cdnforo.com/store/2015/12/3567335_cv.jpg&description=Canvas+là+một+tấm+thớt+đích+thực+giúp+người+dùng+sử+dụng+các+tablet+to+như+iPad+Pro,+iPad+hay+Surface+Pro+một+cách+dễ+dàng+và+rảnh+tay+hơn.+Vì+sao+mình+lại+gọi+nó+là+tấm+thớt&redirect_uri=https://m.facebook.com/v2.0/dialog/app_requests/submit&app_id=1834316393460181&display=touch", UriKind.RelativeOrAbsolute));
            }
        }

        private async void FeedButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Dictionary<string, object> feedParams = new Dictionary<string, object>();
            feedParams.Add("link", "http://tinhte.vn");
            feedParams.Add("name", "Test feed dialog Facebook SDK");
            feedParams.Add("picture", "https://tinhte.cdnforo.com/store/2015/12/3567335_cv.jpg");
            feedParams.Add("description", "Canvas là một tấm thớt đích thực giúp người dùng sử dụng các tablet to như iPad Pro, iPad hay Surface Pro một cách dễ dàng và rảnh tay hơn. Vì sao mình lại gọi nó là tấm thớt");

            var feedresult = await FacebookService.Instance.FeedDialogAsync(feedParams);

            Dispatcher.BeginInvoke(delegate
            {
                if (feedresult.IsSuccess)
                    MessageBox.Show(string.Format("PostId: {0}", feedresult.PostId));
                else
                    MessageBox.Show("Share thất bại");
            });
        }

        private async void GetFriendsButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var resultGetFriends = await FacebookService.Instance.GetInvitableFriendsAsync();

            Dispatcher.BeginInvoke(delegate
            {
                if (resultGetFriends.IsSuccess)
                    MessageBox.Show(string.Format("Get friends successfully!\n\n", resultGetFriends.Friends.ToString()));
                else
                    MessageBox.Show("Get friends thất bại");
            });
        }

        private async void LogoutButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            await FacebookService.Instance.LogoutAsync();

            Dispatcher.BeginInvoke(delegate
            {
                MessageBox.Show("Logout Facebook Successfully!");
            });
        }

        private async void LoginButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var resultLogin = await FacebookService.Instance.LoginAsync();

            Dispatcher.BeginInvoke(delegate
            {
                if (resultLogin.IsSuccess)
                    MessageBox.Show(string.Format("Login completed!\n\n{0}", resultLogin));
                else
                    MessageBox.Show("Login fail!");
            });
        }

        private void GotoPage1_tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Page1.xaml", UriKind.RelativeOrAbsolute));
        }

        private async void ClearCookiesWeb1_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            await web1.ClearCookiesAsync();
            await web1.ClearInternetCacheAsync();
            MessageBox.Show("Clear cookies webbrowser 1 thành công!");
        }

        bool isClearCookiesWeb2 = false;
        private async void ClearCookiesWeb2_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            await web2.ClearCookiesAsync();
            await web2.ClearInternetCacheAsync();
            MessageBox.Show("Clear cookies webbrowser 2 thành công!");
            isClearCookiesWeb2 = true;
            isSetCookie = false;
        }

        private void RefreshWeb1_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            web1.Navigate(new Uri("https://m.facebook.com", UriKind.RelativeOrAbsolute));
        }

        private void RefreshWeb2_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            web2.Navigate(new Uri("https://m.facebook.com/dialog/feed?link=http://tinhte.vn&name=Test+feed+dialog+Facebook+SDK&picture=https://tinhte.cdnforo.com/store/2015/12/3567335_cv.jpg&description=Canvas+là+một+tấm+thớt+đích+thực+giúp+người+dùng+sử+dụng+các+tablet+to+như+iPad+Pro,+iPad+hay+Surface+Pro+một+cách+dễ+dàng+và+rảnh+tay+hơn.+Vì+sao+mình+lại+gọi+nó+là+tấm+thớt&redirect_uri=https://m.facebook.com/v2.0/dialog/app_requests/submit&app_id=1834316393460181&display=touch", UriKind.RelativeOrAbsolute));
        }

        private void SetCookiesWeb2_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
        }

        private void SetCookie(string name, string value, string path = "", string domain = "", bool isSecure = false, string expires = "")
        {
            Dispatcher.BeginInvoke(() =>
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
                web2.InvokeScript("eval", cookieJs);
            });
        }

        private void web2_Navigated(object sender, NavigationEventArgs e)
        {
            if (!isClearCookiesWeb2)
                cookies = web2.GetCookies();
        }

        private async void LikePostButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (string.IsNullOrEmpty(ObjectIdToLike.Text))
                return;
            
            var resultLike = await FacebookService.Instance.LikeAsync(ObjectIdToLike.Text);

            Dispatcher.BeginInvoke(delegate
            {
                if (resultLike.IsSuccess)
                    MessageBox.Show(string.Format("Like thành công!\n\n{0}", resultLike));
                else
                    MessageBox.Show("Like error!\nMessage: " + resultLike.Message);
            });
        }

        private async void GetFriendsListButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            await FacebookService.Instance.GetUserFriends();
        }

        private void LikePageButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            FacebookService.Instance.LikePage("900160633407437");
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (FacebookService.Instance.BackButtonHandler())
                e.Cancel = true;
            base.OnBackKeyPress(e);
        }
    }
}