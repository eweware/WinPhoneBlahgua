using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;

namespace WinPhoneBlahgua
{
    public partial class BlahDetails : PhoneApplicationPage
    {
        ApplicationBar appBar = null;
        ApplicationBarIconButton promoteBtn;
        ApplicationBarIconButton demoteBtn;
        ApplicationBarIconButton shareBtn;
        ApplicationBarIconButton commentBtn;
        ApplicationBarMenuItem reportItem;
        ApplicationBarMenuItem deleteItem;


        public BlahDetails()
        {
            InitializeComponent();
            appBar = new ApplicationBar();
            this.DataContext = App.BlahguaAPI;

            appBar.IsVisible = true;
            appBar.IsMenuEnabled = true;

            promoteBtn = new ApplicationBarIconButton(new Uri("Images\\Icons\\promote.png", UriKind.Relative));
            promoteBtn.Text = "promote";
            promoteBtn.Click += new EventHandler(HandlePromoteBlah);

            demoteBtn = new ApplicationBarIconButton(new Uri("Images\\Icons\\demote.png", UriKind.Relative));
            demoteBtn.Text = "demote";
            demoteBtn.Click += new EventHandler(HandleDemoteBlah);

            shareBtn = new ApplicationBarIconButton(new Uri("Images\\Icons\\share.png", UriKind.Relative));
            shareBtn.Text = "share";
            shareBtn.Click += new EventHandler(HandlePromoteBlah);

            commentBtn = new ApplicationBarIconButton(new Uri("Images\\Icons\\comment.png", UriKind.Relative));
            commentBtn.Text = "add comment";
            commentBtn.Click += new EventHandler(HandlePromoteBlah);

            appBar.Buttons.Add(promoteBtn);
            appBar.Buttons.Add(demoteBtn);
            appBar.Buttons.Add(shareBtn);
            appBar.Buttons.Add(commentBtn);
        }

        private void HandlePromoteBlah(object target, EventArgs theArgs)
        {

        }

        private void HandleDemoteBlah(object target, EventArgs theArgs)
        {

        }

        private void HandleShareBlah(object target, EventArgs theArgs)
        {

        }

        private void HandleAddComment(object target, EventArgs theArgs)
        {

        }

        private void HandleImageOpened(object sender, RoutedEventArgs e)
        {
            BlahImage.MaxHeight = ((BitmapImage)BlahImage.Source).PixelHeight;
            BlahImage.MaxWidth = ((BitmapImage)BlahImage.Source).PixelWidth;
        }
     

        

    }
}