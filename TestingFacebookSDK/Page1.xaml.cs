using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FacebookSDK;

namespace TestingFacebookSDK
{
    public partial class Page1 : PhoneApplicationPage
    {
        public Page1()
        {
            InitializeComponent();
            App.MEAdsPopup.LoadAds();
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
            NavigationService.Navigate(new Uri("Page1.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}