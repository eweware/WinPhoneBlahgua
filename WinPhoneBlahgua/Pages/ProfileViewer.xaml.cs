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
using System.Windows.Media.Animation;
using Microsoft.Phone.Tasks;
using System.Reflection;
using System.Windows.Data;
using System.ComponentModel;


namespace WinPhoneBlahgua
{
    public class SortMenu : ApplicationBarMenuItem
    {
        public SortMenu(string title, string sortKey)
            : base(title)
        {
            SortKey = sortKey;
        }

        public string SortKey { get; set; }
    }

    public partial class ProfileViewer : PhoneApplicationPage
    {
        

        ApplicationBarIconButton deletePostBtn;
        ApplicationBarIconButton openPostBtn;
        ApplicationBarIconButton openCommentBtn;
        ApplicationBarIconButton deleteCommentBtn;
        ApplicationBarIconButton addBadgeBtn;
        ApplicationBarIconButton signOutBtn;

        SortMenu sortByDateAsc;
        SortMenu sortByDateDesc;
        SortMenu sortByUpVote;
        SortMenu sortByDownVote;
        SortMenu sortByStrength;

        private bool commentsLoaded = false;
        private bool postsLoaded = false;
        private bool statsLoaded = false;
        private bool isDemoPopulated = false;
        private string currentPage;
        private string curCommentSort = "byDateDesc";
        private string curBlahSort = "byDateDesc";
        private CollectionViewSource commentDataView;
        private CollectionViewSource blahDataView;

        private Comment curComment = null;
        private Blah curBlah = null;

        public ProfileViewer()
        {
            InitializeComponent();
            
            this.DataContext = App.BlahguaAPI;
            commentsLoaded = false;
            postsLoaded = false;
            statsLoaded = false;
            currentPage = "persona";

            this.ApplicationBar = new ApplicationBar();
            this.ApplicationBar.IsVisible = false;
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.Opacity = .8;

            deletePostBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/appbar.delete.rest.png", UriKind.Relative));
            deletePostBtn.Text = "delete";
            deletePostBtn.Click += HandleDeleteBlah;

            deleteCommentBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/appbar.delete.rest.png", UriKind.Relative));
            deleteCommentBtn.Text = "delete";
            deleteCommentBtn.Click += HandleDeleteComment;

            openPostBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/open.png", UriKind.Relative));
            openPostBtn.Text = "open";
            openPostBtn.Click += HandleOpenPost;

            openCommentBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/open.png", UriKind.Relative));
            openCommentBtn.Text = "open";
            openCommentBtn.Click += HandleOpenComment;


            addBadgeBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/appbar.add.rest.png", UriKind.Relative));
            addBadgeBtn.Text = "get badged";
            addBadgeBtn.Click += HandleAddBadge;

            signOutBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/signout.png", UriKind.Relative));
            signOutBtn.Text = "sign out";
            signOutBtn.Click += HandleSignOut;

            sortByDateAsc = new SortMenu("oldest first", "byDateAsc");
            sortByDateAsc.Click += sortMenuItem_Click;

            sortByDateDesc = new SortMenu("newest first", "byDateDesc");
            sortByDateDesc.Click += sortMenuItem_Click;

            sortByUpVote = new SortMenu("most promoted first", "byPositive");
            sortByUpVote.Click += sortMenuItem_Click;

            sortByDownVote = new SortMenu("most demoted first", "byNegative");
            sortByDownVote.Click += sortMenuItem_Click;

            sortByStrength = new SortMenu("most popular first", "byPopular");
            sortByStrength.Click += sortMenuItem_Click;

            UserHeader.BadgeListArea.Visibility = Visibility.Collapsed;


        }

        void sortMenuItem_Click(object sender, EventArgs e)
        {
            string newSort = ((SortMenu)sender).SortKey;

            if (currentPage == "posts")
            {
                if (curBlahSort != newSort)
                {
                    curBlahSort = newSort;
                    SortAndFilterBlahs();
                }
            }
            else if (currentPage == "comments")
            {
                if (curCommentSort != newSort)
                {
                    curCommentSort = newSort;
                    SortAndFilterComments();
                }
            }
        }

        

    

