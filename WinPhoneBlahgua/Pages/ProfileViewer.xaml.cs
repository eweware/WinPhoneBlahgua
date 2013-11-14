﻿using System;
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

        private void HandlePivotLoaded(object sender, PivotItemEventArgs e)
        {
            switch (e.Item.Header.ToString())
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

        private void UpdatePersona()
        {
            App.BlahguaAPI.CurrentUser.PropertyChanged += User_PropertyChanged;        
        }

        private void UpdateDemographics()
        {
            UserProfile theProfile = App.BlahguaAPI.CurrentUser.Profile;

            if (theProfile != null)
            {
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
            if ((App.BlahguaAPI.CurrentUser.Profile != null) && (e.AddedItems.Count == 1))
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
            if (App.BlahguaAPI.CurrentUser.Profile != null)
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

            if ((App.BlahguaAPI.CurrentUser.Profile != null) && (e.AddedItems.Count == 1))
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
            if (App.BlahguaAPI.CurrentUser.Profile != null)
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

            if ((App.BlahguaAPI.CurrentUser.Profile != null) && (e.AddedItems.Count == 1))
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
            if (App.BlahguaAPI.CurrentUser.Profile != null)
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
            if ((App.BlahguaAPI.CurrentUser.Profile != null) && (e.AddedItems.Count == 1))
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
            if (App.BlahguaAPI.CurrentUser.Profile != null)
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
            if (App.BlahguaAPI.CurrentUser.Profile != null)
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
            if (App.BlahguaAPI.CurrentUser.Profile != null)
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
            if (App.BlahguaAPI.CurrentUser.Profile != null)
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
            if (App.BlahguaAPI.CurrentUser.Profile != null)
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
            if ((App.BlahguaAPI.CurrentUser.Profile != null) && (e.NewDateTime != null))
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
            if (App.BlahguaAPI.CurrentUser.Profile != null)
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
            if (App.BlahguaAPI.CurrentUser.Profile != null)
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
            if (App.BlahguaAPI.CurrentUser.Profile != null)
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

       
    }
}
