using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Media;
using System.Windows.Media.Imaging;

namespace WinPhoneBlahgua
{
    public partial class PhotoShare : PhoneApplicationPage
    {
        public PhotoShare()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get a dictionary of query string keys and values.
            IDictionary<string, string> queryStrings = this.NavigationContext.QueryString;

            // Ensure that there is at least one key in the query string, and check whether the "FileId" key is present.
            if (queryStrings.ContainsKey("FileId"))
            {
                // Retrieve the photo from the media library using the FileID passed to the app.
                MediaLibrary library = new MediaLibrary();
                Picture photoFromLibrary = library.GetPictureFromToken(queryStrings["FileId"]);

                // Create a BitmapImage object and add set it as the image control source.
                // To retrieve a full-resolution image, use the GetImage() method instead.
                BitmapImage bitmapFromPhoto = new BitmapImage();
                bitmapFromPhoto.SetSource(photoFromLibrary.GetImage());
                PreviewImage.Source = bitmapFromPhoto;
            }
        }
    }

    

}