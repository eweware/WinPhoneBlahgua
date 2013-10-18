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
        ApplicationBarIconButton promoteBtn;
        ApplicationBarIconButton demoteBtn;
        ApplicationBarIconButton shareBtn;
        ApplicationBarIconButton commentBtn;
        ApplicationBarMenuItem reportItem;
        ApplicationBarMenuItem deleteItem;



        public BlahDetails()
        {
            InitializeComponent();
            this.DataContext = App.BlahguaAPI;
            this.ApplicationBar = new ApplicationBar();
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.IsVisible = true;

            promoteBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/promote.png", UriKind.Relative));
            promoteBtn.Text = "promote";
            promoteBtn.Click += HandlePromoteBlah;

            demoteBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/demote.png", UriKind.Relative));
            demoteBtn.Text = "demote";
            demoteBtn.Click += HandleDemoteBlah;


            shareBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/share.png", UriKind.Relative));
            shareBtn.Text = "share";
            shareBtn.Click += HandleShareBlah;


            commentBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/comment.png", UriKind.Relative));
            commentBtn.Text = "comment";
            commentBtn.Click += HandleAddComment;

            reportItem = new ApplicationBarMenuItem("flag post as inappropriate");
            reportItem.Click += HandleReportItem;

            deleteItem = new ApplicationBarMenuItem("remove post");
            deleteItem.Click += HandleDeleteItem;



            App.BlahguaAPI.LoadBlahComments((allComments) =>
                {
                    AllCommentList.ItemsSource = allComments;
                    TopCommentList.ItemsSource = App.BlahguaAPI.CurrentBlah.TopComments;
                }
            );
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

        private void HandleReportItem(object target, EventArgs theArgs)
        {

        }

        private void HandleDeleteItem(object target, EventArgs theArgs)
        {

        }

        private void HandleImageOpened(object sender, RoutedEventArgs e)
        {
            BlahImage.MaxHeight = ((BitmapImage)BlahImage.Source).PixelHeight;
            BlahImage.MaxWidth = ((BitmapImage)BlahImage.Source).PixelWidth;
        }

        private void OnPivotLoading(object sender, PivotItemEventArgs e)
        {
            // do the background
            Storyboard sb = new Storyboard();
            DoubleAnimation db = new DoubleAnimation();
            double targetVal = 0;
            double maxScroll = -320;
            double offset;

            if (BlahDetailsPivot.Items.Count() > 1)
                offset = maxScroll / (BlahDetailsPivot.Items.Count() - 1);
            else
                offset = 0;
            ExponentialEase ease = new ExponentialEase();
            ease.Exponent = 5;
            ease.EasingMode = EasingMode.EaseIn;

            targetVal = offset * BlahDetailsPivot.Items.IndexOf(e.Item);
            db.EasingFunction = ease;
            db.BeginTime = TimeSpan.FromSeconds(0);
            db.Duration = TimeSpan.FromSeconds(.5);
            db.To = targetVal;
            Storyboard.SetTarget(db, BackgroundImage);
            Storyboard.SetTargetProperty(db, new PropertyPath("(Canvas.Left)"));
            sb.Children.Add(db);
            sb.Begin();
        }

        private void HandlePivotLoaded(object sender, PivotItemEventArgs e)
        {

            switch (e.Item.Header.ToString())
            {
                case "summary":
                    ApplicationBar.Buttons.Add(promoteBtn);
                    ApplicationBar.Buttons.Add(demoteBtn);
                    ApplicationBar.Buttons.Add(commentBtn);
                    ApplicationBar.Buttons.Add(shareBtn);

                    ApplicationBar.MenuItems.Add(reportItem);
                    ApplicationBar.MenuItems.Add(deleteItem);
                    ApplicationBar.IsVisible = true;
                    break;

                case "comments":
                    ApplicationBar.Buttons.Add(promoteBtn);
                    ApplicationBar.Buttons.Add(demoteBtn);
                    ApplicationBar.Buttons.Add(commentBtn);
                    ApplicationBar.MenuItems.Add(reportItem);
                    ApplicationBar.MenuItems.Add(deleteItem);
                    ApplicationBar.IsVisible = true;
                    break;

                case "stats":
                    ApplicationBar.IsVisible = false;
                    break;

                default:


                    break;
            }
        }

        private void HandlePivotUnloaded(object sender, PivotItemEventArgs e)
        {
            ApplicationBar.Buttons.Clear();
            ApplicationBar.MenuItems.Clear();
        }

        

    }
}