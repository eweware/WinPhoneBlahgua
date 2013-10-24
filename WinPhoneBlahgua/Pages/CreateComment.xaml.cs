using System;
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

namespace WinPhoneBlahgua
{
    public partial class CreateComment : PhoneApplicationPage
    {
        public CreateComment()
        {
            InitializeComponent();

            SelectedBadgesList.SummaryForSelectedItemsDelegate = SummarizeItems;

            App.BlahguaAPI.EnsureUserDescription((desc) =>
            {
                this.DataContext = App.BlahguaAPI;
            }
             );

            Loaded += CreateComment_Loaded;
        }

        void CreateComment_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = App.BlahguaAPI;
        }


        private void UseProfile_Checked(object sender, RoutedEventArgs e)
        {
            AuthorHeader.DataContext = null;
            App.BlahguaAPI.CreateCommentRecord.UseProfile = (bool)((CheckBox)sender).IsChecked;
            AuthorHeader.DataContext = App.BlahguaAPI.CreateCommentRecord;

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
            App.BlahguaAPI.CreateCommentRecord.M = null;
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
                            App.BlahguaAPI.CreateCommentRecord.M = new List<string>();
                            App.BlahguaAPI.CreateCommentRecord.M.Add(photoString);
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

                App.BlahguaAPI.CreateCommentRecord.Badges = newList;
            }
            else
                App.BlahguaAPI.CreateCommentRecord.Badges = null;

            AuthorHeader.DataContext = App.BlahguaAPI.CreateCommentRecord;
        }

        private void OnCreateCommentOK(Comment newComment)
        {
            if (newComment != null)
            {


            }
            else
            {
                // handle create comment failed

            }

            //App.BlahguaAPI.NewBlahToInsert = newBlah;

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

        private void DoCreateClick(object sender, EventArgs e)
        {
            App.BlahguaAPI.CreateComment(OnCreateCommentOK);
        }
    }

}