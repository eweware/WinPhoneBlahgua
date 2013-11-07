using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;


namespace WinPhoneBlahgua
{
    public partial class BadgingPage : PhoneApplicationPage
    {
        string ticketStr = "";
        public BadgingPage()
        {
            InitializeComponent();

           

        }


        private void DoSubmitEmail(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SubmitBtn.IsEnabled = false;
            EmailField.IsEnabled = false;
            ProgressBox.Visibility = Visibility.Visible;
            App.BlahguaAPI.GetBadgeAuthorities((authList) =>
                {
                    string badgeId = authList[0]._id;
                    string emailAddr = EmailField.Text;

                    App.BlahguaAPI.GetEmailBadgeForUser(badgeId, emailAddr, (ticket) =>
                        {
                            ProgressBox.Visibility = Visibility.Collapsed;
                            if (ticket == "")
                            {
                                MessageBox.Show("The authority currently has no badges for that email address.  Please try again in the future.");
                                SubmitBtn.IsEnabled = true;
                                EmailField.IsEnabled = true;
                            }
                            else
                            {
                                ValidationField.Text = "";
                                ticketStr = ticket;
                                EmailArea.Visibility = Visibility.Collapsed;
                                ValidationArea.Visibility = Visibility.Visible;
                                ValidateBtn.IsEnabled = true;
                                ValidationField.IsEnabled = true;
                            }
                        }
                    );

                }
            );
        }

        private void DoValidate(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ValidateBtn.IsEnabled = false;
            ValidationField.IsEnabled = false;
            ProgressBox.Visibility = Visibility.Visible;

            string valString = ValidationField.Text;

            App.BlahguaAPI.VerifyEmailBadge(valString, ticketStr, (resultStr) =>
                {
                    if (resultStr == "")
                    {
                        ProgressBox.Visibility = Visibility.Collapsed;
                        MessageBox.Show("That validation code was not valid.  Please retry your badging attempt.");
                        SubmitBtn.IsEnabled = true;
                        EmailField.IsEnabled = true;
                        EmailArea.Visibility = Visibility.Visible;
                        ValidationArea.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        App.BlahguaAPI.RefreshUserBadges((theStr) =>
                            {
                                ProgressBox.Visibility = Visibility.Collapsed;
                                MessageBox.Show("Badging successful!");
                                NavigationService.GoBack();
                            }
                        );
                       
                    }
                }
            );

        }
    }
}