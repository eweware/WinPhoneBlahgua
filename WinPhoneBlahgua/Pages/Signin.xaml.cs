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

namespace WinPhoneBlahgua
{
    public partial class Signin : PhoneApplicationPage
    {
        bool createNewAccount = false;

        public Signin()
        {
            InitializeComponent();
            DataContext = App.BlahguaAPI;
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



        private void HandleUserSignIn()
        {
            NavigationService.GoBack();
        }

        private void HyperlinkButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "feedback on blahgua";
            emailComposeTask.Body = "";
            emailComposeTask.To = "admin@blahgua.com";

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
    }
}