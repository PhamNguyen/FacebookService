namespace FacebookSDK.Views
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public partial class FBWebBrowserControl : UserControl
    {
        public FBWebBrowserControl()
        {
            InitializeComponent();
            WebBrowser.Navigated += WebBrowser_Navigated;
            WebBrowser.Navigating += WebBrowser_Navigating;
            WebBrowser.ScriptNotify += WebBrowser_ScriptNotify;
            WebBrowser.NavigationFailed += WebBrowser_NavigationFailed;
            ButtonClose.Tap += ButtonClose_Tap;
            ButtonClose.MouseLeave += ButtonClose_MouseLeave;
        }

        private void WebBrowser_ScriptNotify(object sender, Microsoft.Phone.Controls.NotifyEventArgs e)
        {
            Debug.WriteLine("WebBrowser_ScriptNotify: e.Value = " + e.Value);
        }

        public void ShowLoading(bool isShow, string contentLoading = "")
        {
            if (isShow)
            {
                if (!string.IsNullOrEmpty(contentLoading))
                {
                    LoadingStatusTextBlock.Text = contentLoading;
                }
                LoadingStatusTextBlock.Visibility = Visibility.Visible;
                StatusProgressBar.Visibility = Visibility.Visible;
                WebBrowser.Visibility = Visibility.Collapsed;
                LoadingProgressbarContainer.Visibility = Visibility.Visible;
            }
            else
            {
                LoadingStatusTextBlock.Text = string.Empty;
                WebBrowser.Visibility = Visibility.Visible;
                LoadingProgressbarContainer.Visibility = Visibility.Collapsed;
            }
        }
        
        private void WebBrowser_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            Debug.WriteLine("WebBrowser_NavigationFailed: e.Uri = " + e.Uri);
            LoadingProgressbarContainer.Visibility = Visibility.Collapsed;
            return;
        }

        private void WebBrowser_Navigating(object sender, Microsoft.Phone.Controls.NavigatingEventArgs e)
        {
            Debug.WriteLine("WebBrowser_Navigating: e.Uri = " + e.Uri);
            LoadingProgressbarContainer.Visibility = Visibility.Visible;
            return;
        }

        private void WebBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            Debug.WriteLine("WebBrowser_Navigated: e.Uri = " + e.Uri);
            LoadingProgressbarContainer.Visibility = Visibility.Collapsed;
            return;
        }

        private void ButtonClose_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ButtonClose.Foreground = new SolidColorBrush((Color)Application.Current.Resources["PhoneAccentColor"]);
            ButtonClose.FontSize = 38;
        }

        private void ButtonClose_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ButtonClose.Foreground = new SolidColorBrush(Colors.White);
            ButtonClose.FontSize = 36;
        }
    }
}
