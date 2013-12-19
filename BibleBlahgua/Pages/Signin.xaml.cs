using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using Microsoft.Phone.Tasks;
using Telerik.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BibleBlahgua
{
    public partial class Signin : PhoneApplicationPage
    {
        bool createNewAccount = false;
        private string currentPage;

        public Signin()
        {
            InitializeComponent();
            DataContext = App.BlahguaAPI;
            currentPage = "sign in";
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            createNewAccount = (bool)NewAccountBox.IsChecked;

            if (createNewAccount)
            {
                AdditionalInfoPanel.Visibility = Visibility.Visible;
                CreateNewAccountBtn.Visibility = Visibility.Visible;
                SignInBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                AdditionalInfoPanel.Visibility = Visibility.Collapsed;
                CreateNewAccountBtn.Visibility = Visibility.Collapsed;
                SignInBtn.Visibility = Visibility.Visible;
            }
        }


        private void DoSignIn(object sender, EventArgs e)
        {
            SignInProgress.Visibility = Visibility.Visible;
            App.BlahguaAPI.SignIn(App.BlahguaAPI.UserName, App.BlahguaAPI.UserPassword, App.BlahguaAPI.AutoLogin, (errMsg) =>
                {
                    SignInProgress.Visibility = Visibility.Collapsed;
                    if (errMsg == null)
                        HandleUserSignIn();
                    else
                        MessageBox.Show("could not register: " + errMsg);
                }
            );
        }

        private void DoCreateAccount(object sender, EventArgs e)
        {
            if (App.BlahguaAPI.UserPassword != App.BlahguaAPI.UserPassword2)
                MessageBox.Show("Passwords must match");
            else
            {
                SignInProgress.Visibility = Visibility.Visible;
                App.BlahguaAPI.Register(App.BlahguaAPI.UserName, App.BlahguaAPI.UserPassword, App.BlahguaAPI.AutoLogin, (errMsg) =>
                    {
                        SignInProgress.Visibility = Visibility.Collapsed;
                        if (errMsg == null)
                            HandleUserSignIn();
                        else
                            MessageBox.Show("could not register: " + errMsg);
                    }
                );
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.analytics.PostPageView("/signup");
        }



        private void HandleUserSignIn()
        {
            NavigationService.GoBack();
        }

        private void HyperlinkButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "feedback on daily bread";
            emailComposeTask.Body = "";
            emailComposeTask.To = "dailybread@blahgua.com";

            emailComposeTask.Show();    
        }

        
        private void Recover_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            InputPromptSettings settings = new InputPromptSettings();
            settings.Field1Mode = InputMode.Text;
            settings.Field2Mode = InputMode.Text;
            RadInputPrompt.Show(settings, "Please enter username and email address", closedHandler: (args) =>
                {
                    App.BlahguaAPI.UpdatePassword(args.Text, (theResult) =>
                        {
                            MessageBox.Show("Check the email for a recovery link.");
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

        private void HandlePivotLoaded(object sender, PivotItemEventArgs e)
        {
            currentPage = e.Item.Header.ToString();
        }
        

        private void DonateButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=A827D4JMPL4X8", UriKind.Absolute);
            wbt.Show();
        }

        private void ChristImagesButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri("http://christimages.org", UriKind.Absolute);
            wbt.Show();
        }



        private void OnPivotLoading(object sender, PivotItemEventArgs e)
        {
            string newItem = e.Item.Header.ToString();
            App.analytics.PostPageView("/signin/" + newItem);

            // backgrounds
            if ((newItem == "sign in") && (currentPage == "help"))
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
                db1.To = -1000;
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
            else if ((newItem == "help") && (currentPage == "sign in"))
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
                db1.From = -1000;
                db1.To = -520;
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
                double maxScroll = -520;
                double offset;

                if (SignInPivot.Items.Count() > 1)
                    offset = maxScroll / (SignInPivot.Items.Count() - 1);
                else
                    offset = 0;
                ExponentialEase ease = new ExponentialEase();
                ease.Exponent = 5;
                ease.EasingMode = EasingMode.EaseIn;

                targetVal = offset * SignInPivot.Items.IndexOf(e.Item);
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
    }
}