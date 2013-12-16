using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BibleBlahgua.Pages
{
    public partial class ImageViewer : PhoneApplicationPage
    {
        public ImageViewer()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.analytics.PostPageView("/ImageViewer");

            string imageStr = App.BlahguaAPI.CurrentBlah.ImageURL;

            if (imageStr != null)
            {
                Uri newUri = new Uri(imageStr, UriKind.Absolute);
                WebBrowserControl.Navigate(newUri);
            }
            else
            {
                WebBrowserControl.NavigateToString("<html><head><meta name='viewport' content='width=480, user-scalable=yes' /></head><body>Image load failed.  Please press the back button to return to your previous page.</body></html>");
            }
            
        }

    }
}