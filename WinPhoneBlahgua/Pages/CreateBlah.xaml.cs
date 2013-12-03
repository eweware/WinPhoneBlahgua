﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using System.Windows.Data;



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
            if ((App.BlahguaAPI.CurrentUser.Badges != null) && (App.BlahguaAPI.CurrentUser.Badges.Count > 0))
                SelectedBadgesList.Visibility = Visibility.Visible;
            else
                SelectedBadgesList.Visibility = Visibility.Collapsed;

            App.BlahguaAPI.EnsureUserDescription((desc) => 
                {
                    this.DataContext = App.BlahguaAPI;
                }
             );

        }

       

        private void OnCreateBlahOK(Blah newBlah)
        {
            if (newBlah != null)
            {

                App.BlahguaAPI.NewBlahToInsert = newBlah;

                NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("Unable to create the blah.  Please try again.  If the problem persists, go ahead and try at a different time.");

            }



        }

        private string IsBlahValid()
        {
            BlahCreateRecord curBlah = App.BlahguaAPI.CreateRecord;

            bool hasImage = ((curBlah.M != null) && (curBlah.M.Count > 0));

            if (curBlah.T == null)
            {
                if (!hasImage)
                    return "Headline is too short for a post with no image (< 3 characters)";
            }
            else
            {
                if ((curBlah.T.Length < 3) && (!hasImage))
                    return "Headline is too short for a post with no image (< 3 characters)";
             
                if (curBlah.T.Length > 64)
                    return "Headline is too long (> 64 characters)";
            }

            if ((curBlah.F != null) && (curBlah.F.Length > 2000))
                return "Body text is too long (> 2000 characters)";

            // type restrictions
            switch (curBlah.BlahType.N)
            {
                case "leaks":
                    if ((curBlah.B == null) || (curBlah.B.Count == 0))
                        return "Leaks must be badged.";
                    break;

                case "asks":
                    if (curBlah.T.IndexOf("?") == -1)
                        return "Asks must contain a question mark.";
                    break;

                case "polls":
                    if ((curBlah.I == null) || (curBlah.I.Count < 2))
                        return "Polls require at least two chouices.";

                    foreach (PollItem curItem in curBlah.I)
                    {
                        if ((curItem.T == null) || (curItem.T.Length == 0))
                            return "Each poll response requires a title.";
                    }
                    break;

                case "predicts":
                    if ((curBlah.ExpirationDate == null) || (curBlah.ExpirationDate <= DateTime.Now.AddDays(1)))
                        return "Predictions must be at least a day in the future.";

                    break;
            }
            

            return "";
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

        private void BlahTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems != null) && (e.AddedItems.Count > 0))
            {

                BlahType newType = e.AddedItems[0] as BlahType;
                Storyboard sb;

                switch (newType.N)
                {
                    case "polls":
                        sb = ((Storyboard)Resources["ShowPollAnimation"]);
                        break;
                    case "predicts":
                        sb = ((Storyboard)Resources["ShowPredictionAnimation"]);
                        break;
                    default:
                        sb = ((Storyboard)Resources["HideAllSectionsAnimation"]);
                        break;

                }

                //sb.SpeedRatio = 3;
                sb.Begin();
                SetBlahBackground(newType.N);
            }

            

        }

       

        private void UseProfile_Checked(object sender, RoutedEventArgs e)
        {
            AuthorHeader.DataContext = null;
            App.BlahguaAPI.CreateRecord.UseProfile = (bool)((CheckBox)sender).IsChecked;
            AuthorHeader.DataContext = App.BlahguaAPI.CreateRecord;
             
        }

        private void DoAddImage(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PhotoChooserTask photoChooserTask;
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.ShowCamera = true;
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
            photoChooserTask.Show();
        }

        private void DoRemoveImage(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ClearImages();
        }

        private void ClearImages()
        {
            App.BlahguaAPI.CreateRecord.M = null;
            ImagesPanel.Children.Clear();
            ImagesPanel.Children.Add(NoImageText);
            BackgroundImage.Source = null;
        }

        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                Image newImage = new Image();
                newImage.Width = 256;
                newImage.Height = 256;
                newImage.Stretch = System.Windows.Media.Stretch.UniformToFill;
                if (NoImageText.Parent != null)
                    ImagesPanel.Children.Remove(NoImageText);

                ImagesPanel.Children.Clear();
                newImage.Margin = new Thickness(8);
                newImage.Source = new BitmapImage(new Uri("/Images/uploadplaceholder.png", UriKind.Relative));
                ImagesPanel.Children.Add(newImage);
                ProgressBar newBar = new ProgressBar();
                newBar.IsIndeterminate = true;
                newBar.Width = 256;
                ImagesPanel.Children.Add(newBar);
                App.BlahguaAPI.UploadPhoto(e.ChosenPhoto, e.OriginalFileName.Substring(e.OriginalFileName.LastIndexOf("\\") + 1), (photoString) =>
                    {
                        if ((photoString != null) && (photoString.Length > 0))
                        {
                            newImage.Tag = photoString;
                            string photoURL = App.BlahguaAPI.GetImageURL(photoString, "B");
                            newImage.Source = new BitmapImage(new Uri(photoURL, UriKind.Absolute));
                            ImagesPanel.Children.Remove(newBar);
                            App.BlahguaAPI.CreateRecord.M = new List<string>();
                            App.BlahguaAPI.CreateRecord.M.Add(photoString);
                            BackgroundImage.Source = new BitmapImage(new Uri(App.BlahguaAPI.GetImageURL(photoString, "D"), UriKind.Absolute));
                        }
                        else
                            ClearImages();
                    }
                );
            }
        }

        private void SelectedBadgesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AuthorHeader.DataContext = null;
            System.Collections.IList curSelection = SelectedBadgesList.SelectedItems;

            if ((curSelection != null) && (curSelection.Count > 0))
            {
                BadgeList newList = new BadgeList();
                foreach (object curItem in curSelection)
                {
                    BadgeReference curBadge = (BadgeReference)curItem;
                    newList.Add(curBadge);
                }

                App.BlahguaAPI.CreateRecord.Badges = newList;
            }
            else
                App.BlahguaAPI.CreateRecord.Badges = null;

            AuthorHeader.DataContext = App.BlahguaAPI.CreateRecord;
        }

        private void PollItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (App.BlahguaAPI.CreateRecord.I.Count > 2)
            {
                var curItem = sender;
                var parent = VisualTreeHelper.GetParent((TextBlock)sender);

                while (!(parent is StackPanel))
                {
                    curItem = parent;
                    parent = VisualTreeHelper.GetParent(parent);
                }

                int index = ((StackPanel)parent).Children.IndexOf((UIElement)curItem);
                App.BlahguaAPI.CreateRecord.I.RemoveAt(index);
                MaybeEnableAddPollBtns();

            }
        }


        void SetBlahBackground(string newType)
        {
            Brush newBrush;

            switch (newType)
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


        private void DoAddPollChoice(object sender, System.Windows.Input.GestureEventArgs e)
        {
            int count = App.BlahguaAPI.CreateRecord.I.Count;

            if (count < 10)
            {
                App.BlahguaAPI.CreateRecord.I.Add(new PollItem("choice " + (count + 1)));
                MaybeEnableAddPollBtns();
            }
        }

        private void MaybeEnableAddPollBtns()
        {
            int count = App.BlahguaAPI.CreateRecord.I.Count;

            AddPollChoiceBtn.IsEnabled = (count < 10);

            foreach (object curItem in PollChoiceList.Descendents().OfType<TextBlock>())
            {
                if (count > 2)
                    ((TextBlock)curItem).Opacity = 1;
                else
                    ((TextBlock)curItem).Opacity = .4;
            }
        }

        private void DoCreateClick(object sender, EventArgs e)
        {
            SelectedBadgesList.Focus();
            App.BlahguaAPI.CreateRecord.T = BlahHeadlineBox.Text;
            string valStr = IsBlahValid();
            if (valStr == "")
            {
                App.BlahguaAPI.CreateBlah(OnCreateBlahOK);
            }
            else
            {
                MessageBox.Show(valStr);
            }
        }

    }
}