        private void HandleDeleteBlah(object target, EventArgs theArgs)
        {
            if (MessageBox.Show("Are you sure you want to delete this post?", "confirm delete", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                App.BlahguaAPI.DeleteBlah(curBlah._id, (theStr) =>
                    {
                        LoadUserPosts();   
                    }
                );
            }
        }

        private void HandleOpenPost(object target, EventArgs theArgs)
        {
            if (curBlah != null)
            {
                App.BlahguaAPI.SetCurrentBlahFromId(curBlah._id, OpenFullBlah);
            }
        }

        private void HandleOpenComment(object target, EventArgs theArgs)
        {
            if (curComment != null)
            {
                App.BlahguaAPI.SetCurrentBlahFromId(curComment.B, OpenFullBlah);
            }    
        }

        void OpenFullBlah(Blah theBlah)
        {
            if (theBlah != null)
            {
                NavigationService.Navigate(new Uri("/Pages/BlahDetails.xaml", UriKind.Relative));
            }
            else
            {
               // MessageBox.Show("Blah failed to load");
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

        

        private void LoadUserComments()
        {
            App.BlahguaAPI.LoadUserComments((theComments) =>
                {
                    commentDataView = new CollectionViewSource();
                    commentDataView.Source = theComments;
                    SortAndFilterComments();
                    commentsLoaded = true;
                    UserCommentList.ItemsSource = commentDataView.View;
                    NoCommentsBox.Visibility = Visibility.Collapsed;

                }
            );

        }

        private void SortAndFilterComments()
        {
            commentDataView.SortDescriptions.Clear();

            switch (curCommentSort)
            {
                case "byDateAsc":
                    commentDataView.SortDescriptions.Add(new SortDescription("c", ListSortDirection.Ascending));
                    break;
                case "byDateDesc":
                    commentDataView.SortDescriptions.Add(new SortDescription("c", ListSortDirection.Descending));
                    break;
                case "byPositive":
                    commentDataView.SortDescriptions.Add(new SortDescription("U", ListSortDirection.Descending));
                    break;
                case "byNegative":
                    commentDataView.SortDescriptions.Add(new SortDescription("D", ListSortDirection.Descending));
                    break;
                case "byPopular":
                    commentDataView.SortDescriptions.Add(new SortDescription("S", ListSortDirection.Descending));
                    break;
            }
        }


        private void SortAndFilterBlahs()
        {
            blahDataView.SortDescriptions.Clear();

            switch (curBlahSort)
            {
                case "byDateAsc":
                    blahDataView.SortDescriptions.Add(new SortDescription("c", ListSortDirection.Ascending));
                    break;
                case "byDateDesc":
                    blahDataView.SortDescriptions.Add(new SortDescription("c", ListSortDirection.Descending));
                    break;
                case "byPositive":
                    blahDataView.SortDescriptions.Add(new SortDescription("P", ListSortDirection.Descending));
                    break;
                case "byNegative":
                    blahDataView.SortDescriptions.Add(new SortDescription("D", ListSortDirection.Descending));
                    break;
                case "byPopular":
                    blahDataView.SortDescriptions.Add(new SortDescription("S", ListSortDirection.Descending));

                    break;
            }
            


        }
        private void LoadUserPosts()
        {
            App.BlahguaAPI.LoadUserPosts((theBlahs) =>
                {
                    blahDataView = new CollectionViewSource();
                    blahDataView.Source = theBlahs.Where(blah => blah.S > 0);
                    postsLoaded = true;
                    SortAndFilterBlahs();
                    UserPostList.ItemsSource = blahDataView.View;
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
            newPoint.Value = curUser.K;
            newPoint.Category = "controversy";
            newSeries.DataPoints.Add(newPoint);
            newPoint = new CategoricalDataPoint();
            newPoint.Value = curUser.S;
            newPoint.Category = "score";
            newSeries.DataPoints.Add(newPoint);
            StandingChart.Series.Add(newSeries);


            // votes

            // views
            bool hasVotes = false;
            int maxVal;


            maxVal = 0;
            newSeries = new SplineAreaSeries();
            for (int i = 0; i < userInfo.DayCount; i++)
            {
                newPoint = new CategoricalDataPoint();
                newPoint.Value = userInfo.UserViews(i);
                if (newPoint.Value > maxVal)
                    maxVal = (int)newPoint.Value;
                if (newPoint.Value > 0)
                    hasVotes = true;
                newPoint.Category = userInfo.StatDate(i);
                newSeries.DataPoints.Add(newPoint);
            }
            
            if (hasVotes)
            {
                UserViewChart.Series.Add(newSeries);
                maxVal += 2;
                if (maxVal < 5)
                    maxVal = 5;
                double step = (double)maxVal / 5;
                ((LinearAxis)UserViewChart.VerticalAxis).MajorStep = (int)Math.Round(step);
                ((LinearAxis)UserViewChart.VerticalAxis).Maximum = (int)maxVal;
            }
            

            // opens
            maxVal = 0;
            hasVotes = false;
            newSeries = new SplineAreaSeries();
            for (int i = 0; i < userInfo.DayCount; i++)
            {
                newPoint = new CategoricalDataPoint();
                newPoint.Value = userInfo.UserOpens(i);
                if (newPoint.Value > maxVal)
                    maxVal = (int)newPoint.Value;
                if (newPoint.Value > 0)
                    hasVotes = true;
                newPoint.Category = userInfo.StatDate(i);
                newSeries.DataPoints.Add(newPoint);
            }
            if (hasVotes)
            {
                UserOpenChart.Series.Add(newSeries);
                maxVal += 2;
                if (maxVal < 5)
                    maxVal = 5;
                double step = (double)maxVal / 5;
                ((LinearAxis)UserOpenChart.VerticalAxis).MajorStep = (int)Math.Round(step);
                ((LinearAxis)UserOpenChart.VerticalAxis).Maximum = (int)maxVal;
            }
            

            // created
            maxVal = 0;
            hasVotes = false;
            newSeries = new SplineAreaSeries();
            for (int i = 0; i < userInfo.DayCount; i++)
            {
                newPoint = new CategoricalDataPoint();
                newPoint.Value = userInfo.UserCreates(i);
                if (newPoint.Value > maxVal)
                    maxVal = (int)newPoint.Value;
                if (newPoint.Value > 0)
                    hasVotes = true;
                newPoint.Category = userInfo.StatDate(i);
                newSeries.DataPoints.Add(newPoint);
            }
            if (hasVotes)
            {
                PostsCreatedChart.Series.Add(newSeries);
                maxVal += 2;
                if (maxVal < 5)
                    maxVal = 5;
                double step = (double)maxVal / 5;
                ((LinearAxis)PostsCreatedChart.VerticalAxis).MajorStep = (int)Math.Round(step);
                ((LinearAxis)PostsCreatedChart.VerticalAxis).Maximum = (int)maxVal;
            }
            


            // comments
            maxVal = 0;
            hasVotes = false;
            newSeries = new SplineAreaSeries();
            for (int i = 0; i < userInfo.DayCount; i++)
            {
                newPoint = new CategoricalDataPoint();
                newPoint.Value = userInfo.UserComments(i);
                if (newPoint.Value > maxVal)
                    maxVal = (int)newPoint.Value;
                if (newPoint.Value > 0)
                    hasVotes = true;
                newPoint.Category = userInfo.StatDate(i);
                newSeries.DataPoints.Add(newPoint);
            }
            if (hasVotes)
            {
                CommentsCreatedChart.Series.Add(newSeries);
                maxVal += 2;
                if (maxVal < 5)
                    maxVal = 5;
                double step = (double)maxVal / 5;
                ((LinearAxis)CommentsCreatedChart.VerticalAxis).MajorStep = (int)Math.Round(step);
                ((LinearAxis)CommentsCreatedChart.VerticalAxis).Maximum = (int)maxVal;
            }
            

            // views
            maxVal = 0;
            hasVotes = false;
            newSeries = new SplineAreaSeries();
            for (int i = 0; i < userInfo.DayCount; i++)
            {
                newPoint = new CategoricalDataPoint();
                newPoint.Value = userInfo.Views(i);
                if (newPoint.Value > maxVal)
                    maxVal = (int)newPoint.Value;
                if (newPoint.Value > 0)
                    hasVotes = true;
                newPoint.Category = userInfo.StatDate(i);
                newSeries.DataPoints.Add(newPoint);
            }
            if (hasVotes)
            {
                ViewChart.Series.Add(newSeries);
                maxVal += 2;
                if (maxVal < 5)
                    maxVal = 5;
                double step = (double)maxVal / 5;
                ((LinearAxis)ViewChart.VerticalAxis).MajorStep = (int)Math.Round(step);
                ((LinearAxis)ViewChart.VerticalAxis).Maximum = (int)maxVal;
            }
            

            // opens
            maxVal = 0;
            hasVotes = false;
            newSeries = new SplineAreaSeries();
            for (int i = 0; i < userInfo.DayCount; i++)
            {
                newPoint = new CategoricalDataPoint();
                newPoint.Value = userInfo.Opens(i);
                if (newPoint.Value > maxVal)
                    maxVal = (int)newPoint.Value;
                if (newPoint.Value > 0)
                    hasVotes = true;
                newPoint.Category = userInfo.StatDate(i);
                newSeries.DataPoints.Add(newPoint);
            }
            if (hasVotes)
            {
                OpenChart.Series.Add(newSeries);
                maxVal += 2;
                if (maxVal < 5)
                    maxVal = 5;
                double step = (double)maxVal / 5;
                ((LinearAxis)OpenChart.VerticalAxis).MajorStep = (int)Math.Round(step);
                ((LinearAxis)OpenChart.VerticalAxis).Maximum = (int)maxVal;
            }
            


            // comments
            maxVal = 0;
            hasVotes = false;
            newSeries = new SplineAreaSeries();
            for (int i = 0; i < userInfo.DayCount; i++)
            {
                newPoint = new CategoricalDataPoint();
                newPoint.Value = userInfo.Comments(i);
                if (newPoint.Value > maxVal)
                    maxVal = (int)newPoint.Value;
                if (newPoint.Value > 0)
                    hasVotes = true;
                newPoint.Category = userInfo.StatDate(i);
                newSeries.DataPoints.Add(newPoint);
            }
            if (hasVotes)
            {
                CommentChart.Series.Add(newSeries);
                maxVal += 2;
                if (maxVal < 5)
                    maxVal = 5;
                double step = (double)maxVal / 5;
                ((LinearAxis)CommentChart.VerticalAxis).MajorStep = (int)Math.Round(step);
                ((LinearAxis)CommentChart.VerticalAxis).Maximum = (int)maxVal;
            }
            

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
            bool visible = false;

            if ((App.BlahguaAPI.CurrentUser != null) &&
                (App.BlahguaAPI.CurrentUser.Profile != null))
            {
                UserProfile  theProfile = App.BlahguaAPI.CurrentUser.Profile;
                PropertyInfo theProp = theProfile.GetType().GetProperty(demoProp);
                if (theProp != null)
                {
                    string theVal = (string)theProp.GetValue(theProfile, null);
                    if (theVal != "-1")
                        visible = true;
                }
            }

            if (visible)
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
                    double step = (double)maxVal / 5;
                    ((LinearAxis)theChart.VerticalAxis).MajorStep = (int)Math.Round(step);
                    theChart.Series.Add(promoteSeries);
                    theChart.Series.Add(demoteSeries);
                }
            }
            else
            {
                theChart.EmptyContent = "Set this attribute in your profile to see it for others.";
                theChart.EmptyContentTemplate = (DataTemplate)Resources["HiddenChartTemplate"];
            }
        }

        private void HandlePivotUnloaded(object sender, PivotItemEventArgs e)
        {
            ApplicationBar.Buttons.Clear();
            ApplicationBar.MenuItems.Clear();
            switch (e.Item.Header.ToString())
            {
                case "persona":
                    App.BlahguaAPI.CurrentUser.PropertyChanged -= User_PropertyChanged;  
                    break;


            }
        }


        private void OnPivotLoading(object sender, PivotItemEventArgs e)
        {
            string newItem = e.Item.Header.ToString();
            App.analytics.PostPageView("/self/" + newItem);

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

            // backgrounds
            if ((newItem == "persona") && (currentPage == "prefs"))
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
            else if ((newItem == "prefs") && (currentPage == "persona"))
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

                if (ProfilePivot.Items.Count() > 1)
                    offset = maxScroll / (ProfilePivot.Items.Count() - 1);
                else
                    offset = 0;
                ExponentialEase ease = new ExponentialEase();
                ease.Exponent = 5;
                ease.EasingMode = EasingMode.EaseIn;

                targetVal = offset * ProfilePivot.Items.IndexOf(e.Item);
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
            switch (currentPage)
            {
                case "persona":
                    ApplicationBar.Buttons.Add(signOutBtn);
                    ApplicationBar.IsVisible = true;
                    UpdatePersona();
                    
                    break;

                case "badges":
                    ApplicationBar.Buttons.Add(addBadgeBtn);
                    ApplicationBar.IsVisible = true;
                    UpdateBadgeArea();
                    break;

                case "demographics":
                    ApplicationBar.IsVisible = false;
                    UpdateDemographics();
                    break;

                case "posts":
                    ApplicationBar.Buttons.Add(openPostBtn);
                    ApplicationBar.Buttons.Add(deletePostBtn);
                    ApplicationBar.MenuItems.Add(sortByDateDesc);
                    ApplicationBar.MenuItems.Add(sortByDateAsc);
                    ApplicationBar.MenuItems.Add(sortByStrength);
                    ApplicationBar.MenuItems.Add(sortByUpVote);
                    ApplicationBar.MenuItems.Add(sortByDownVote);
                    ApplicationBar.IsVisible = true;
                    UpdateBlahHistoryButtons();
                    break;

                case "comments":
                    ApplicationBar.Buttons.Add(openCommentBtn);
                    ApplicationBar.Buttons.Add(deleteCommentBtn);
                    ApplicationBar.MenuItems.Add(sortByDateDesc);
                    ApplicationBar.MenuItems.Add(sortByDateAsc);
                    ApplicationBar.MenuItems.Add(sortByStrength);
                    ApplicationBar.MenuItems.Add(sortByUpVote);
                    ApplicationBar.MenuItems.Add(sortByDownVote);
                    ApplicationBar.IsVisible = true;
                    UpdateCommentHistoryButtons();
                    break;

                case "stats":
                    ApplicationBar.IsVisible = false;        
                    break;

                case "prefs":
                    ApplicationBar.IsVisible = false;
                    UpdatePrefs();
                    break;

                default:
                    ApplicationBar.IsVisible = false;     
                    break;
            }
        }

        private void UpdatePrefs()
        {
            App.BlahguaAPI.GetRecoveryEmail((theEmail) =>
                {
                    // don't need to do anything actually
                }
            );
        }

        private void UpdatePersona()
        {
            App.BlahguaAPI.CurrentUser.PropertyChanged += User_PropertyChanged;        
        }

        private void UpdateDemographics()
        {
            UserProfile theProfile = App.BlahguaAPI.CurrentUser.Profile;

            if (theProfile != null)
            {
                isDemoPopulated = false;
                GenderList.SelectedItem = theProfile.Gender;
                IncomeList.SelectedItem = theProfile.Income;
                CountryList.SelectedItem = theProfile.Country;
                RaceList.SelectedItem = theProfile.Race;
                CityField.Text = theProfile.City;
                StateField.Text = theProfile.State;
                ZipcodeField.Text = theProfile.Zipcode;
                DOBField.Value = theProfile.DOB;

                GenderPerm.IsChecked = theProfile.GenderPerm;
                IncomePerm.IsChecked = theProfile.IncomePerm;
                CountryPerm.IsChecked = theProfile.CountryPerm;
                RacePerm.IsChecked = theProfile.RacePerm;
                CityPerm.IsChecked = theProfile.CityPerm;
                StatePerm.IsChecked = theProfile.StatePerm;
                ZipcodePerm.IsChecked = theProfile.ZipcodePerm;
                DOBPerm.IsChecked = theProfile.DOBPerm;
                isDemoPopulated = true;
            }
        }

        void UpdateProfile()
        {
            // the profile has changed, save and reload the description...
            App.BlahguaAPI.UpdateUserProfile((theString) =>
                {
                    App.BlahguaAPI.GetUserDescription((theDesc) =>
                        {
                            // to do - see if we need to rebind or...
                        }
                    );
                }
            );
        }

        void User_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // here we are really just saving the user profile..
            if (e.PropertyName == "UserName")
            {
                App.BlahguaAPI.UpdateUserName(App.BlahguaAPI.CurrentUser.UserName, (theString) =>
                    {
                        App.BlahguaAPI.GetUserDescription((theDesc) =>
                            {
                                // to do - see if we need to rebind or...
                            }
                        );
                    }
                );
            }
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

        private void Country_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isDemoPopulated && (App.BlahguaAPI.CurrentUser.Profile != null) && (e.AddedItems.Count == 1))
            {
                string newVal = e.AddedItems[0].ToString();
                if (newVal != App.BlahguaAPI.CurrentUser.Profile.Country)
                {
                    App.BlahguaAPI.CurrentUser.Profile.Country = newVal;
                    UpdateProfile();
                }
            }
        }

        private void CountryPerm_Checked(object sender, RoutedEventArgs e)
        {
            if (isDemoPopulated && (App.BlahguaAPI.CurrentUser.Profile != null))
            {
                bool newVal = (bool)CountryPerm.IsChecked;

                if (newVal != App.BlahguaAPI.CurrentUser.Profile.CountryPerm)
                {
                    App.BlahguaAPI.CurrentUser.Profile.CountryPerm = newVal;
                    UpdateProfile();
                }
            }
        }

        private void Race_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (isDemoPopulated && (App.BlahguaAPI.CurrentUser.Profile != null) && (e.AddedItems.Count == 1))
            {
                string newVal = e.AddedItems[0].ToString();
                if (newVal != App.BlahguaAPI.CurrentUser.Profile.Race)
                {
                    App.BlahguaAPI.CurrentUser.Profile.Race = newVal;
                    UpdateProfile();
                }
            }

        }

