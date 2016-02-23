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

namespace TesttingFacebookSDKInLibrary
{
    public partial class WindowsPhoneControl1 : UserControl
    {
        public WindowsPhoneControl1()
        {
            InitializeComponent();
            FacebookService.Instance.AppId = "1834316393460181";
            FacebookService.Instance.ExtendedPermissions = "user_about_me,user_birthday,user_photos,user_posts,user_friends,read_stream, publish_actions";
            FacebookService.Instance.IsPopup = true;
        }

        private async void Button_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var loginResult = await FacebookService.Instance.LoginAsync();
            if (loginResult.IsSuccess)
            {
                MessageBox.Show(string.Format("Login FB {0} thành công!", loginResult.UserInfo.Name));
            }
            else
            {
                MessageBox.Show("Login Facebook thất bại! :(");
            }
        }

        private async void ButtonLogout_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            await FacebookService.Instance.LogoutAsync();
        }

        private async void ButtonPostFeed_Tap(object sender, System.Windows.Input.GestureEventArgs e)
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
    }
}
