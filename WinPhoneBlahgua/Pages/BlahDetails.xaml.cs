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
        bool commentsLoaded = false;
        CommentList blahComments = null;
        bool statsLoaded = false;


        public BlahDetails()
        {
            commentsLoaded = false;
            blahComments = null;
            statsLoaded = false;
            currentPage = "summary";
            InitializeComponent();
            this.DataContext = App.BlahguaAPI;
            this.ApplicationBar = new ApplicationBar();
            SetBlahBackground();
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.IsVisible = true;
            ApplicationBar.Opacity = .8;

            promoteBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/white_promote.png", UriKind.Relative));
            promoteBtn.Text = "promote";
            promoteBtn.Click += HandlePromoteBlah;

            demoteBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/white_demote.png", UriKind.Relative));
            demoteBtn.Text = "demote";
            demoteBtn.Click += HandleDemoteBlah;


            shareBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/share.png", UriKind.Relative));
            shareBtn.Text = "share";
            shareBtn.Click += HandleShareBlah;


            commentBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/white_comment.png", UriKind.Relative));
            commentBtn.Text = "comment";
            commentBtn.Click += HandleAddComment;

            signInBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/signin.png", UriKind.Relative));
            signInBtn.Text = "sign in";
            signInBtn.Click += HandleSignin;

            reportItem = new ApplicationBarMenuItem("flag post as inappropriate");
            reportItem.Click += HandleReportItem;

            deleteItem = new ApplicationBarMenuItem("remove post");
            deleteItem.Click += HandleDeleteItem;

            StatsArea.DataContext = null;
        }


        void UpdateSummaryButtons()
        {
            promoteBtn.IconUri = new Uri("/Images/Icons/white_promote.png", UriKind.Relative);
            demoteBtn.IconUri = new Uri("/Images/Icons/white_demote.png", UriKind.Relative);
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
                        promoteBtn.IconUri = new Uri("/Images/Icons/promote_active.png", UriKind.Relative); 
                    }
                    else
                    {
                        demoteBtn.IconUri = new Uri("/Images/Icons/demote_active.png", UriKind.Relative); 
                    }
                }
            }
        }

        void UpdateStatsPage()
        {
            if (App.BlahguaAPI.CurrentUser == null)
            {
                DemoCharts.Visibility = Visibility.Collapsed;
                SignInStatPromp.Visibility = Visibility.Visible;
            }
            else
            {
                DemoCharts.Visibility = Visibility.Visible;
                SignInStatPromp.Visibility = Visibility.Collapsed;
            }
        }

        void UpdateCommentButtons()
        {
            promoteBtn.IconUri = new Uri("/Images/Icons/white_promote.png", UriKind.Relative);
            demoteBtn.IconUri = new Uri("/Images/Icons/white_demote.png", UriKind.Relative);
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
                            promoteBtn.IconUri = new Uri("/Images/Icons/promote_active.png", UriKind.Relative);
                        }
                        else
                        {
                            demoteBtn.IconUri = new Uri("/Images/Icons/demote_active.png", UriKind.Relative);
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
            if (currentPage == "summary")
                UpdateSummaryButtons();
            else if (currentPage == "comments")
                UpdateCommentButtons();
            else if (currentPage == "stats")
                UpdateStatsPage();
            
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
                    newBrush = (Brush)App.Current.Resources["BaseBrushPolls"]; 
                    break;
                case "asks":
                    newBrush = (Brush)App.Current.Resources["BaseBrushAsks"]; 
                    break;
                case "predicts":
                    newBrush = (Brush)App.Current.Resources["BaseBrushPredicts"]; 
                    break;
                default:
                    newBrush = (Brush)App.Current.Resources["BaseBrushSays"]; 
                    break;
            }

            BackgroundScreen.Fill = newBrush;
        }

        private void HandlePollInit()
        {
            App.BlahguaAPI.GetUserPollVote((theVote) =>
                {
                    ((Storyboard)Resources["ShowPollAnimation"]).Begin();
                }
            );
        }

        private void HandlePredictInit()
        {
            App.BlahguaAPI.GetUserPredictionVote((theVote) =>
                {
                    Blah curBlah = App.BlahguaAPI.CurrentBlah;
                    if (curBlah.E > DateTime.Now)
                    {
                        // still has time
                        PredictDateBox.Text = "happening by " + curBlah.E.ToShortDateString();
                        PredictElapsedTimeBox.Text = "(" + Utilities.ElapsedDateString(curBlah.E) + ")";
                        WillHappenItems.Visibility = Visibility.Visible;
                        AllreadyHappenedItems.Visibility = Visibility.Collapsed;
                        WillHappenItems.ItemsSource = curBlah.PredictionItems;
                    }
                    else
                    {
                        // expired
                        PredictDateBox.Text = "should have happened on " + curBlah.E.ToShortDateString();
                        PredictElapsedTimeBox.Text = "(" + Utilities.ElapsedDateString(curBlah.E) + ")";
                        WillHappenItems.Visibility = Visibility.Visible;
                        AllreadyHappenedItems.Visibility = Visibility.Collapsed;
                        AllreadyHappenedItems.ItemsSource = curBlah.ExpPredictionItems;
                    }

                    ((Storyboard)Resources["ShowPredictionAnimation"]).Begin();
                }
            );
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
            if (App.BlahguaAPI.CreateCommentRecord == null)
                App.BlahguaAPI.CreateCommentRecord = new CommentCreateRecord();

            if (currentPage == "summary")
            {
                App.BlahguaAPI.CreateCommentRecord.CID = null;
                NavigationService.Navigate(new Uri("/Pages/CreateComment.xaml", UriKind.Relative));
            }
            else
            {
                // on the comment page
                Comment curComment = (Comment)AllCommentList.SelectedItem;
                if (curComment != null)
                    App.BlahguaAPI.CreateCommentRecord.CID = curComment._id;
                else
                    App.BlahguaAPI.CreateCommentRecord.CID = null;

                NavigationService.Navigate(new Uri("/Pages/CreateComment.xaml", UriKind.Relative));
            }
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

        private void LoadComments()
        {
            App.BlahguaAPI.LoadBlahComments((theList) =>
                {
                    commentsLoaded = true;
                    blahComments = theList;
                    NoCommentBox.Visibility = Visibility.Collapsed;
                    AllCommentList.ItemsSource = blahComments;
                });
            
        }

        private void LoadStats()
        {
            App.BlahguaAPI.LoadBlahStats((theStats) =>
            {
                statsLoaded = true;
                NoStatsBox.Visibility = Visibility.Collapsed;
                StatsArea.Visibility = Visibility.Visible;
                StatsArea.DataContext = App.BlahguaAPI.CurrentBlah;
                DrawBlahStats();
            });

        }

        private void DrawBlahStats()
        {
            



        }

        private void OnPivotLoading(object sender, PivotItemEventArgs e)
        {
            string newItem = e.Item.Header.ToString();

            if (newItem == "comments")
            {
                if (App.BlahguaAPI.CurrentBlah.C == 0)
                {
                    NoCommentBox.Visibility = Visibility.Visible;
                    NoCommentProgress.Visibility = Visibility.Collapsed;
                    NoCommentTextBox.Text = "This post has no comments.\nMaybe you can add the first!";
                }
                else
                {
                    if (commentsLoaded)
                        NoCommentBox.Visibility = Visibility.Collapsed;
                    else
                    {
                        NoCommentBox.Visibility = Visibility.Visible;
                        NoCommentTextBox.Text = "loading comments";
                        NoCommentProgress.Visibility = Visibility.Visible;
                        LoadComments();
                    }
                }
            }
            else if (newItem == "stats")
            {
                if (!statsLoaded)
                {
                    NoStatsBox.Visibility = Visibility.Visible;
                    LoadStats();

                }
            }
            
            
            if ((newItem == "summary") && (currentPage == "stats"))
            {
                // wrap around
                BackgroundImage2.Visibility = Visibility.Visible;

                Storyboard sb = new Storyboard();
                DoubleAnimation db1 = new DoubleAnimation();
                DoubleAnimation db2 = new DoubleAnimation();
                ExponentialEase ease = new ExponentialEase();
                ease.Exponent = 5;
                ease.EasingMode = EasingMode.EaseIn;

                db1.EasingFunction = ease;
                db1.BeginTime = TimeSpan.FromSeconds(0);
                db1.Duration = TimeSpan.FromSeconds(.5);
                db1.To = -800;
                Storyboard.SetTarget(db1, BackgroundImage);
                Storyboard.SetTargetProperty(db1, new PropertyPath("(Canvas.Left)"));
                sb.Children.Add(db1);

                db2.EasingFunction = ease;
                db2.BeginTime = TimeSpan.FromSeconds(0);
                db2.Duration = TimeSpan.FromSeconds(.5);
                db2.To = 0;
                Storyboard.SetTarget(db2, BackgroundImage2);
                Storyboard.SetTargetProperty(db2, new PropertyPath("(Canvas.Left)"));
                sb.Children.Add(db2);

                sb.Completed += sbWrap_Completed;
                sb.Begin();
            }
            else if ((newItem == "stats") && (currentPage == "summary"))
            {
                // back up
                // wrap around
                BackgroundImage2.Visibility = Visibility.Visible;

                Storyboard sb = new Storyboard();
                DoubleAnimation db1 = new DoubleAnimation();
                DoubleAnimation db2 = new DoubleAnimation();
                ExponentialEase ease = new ExponentialEase();
                ease.Exponent = 5;
                ease.EasingMode = EasingMode.EaseIn;

                db1.EasingFunction = ease;
                db1.BeginTime = TimeSpan.FromSeconds(0);
                db1.Duration = TimeSpan.FromSeconds(.5);
                db1.From = -800;
                db1.To = -320;
                Storyboard.SetTarget(db1, BackgroundImage);
                Storyboard.SetTargetProperty(db1, new PropertyPath("(Canvas.Left)"));
                sb.Children.Add(db1);

                db2.EasingFunction = ease;
                db2.BeginTime = TimeSpan.FromSeconds(0);
                db2.Duration = TimeSpan.FromSeconds(.5);
                db2.From = 0;
                db2.To = 480;
                Storyboard.SetTarget(db2, BackgroundImage2);
                Storyboard.SetTargetProperty(db2, new PropertyPath("(Canvas.Left)"));
                sb.Children.Add(db2);

                sb.Completed += sbBackWrap_Completed;
                sb.Begin();

            }
            else
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
        }

        void sbWrap_Completed(object sender, EventArgs e)
        {
            BackgroundImage2.Visibility = Visibility.Collapsed;
            Canvas.SetLeft(BackgroundImage2, 480);
            Canvas.SetLeft(BackgroundImage, 0);
        }

        void sbBackWrap_Completed(object sender, EventArgs e)
        {
            BackgroundImage2.Visibility = Visibility.Collapsed;
            Canvas.SetLeft(BackgroundImage2, 480);
            Canvas.SetLeft(BackgroundImage, -320);
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
                        UpdateStatsPage();
                        break;

                    default:


                        break;
                }
            }
            else
            {
                ApplicationBar.Buttons.Add(signInBtn);
                ApplicationBar.IsVisible = true;
                if (currentPage == "stats")
                    UpdateStatsPage();
            }
        }

        private void HandlePivotUnloaded(object sender, PivotItemEventArgs e)
        {
            ApplicationBar.Buttons.Clear();
            ApplicationBar.MenuItems.Clear();
        }

        private void NavigationInTransition_EndTransition(object sender, RoutedEventArgs e)
        {
            switch (App.BlahguaAPI.CurrentBlah.TypeName)
            {
                case "polls":
                    HandlePollInit();
                    break;
                case "predicts":
                    HandlePredictInit();
                    break;
            }  
        }

        

    }
}