        private void RacePerm_Checked(object sender, RoutedEventArgs e)
        {
            if (isDemoPopulated && (App.BlahguaAPI.CurrentUser.Profile != null))
            {
                bool newVal = (bool)RacePerm.IsChecked;

                if (newVal != App.BlahguaAPI.CurrentUser.Profile.RacePerm)
                {
                    App.BlahguaAPI.CurrentUser.Profile.RacePerm = newVal;
                    UpdateProfile();
                }
            }
        }

        private void Gender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (isDemoPopulated && (App.BlahguaAPI.CurrentUser.Profile != null) && (e.AddedItems.Count == 1))
            {
                string newVal = e.AddedItems[0].ToString();
                if (newVal != App.BlahguaAPI.CurrentUser.Profile.Gender)
                {
                    App.BlahguaAPI.CurrentUser.Profile.Gender = newVal;
                    UpdateProfile();
                }
            }
        }

        private void GenderPerm_Checked(object sender, RoutedEventArgs e)
        {
            if (isDemoPopulated && (App.BlahguaAPI.CurrentUser.Profile != null))
            {
                bool newVal = (bool)GenderPerm.IsChecked;

                if (newVal != App.BlahguaAPI.CurrentUser.Profile.GenderPerm)
                {
                    App.BlahguaAPI.CurrentUser.Profile.GenderPerm = newVal;
                    UpdateProfile();
                }
            }
        }

