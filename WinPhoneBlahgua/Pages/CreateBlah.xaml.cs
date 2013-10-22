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
    public partial class CreateBlah : PhoneApplicationPage
    {
        public CreateBlah()
        {
            InitializeComponent();
            if (App.BlahguaAPI.CreateRecord == null)
                App.BlahguaAPI.CreateRecord = new BlahCreateRecord();

            SelectedBadgesList.SummaryForSelectedItemsDelegate = SummarizeItems;

            App.BlahguaAPI.EnsureUserDescription((desc) => 
                {
                    this.DataContext = App.BlahguaAPI;
                }
             );

        }

        private void DoCreateBlah(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.BlahguaAPI.CreateBlah(OnCreateBlahOK);
        }

        private void OnCreateBlahOK(Blah newBlah)
        {
            if (newBlah != null)
            {
                

            }
            else
            {
                // handle create blah failed

            }

            App.BlahguaAPI.NewBlahToInsert = newBlah;

            NavigationService.GoBack();

        }

        private string SummarizeItems(System.Collections.IList theItems)
        {
            if ((theItems == null) || (theItems.Count == 0))
            {
                return "no badges selected";
            }
            else
            {
                string badgeNames = "";
                foreach (BadgeReference curBadge in theItems)
                {
                    if (badgeNames != "")
                        badgeNames += ", ";
                    badgeNames += curBadge.BadgeName;

                }

                return badgeNames;
            }
        }
    }
}