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


namespace WinPhoneBlahgua
{
    public partial class Signin : PhoneApplicationPage
    {
        public Signin()
        {
            InitializeComponent();
            DataContext = App.BlahguaAPI;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (App.BlahguaAPI.NewAccount)
                AdditionalInfoPanel.Visibility = Visibility.Visible;
            else
                AdditionalInfoPanel.Visibility = Visibility.Collapsed;
        }


        private void DoSignIn(object sender, EventArgs e)
        {
            SignInProgress.Visibility = Visibility.Visible;
            if (App.BlahguaAPI.NewAccount)
            {
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
            else
            {
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
        }

        private void HandleUserSignIn()
        {
            NavigationService.GoBack();
        }
    }
}