        private void Income_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isDemoPopulated && (App.BlahguaAPI.CurrentUser.Profile != null) && (e.AddedItems.Count == 1))
            {
                string newVal = e.AddedItems[0].ToString();
                if (newVal != App.BlahguaAPI.CurrentUser.Profile.Income)
                {
                    App.BlahguaAPI.CurrentUser.Profile.Income = newVal;
                    UpdateProfile();
                }
            }    
        }

        private void IncomePerm_Checked(object sender, RoutedEventArgs e)
        {
            if (isDemoPopulated && (App.BlahguaAPI.CurrentUser.Profile != null))
            {
                bool newVal = (bool)IncomePerm.IsChecked;

                if (newVal != App.BlahguaAPI.CurrentUser.Profile.IncomePerm)
                {
                    App.BlahguaAPI.CurrentUser.Profile.IncomePerm = newVal;
                    UpdateProfile();
                }
            } 
        }

        private void DOBPerm_Checked(object sender, RoutedEventArgs e)
        {
            if (isDemoPopulated && (App.BlahguaAPI.CurrentUser.Profile != null))
            {
                bool newVal = (bool)DOBPerm.IsChecked;

                if (newVal != App.BlahguaAPI.CurrentUser.Profile.DOBPerm)
                {
                    App.BlahguaAPI.CurrentUser.Profile.DOBPerm = newVal;
                    UpdateProfile();
                }
            }
        }

        private void CityPerm_Checked(object sender, RoutedEventArgs e)
        {
            if (isDemoPopulated && (App.BlahguaAPI.CurrentUser.Profile != null))
            {
                bool newVal = (bool)CityPerm.IsChecked;

                if (newVal != App.BlahguaAPI.CurrentUser.Profile.CityPerm)
                {
                    App.BlahguaAPI.CurrentUser.Profile.CityPerm = newVal;
                    UpdateProfile();
                }
            }
        }

        private void StatePerm_Checked(object sender, RoutedEventArgs e)
        {
            if (isDemoPopulated && (App.BlahguaAPI.CurrentUser.Profile != null))
            {
                bool newVal = (bool)StatePerm.IsChecked;

                if (newVal != App.BlahguaAPI.CurrentUser.Profile.StatePerm)
                {
                    App.BlahguaAPI.CurrentUser.Profile.StatePerm = newVal;
                    UpdateProfile();
                }
            }
        }

        private void ZipcodePerm_Checked(object sender, RoutedEventArgs e)
        {
            if (isDemoPopulated && (App.BlahguaAPI.CurrentUser.Profile != null))
            {
                bool newVal = (bool)ZipcodePerm.IsChecked;

                if (newVal != App.BlahguaAPI.CurrentUser.Profile.ZipcodePerm)
                {
                    App.BlahguaAPI.CurrentUser.Profile.ZipcodePerm = newVal;
                    UpdateProfile();
                }
            }
        }

        private void DOBValChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            if (isDemoPopulated && (App.BlahguaAPI.CurrentUser.Profile != null) && (e.NewDateTime != null))
            {
                DateTime newVal = (DateTime)e.NewDateTime;
                if (newVal != App.BlahguaAPI.CurrentUser.Profile.DOB)
                {
                    App.BlahguaAPI.CurrentUser.Profile.DOB = newVal;
                    UpdateProfile();
                }
            }
        }

        private void CityTextChanged(object sender, TextChangedEventArgs e)
        {
            if (isDemoPopulated && (App.BlahguaAPI.CurrentUser.Profile != null))
            {
                string newVal = CityField.Text;
                if (newVal != App.BlahguaAPI.CurrentUser.Profile.City)
                {
                    App.BlahguaAPI.CurrentUser.Profile.City = newVal;
                    UpdateProfile();
                }
            }
        }

        private void StateTextChanged(object sender, TextChangedEventArgs e)
        {
            if (isDemoPopulated && (App.BlahguaAPI.CurrentUser.Profile != null))
            {
                string newVal = StateField.Text;
                if (newVal != App.BlahguaAPI.CurrentUser.Profile.State)
                {
                    App.BlahguaAPI.CurrentUser.Profile.State = newVal;
                    UpdateProfile();
                }
            }
        }
        private void ZipcodeTextChanged(object sender, TextChangedEventArgs e)
        {
            if (isDemoPopulated && (App.BlahguaAPI.CurrentUser.Profile != null))
            {
                string newVal = ZipcodeField.Text;
                if (newVal != App.BlahguaAPI.CurrentUser.Profile.Zipcode)
                {
                    App.BlahguaAPI.CurrentUser.Profile.Zipcode = newVal;
                    UpdateProfile();
                }
            }
        }

        private void ClearImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.BlahguaAPI.DeleteUserImage((theString) =>
                {

                }
             );
        }

        private void ChangeImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PhotoChooserTask photoChooserTask;
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.ShowCamera = true;
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
            photoChooserTask.Show();
        }

        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                UploadImageProgress.Visibility = Visibility.Visible;
                App.BlahguaAPI.UploadUserImage(e.ChosenPhoto, e.OriginalFileName.Substring(e.OriginalFileName.LastIndexOf("\\") + 1), (photoString) =>
                    {
                        UploadImageProgress.Visibility = Visibility.Collapsed;
                        if ((photoString != null) && (photoString.Length > 0))
                        {

                        }
                        else
                        {

                        }
                    }
                );
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.analytics.PostPageView("/self");
        }

        private void UserCommentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem curItem;

            foreach (Comment oldComment in e.RemovedItems)
            {
                curItem = (ListBoxItem)UserCommentList.ItemContainerGenerator.ContainerFromItem(oldComment);
                if (curItem != null)
                    curItem.Background = new SolidColorBrush(Colors.Transparent);
            }

            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
            {
                curComment = null;
                openCommentBtn.IsEnabled = false;
            }
            else
            {
                foreach (Comment newComment in e.AddedItems)
                {
                    curItem = (ListBoxItem)UserCommentList.ItemContainerGenerator.ContainerFromItem(newComment);
                    if (curItem != null)
                    {
                        curItem.Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0));
                        curComment = newComment;
                        openCommentBtn.IsEnabled = true;
                    }
                    else
                    {
                        curComment = null;
                        openCommentBtn.IsEnabled = false;
                    }
                }
            }
        }

        private void UserPostList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem curItem;

            foreach (Blah oldBlah in e.RemovedItems)
            {
                curItem = (ListBoxItem)UserPostList.ItemContainerGenerator.ContainerFromItem(oldBlah);
                if (curItem != null)
                    curItem.Background = new SolidColorBrush(Colors.Transparent);
            }

            if ((e.AddedItems == null) || (e.AddedItems.Count == 0))
            {
                curBlah = null;
                openPostBtn.IsEnabled = false;
                deletePostBtn.IsEnabled = false;
            }
            else
            {
                foreach (Blah newBlah in e.AddedItems)
                {
                    curItem = (ListBoxItem)UserPostList.ItemContainerGenerator.ContainerFromItem(newBlah);
                    if (curItem != null)
                    {
                        curItem.Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0));
                        curBlah = newBlah;
                        openPostBtn.IsEnabled = true;
                        deletePostBtn.IsEnabled = true;
                    }
                    else
                    {
                        curBlah = null;
                        openPostBtn.IsEnabled = false;
                        deletePostBtn.IsEnabled = false;
                    }
                }
            }

        }

        private void ChangePassword_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            InputPromptSettings settings = new InputPromptSettings();
            settings.Field1Mode = InputMode.Password;
            settings.Field2Mode = InputMode.Password;
            RadInputPrompt.Show(settings, "Please enter new password", closedHandler: (args) =>
                {
                    if (args.Text != args.Text2)
                        MessageBox.Show("Passwords do not match.");
                    else
                        App.BlahguaAPI.UpdatePassword(args.Text, (theResult) =>
                            {

                            }
                    );
                }
            );
        }

        private void RecoveryInfo_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            InputPromptSettings settings = new InputPromptSettings();
            settings.Field1Mode = InputMode.Text;
            RadInputPrompt.Show(settings, "Please enter recovery email (leave blank to clear)", closedHandler: (args) =>
            {
                App.BlahguaAPI.SetRecoveryEmail(args.Text, (resultStr) =>
                    {
                        
                    }
                );
            }
            );
        }

        private void RateReview_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MarketplaceReviewTask rev = new MarketplaceReviewTask();
            rev.Show();

        }

        private void HyperlinkButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "feedback on blahgua";
            emailComposeTask.Body = "";
            emailComposeTask.To = "admin@blahgua.com";

            emailComposeTask.Show();
        }


       
    }
}
