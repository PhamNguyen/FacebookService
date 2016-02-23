using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace FacebookSDK.Views
{
    public partial class LoadingBar : UserControl
    {
        public Grid Panel { get; set; }
        public LoadingBar()
        {
            InitializeComponent();
            stbRotation.Begin();
            Panel = LayoutRoot;
        }
        public void SetText(String text)
        {
            lblMess.Text = text;
        }
    }
}
