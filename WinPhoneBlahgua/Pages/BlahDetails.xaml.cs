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
        ApplicationBarIconButton signInBtn;
        ApplicationBarIconButton promoteBtn;
        ApplicationBarIconButton demoteBtn;
        ApplicationBarIconButton shareBtn;
        ApplicationBarIconButton commentBtn;
        ApplicationBarMenuItem reportItem;
        ApplicationBarMenuItem deleteItem;
        string currentPage;


        public BlahDetails()
        {
            currentPage = "summary";
            InitializeComponent();
            this.DataContext = App.BlahguaAPI;
            this.ApplicationBar = new ApplicationBar();
            SetBlahBackground();
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.IsVisible = true;
            ApplicationBar.Opacity = .8;

            promoteBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/black_promote.png", UriKind.Relative));
            promoteBtn.Text = "promote";
            promoteBtn.Click += HandlePromoteBlah;

            demoteBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/black_demote.png", UriKind.Relative));
            demoteBtn.Text = "demote";
            demoteBtn.Click += HandleDemoteBlah;


            shareBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/share.png", UriKind.Relative));
            shareBtn.Text = "share";
            shareBtn.Click += HandleShareBlah;


            commentBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/comment.png", UriKind.Relative));
            commentBtn.Text = "comment";
            commentBtn.Click += HandleAddComment;

            signInBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/signin.png", UriKind.Relative));
            signInBtn.Text = "sign in";
            signInBtn.Click += HandleSignin;

            reportItem = new ApplicationBarMenuItem("flag post as inappropriate");
            reportItem.Click += HandleReportItem;

            deleteItem = new ApplicationBarMenuItem("remove post");
            deleteItem.Click += HandleDeleteItem;



            App.BlahguaAPI.LoadBlahComments((allComments) =>
                {
                    AllCommentList.ItemsSource = allComments;
                }
            );
        }

        void UpdateSummaryButtons()
        {
            promoteBtn.IconUri = new Uri("/Images/Icons/black_promote.png");
            demoteBtn.IconUri = new Uri("/Images/Icons/black_demote.png");
            Blah curBlah = App.BlahguaAPI.CurrentBlah;

            if (App.BlahguaAPI.CurrentUser != null)
            {
                if (curBlah.A == App.BlahguaAPI.CurrentUser._id)
                {
                    promoteBtn.IsEnabled = false;
                    demoteBtn.IsEnabled = false;
                }
                else if (curBlah.uv == 0)
                {
                    promoteBtn.IsEnabled = true;
                    demoteBtn.IsEnabled = true;
                }
                else
                {
                    promoteBtn.IsEnabled = false;
                    demoteBtn.IsEnabled = false;
                    if (curBlah.uv == 1)
                    {
                        promoteBtn.IconUri = new Uri("/Images/Icons/promote.png"); 
                    }
                    else
                    {
                        demoteBtn.IconUri = new Uri("/Images/Icons/demote.png"); 
                    }
                }
            }
        }

        void UpdateCommentButtons()
        {
            promoteBtn.IconUri = new Uri("/Images/Icons/black_promote.png");
            demoteBtn.IconUri = new Uri("/Images/Icons/black_demote.png");
            Blah curBlah = App.BlahguaAPI.CurrentBlah;

            if (App.BlahguaAPI.CurrentUser == null)
            {
                promoteBtn.IsEnabled = false;
                demoteBtn.IsEnabled = false;
                commentBtn.IsEnabled = false;


            }
            else
            {
                commentBtn.IsEnabled = true;
                Comment curComment = (Comment)AllCommentList.SelectedItem;

                if (curComment != null)
                {
                    // see if the user voted on it already
                    if (curComment.uv == 0)
                    {
                        promoteBtn.IsEnabled = true;
                        demoteBtn.IsEnabled = true;
                    }
                    else
                    {
                        promoteBtn.IsEnabled = false;
                        demoteBtn.IsEnabled = false;
                        if (curComment.uv == 1)
                        {
                            promoteBtn.IconUri = new Uri("/Images/Icons/promote.png");
                        }
                        else
                        {
                            demoteBtn.IconUri = new Uri("/Images/Icons/demote.png");
                        }
                    }
                }
                else
                {
                    promoteBtn.IsEnabled = false;
                    demoteBtn.IsEnabled = false;
                }
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // update the text
            
        }

        void SetBlahBackground()
        {
            Brush newBrush;


            switch (App.BlahguaAPI.CurrentBlah.TypeName)
            {
                case "leaks":
                    newBrush = (Brush)App.Current.Resources["BaseBrushLeaks"];
                    break;
                case "polls":
                    newBrush = (Brush)App.Current.Resources["BaseBrushPolls"]; ;
                    break;
                case "asks":
                    newBrush = (Brush)App.Current.Resources["BaseBrushAsks"]; ;
                    break;
                case "predicts":
                    newBrush = (Brush)App.Current.Resources["BaseBrushPredicts"]; ;
                    break;
                default:
                    newBrush = (Brush)App.Current.Resources["BaseBrushSays"]; ;
                    break;
            }

            BackgroundScreen.Fill = newBrush;

        }

        private void HandlePromoteBlah(object target, EventArgs theArgs)
        {
            if (currentPage == "summary")
            {
                App.BlahguaAPI.SetBlahVote(1, (newVote) =>
                    {
                        UpdateSummaryButtons();
                    });
            }
            else
            {
                Comment curComment = (Comment)AllCommentList.SelectedItem;

                if (curComment != null)
                {
                    App.BlahguaAPI.SetCommentVote(curComment, 1, (newVote) =>
                        {
                            UpdateCommentButtons();
                        }
                    );
                }
            }
        }

        private void HandleSignin(object target, EventArgs theArgs)
        {
            NavigationService.Navigate(new Uri("/Pages/Signin.xaml", UriKind.Relative));    
        }

        private void HandleDemoteBlah(object target, EventArgs theArgs)
        {
            if (currentPage == "summary")
            {
                App.BlahguaAPI.SetBlahVote(1, (newVote) =>
                {
                    UpdateSummaryButtons();
                });
            }
            else
            {
                Comment curComment = (Comment)AllCommentList.SelectedItem;

                if (curComment != null)
                {
                    App.BlahguaAPI.SetCommentVote(curComment, -1, (newVote) =>
                    {
                        UpdateCommentButtons();
                    }
                    );
                }
            }
        }

        private void HandleShareBlah(object target, EventArgs theArgs)
        {

        }

        private void HandleAddComment(object target, EventArgs theArgs)
        {
            NavigationService.Navigate(new Uri("/Pages/CreateCommemt.xaml", UriKind.Relative));  
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
            currentPage = e.Item.Header.ToString();
            if (App.BlahguaAPI.CurrentUser != null)
            {
                switch (currentPage)
                {
                    case "summary":
                        ApplicationBar.Buttons.Add(promoteBtn);
                        ApplicationBar.Buttons.Add(demoteBtn);
                        ApplicationBar.Buttons.Add(commentBtn);
                        ApplicationBar.Buttons.Add(shareBtn);

                        ApplicationBar.MenuItems.Add(reportItem);
                        ApplicationBar.MenuItems.Add(deleteItem);
                        ApplicationBar.IsVisible = true;
                        UpdateSummaryButtons();
                        break;

                    case "comments":
                        ApplicationBar.Buttons.Add(promoteBtn);
                        ApplicationBar.Buttons.Add(demoteBtn);
                        ApplicationBar.Buttons.Add(commentBtn);
                        ApplicationBar.MenuItems.Add(reportItem);
                        ApplicationBar.MenuItems.Add(deleteItem);
                        ApplicationBar.IsVisible = true;
                        UpdateCommentButtons();
                        break;

                    case "stats":
                        ApplicationBar.IsVisible = false;
                        break;

                    default:


                        break;
                }
            }
            else
            {
                ApplicationBar.Buttons.Add(signInBtn);
                ApplicationBar.IsVisible = true;
            }
        }

        private void HandlePivotUnloaded(object sender, PivotItemEventArgs e)
        {
            ApplicationBar.Buttons.Clear();
            ApplicationBar.MenuItems.Clear();
        }

        

    }
}