using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;
using Telerik.Charting;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace WinPhoneBlahgua
{
    public partial class ProfileViewer : PhoneApplicationPage
    {
        ApplicationBarIconButton deletePostBtn;
        ApplicationBarIconButton deleteCommentBtn;
        ApplicationBarIconButton addBadgeBtn;
        ApplicationBarIconButton signOutBtn;
        private bool commentsLoaded = false;
        private bool postsLoaded = false;
        private bool statsLoaded = false;

        public ProfileViewer()
        {
            InitializeComponent();
            
            this.DataContext = App.BlahguaAPI;
            commentsLoaded = false;
            postsLoaded = false;
            statsLoaded = false;

            this.ApplicationBar = new ApplicationBar();
            this.ApplicationBar.IsVisible = false;
            ApplicationBar.IsMenuEnabled = false;
            ApplicationBar.Opacity = .8;

            deletePostBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/appbar.delete.rest.png", UriKind.Relative));
            deletePostBtn.Text = "delete";
            deletePostBtn.Click += HandleDeleteBlah;

            deleteCommentBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/appbar.delete.rest.png", UriKind.Relative));
            deleteCommentBtn.Text = "delete";
            deleteCommentBtn.Click += HandleDeleteComment;


            addBadgeBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/appbar.add.rest.png", UriKind.Relative));
            addBadgeBtn.Text = "get badged";
            addBadgeBtn.Click += HandleAddBadge;

            signOutBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/appbar.add.rest.png", UriKind.Relative));
            signOutBtn.Text = "sign out";
            signOutBtn.Click += HandleSignOut;

            UserHeader.BadgeListArea.Visibility = Visibility.Collapsed;

        }

    

        private void HandleDeleteBlah(object target, EventArgs theArgs)
        {
            if (MessageBox.Show("Are you sure you want to delete this post?", "confirm delete", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {

            }
        }

        private void HandleAddBadge(object target, EventArgs theArgs)
        {
            NavigationService.Navigate(new Uri("/Pages/BadgingPage.xaml", UriKind.Relative));    
        }

        private void HandleSignOut(object target, EventArgs theArgs)
        {
            App.BlahguaAPI.SignOut((theStr) =>
                {
                    NavigationService.GoBack();
                }
            );
           
        }



        private void HandleDeleteComment(object target, EventArgs theArgs)
        {
            if (MessageBox.Show("Are you sure you want to delete this comment?", "confirm delete", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {

            }    
        }

        private void UpdateBlahHistoryButtons()
        {
            deletePostBtn.IsEnabled =  (UserPostList.SelectedItem != null);

        }

        private void UpdateCommentHistoryButtons()
        {
            deleteCommentBtn.IsEnabled = (UserCommentList.SelectedItem != null);
        }

        private void HandlePivotUnloaded(object sender, PivotItemEventArgs e)
        {
            ApplicationBar.Buttons.Clear();
            ApplicationBar.MenuItems.Clear();
        }


        private void OnPivotLoading(object sender, PivotItemEventArgs e)
        {
            string newItem = e.Item.Header.ToString();

            if (newItem == "posts")
            {

                if (postsLoaded)
                    NoPostsBox.Visibility = Visibility.Collapsed;
                else
                {
                    NoPostsBox.Visibility = Visibility.Visible;
                    NoPostTextBlock.Text = "loading posts";
                    NoPostProgress.Visibility = Visibility.Visible;
                    LoadUserPosts();
                }
                
            }
            else if (newItem == "comments")
            {

                if (commentsLoaded)
                    NoCommentsBox.Visibility = Visibility.Collapsed;
                else
                {
                    NoCommentsBox.Visibility = Visibility.Visible;
                    NoCommentTextBlock.Text = "loading comments";
                    NoCommentProgress.Visibility = Visibility.Visible;
                    LoadUserComments();
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
        }

        private void LoadUserComments()
        {
            App.BlahguaAPI.LoadUserComments((theComments) =>
                {
                    commentsLoaded = true;
                    UserCommentList.ItemsSource = theComments;
                    NoCommentsBox.Visibility = Visibility.Collapsed;
                }
            );

        }

        private void LoadUserPosts()
        {
            App.BlahguaAPI.LoadUserPosts((theBlahs) =>
                {
                    postsLoaded = true;
                    UserPostList.ItemsSource = theBlahs;
                    NoPostsBox.Visibility = Visibility.Collapsed;
                }
        );

        }

        private void LoadStats()
        {
            App.BlahguaAPI.LoadUserStatsInfo((theStats) =>
            {
                statsLoaded = true;
                NoStatsBox.Visibility = Visibility.Collapsed;
                StatsArea.Visibility = Visibility.Visible;
                StatsArea.DataContext = App.BlahguaAPI.CurrentBlah;
                DrawUserStats();
            });

        }

        private void DrawUserStats()
        {
            User curUser = App.BlahguaAPI.CurrentUser;
            UserInfoObject userInfo = curUser.UserInfo;

            // blahgua score

            CategoricalSeries newSeries;
            newSeries = new BarSeries();
            CategoricalDataPoint newPoint = new CategoricalDataPoint();
            newPoint.Value = curUser.S;
            newPoint.Category = "score";
            newSeries.DataPoints.Add(newPoint);
            newPoint = new CategoricalDataPoint();
            newPoint.Value = curUser.K;
            newPoint.Category = "controversy";
            newSeries.DataPoints.Add(newPoint);
            StandingChart.Series.Add(newSeries);


            // votes

            // views
            bool hasVotes = false;

            newSeries = new SplineAreaSeries();
            for (int i = 0; i < userInfo.DayCount; i++)
            {
                newPoint = new CategoricalDataPoint();
                newPoint.Value = userInfo.UserViews(i);
                if (newPoint.Value > 0)
                    hasVotes = true;
                newPoint.Category = userInfo.StatDate(i);
                newSeries.DataPoints.Add(newPoint);
            }
            
            if (hasVotes)
                UserViewChart.Series.Add(newSeries);
            

            // opens
            hasVotes = false;
            newSeries = new SplineAreaSeries();
            for (int i = 0; i < userInfo.DayCount; i++)
            {
                newPoint = new CategoricalDataPoint();
                newPoint.Value = userInfo.UserOpens(i);
                if (newPoint.Value > 0)
                    hasVotes = true;
                newPoint.Category = userInfo.StatDate(i);
                newSeries.DataPoints.Add(newPoint);
            }
            if (hasVotes)
                UserOpenChart.Series.Add(newSeries);
            

            // created
            hasVotes = false;
            newSeries = new SplineAreaSeries();
            for (int i = 0; i < userInfo.DayCount; i++)
            {
                newPoint = new CategoricalDataPoint();
                newPoint.Value = userInfo.UserCreates(i);
                if (newPoint.Value > 0)
                    hasVotes = true;
                newPoint.Category = userInfo.StatDate(i);
                newSeries.DataPoints.Add(newPoint);
            }
            if (hasVotes)
                PostsCreatedChart.Series.Add(newSeries);
            


            // comments
            hasVotes = false;
            newSeries = new SplineAreaSeries();
            for (int i = 0; i < userInfo.DayCount; i++)
            {
                newPoint = new CategoricalDataPoint();
                newPoint.Value = userInfo.UserComments(i);
                if (newPoint.Value > 0)
                    hasVotes = true;
                newPoint.Category = userInfo.StatDate(i);
                newSeries.DataPoints.Add(newPoint);
            }
            if (hasVotes)
                CommentsCreatedChart.Series.Add(newSeries);
            

            // audience stats
            hasVotes = false;

            if (hasVotes)
            { }


            // views
            hasVotes = false;
            newSeries = new SplineAreaSeries();
            for (int i = 0; i < userInfo.DayCount; i++)
            {
                newPoint = new CategoricalDataPoint();
                newPoint.Value = userInfo.Views(i);
                if (newPoint.Value > 0)
                    hasVotes = true;
                newPoint.Category = userInfo.StatDate(i);
                newSeries.DataPoints.Add(newPoint);
            }
            if (hasVotes)
                ViewChart.Series.Add(newSeries);
            

            // opens
            hasVotes = false;
            newSeries = new SplineAreaSeries();
            for (int i = 0; i < userInfo.DayCount; i++)
            {
                newPoint = new CategoricalDataPoint();
                newPoint.Value = userInfo.Opens(i);
                if (newPoint.Value > 0)
                    hasVotes = true;
                newPoint.Category = userInfo.StatDate(i);
                newSeries.DataPoints.Add(newPoint);
            }
            if (hasVotes)
                OpenChart.Series.Add(newSeries);
            


            // comments
            hasVotes = false;
            newSeries = new SplineAreaSeries();
            for (int i = 0; i < userInfo.DayCount; i++)
            {
                newPoint = new CategoricalDataPoint();
                newPoint.Value = userInfo.Comments(i);
                if (newPoint.Value > 0)
                    hasVotes = true;
                newPoint.Category = userInfo.StatDate(i);
                newSeries.DataPoints.Add(newPoint);
            }
            if (hasVotes)
                CommentChart.Series.Add(newSeries);
            

            // gender
            CreateUserDemoChart(GenderChart, "B");

            // age
            CreateUserDemoChart(AgeChart, "C");


            // race
            CreateUserDemoChart(RaceChart, "D");


            // income
            CreateUserDemoChart(IncomeChart, "E");

            // country
            CreateUserDemoChart(CountryChart, "J");

        }

        private void CreateUserDemoChart(RadCartesianChart theChart, string demoProp)
        {
            Dictionary<string, string> curDict = App.BlahguaAPI.UserProfileSchema.GetTypesForProperty(demoProp);

            CategoricalDataPoint promotePoint, demotePoint;
            CategoricalSeries promoteSeries = new BarSeries();
            CategoricalSeries demoteSeries = new BarSeries();
            UserInfoObject curInfo = App.BlahguaAPI.CurrentUser.UserInfo;
            DemoProfileSummaryRecord upVotes = curInfo._d._u;
            DemoProfileSummaryRecord downVotes = curInfo._d._d;
            promoteSeries.CombineMode = ChartSeriesCombineMode.Stack;
            demoteSeries.CombineMode = ChartSeriesCombineMode.Stack;

            int maxVal = 0;
            foreach (string curVal in curDict.Keys)
            {
                promotePoint = new CategoricalDataPoint();
                promotePoint.Category = curDict[curVal];
                promotePoint.Value = upVotes.GetPropertyValue(demoProp, curVal);// curBlah._d._u.B.GetValue(curVal);
                promoteSeries.DataPoints.Add(promotePoint);


                demotePoint = new CategoricalDataPoint();
                demotePoint.Category = curDict[curVal];
                demotePoint.Value = downVotes.GetPropertyValue(demoProp, curVal);
                demoteSeries.DataPoints.Add(demotePoint);

                if ((promotePoint.Value + demotePoint.Value) > maxVal)
                    maxVal = (int)(promotePoint.Value + demotePoint.Value);
            }
            if (maxVal > 0)
            {
                maxVal += 2;
                ((LinearAxis)theChart.VerticalAxis).Maximum = maxVal;
                theChart.Series.Add(promoteSeries);
                theChart.Series.Add(demoteSeries);
            }
        }

        private void HandlePivotLoaded(object sender, PivotItemEventArgs e)
        {
            switch (e.Item.Header.ToString())
            {
                case "persona":
                    ApplicationBar.Buttons.Add(signOutBtn);
                    ApplicationBar.IsVisible = true;     
                    
                    break;

                case "badges":
                    ApplicationBar.Buttons.Add(addBadgeBtn);
                    ApplicationBar.IsVisible = true;
                    UpdateBadgeArea();
                    break;

                case "demographics":
                    UpdateDemographics();
                    break;

                case "posts":
                    ApplicationBar.Buttons.Add(deletePostBtn);
                    ApplicationBar.IsVisible = true;
                    UpdateBlahHistoryButtons();
                    break;

                case "comments":
                    ApplicationBar.Buttons.Add(deleteCommentBtn);
                    ApplicationBar.IsVisible = true;
                    UpdateCommentHistoryButtons();
                    break;

                case "stats":
                    ApplicationBar.IsVisible = false;        
                    break;

                case "account":
                    ApplicationBar.IsVisible = false;        
                    break;

                default:
                    ApplicationBar.IsVisible = false;     
                    break;
            }
        }

        private void UpdateDemographics()
        {
            App.BlahguaAPI.GetUserProfile((theProfile) =>
                {
                    // update everything by hand
                    GenderList.SelectedItem = "Female";
                    



                }
            );
        }

        private void UpdateBadgeArea()
        {
            if (App.BlahguaAPI.CurrentUser.B != null)
            {
                UserBadgeList.Visibility = Visibility.Visible;
                NoBadgeList.Visibility = Visibility.Collapsed;
            }
            else
            {
                UserBadgeList.Visibility = Visibility.Collapsed;
                NoBadgeList.Visibility = Visibility.Visible;
            }
        }


       
    }
}
