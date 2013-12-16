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
using System.Windows.Media.Imaging;

namespace BibleBlahgua
{
    public partial class BlahRollItem : UserControl
    {
        private static float smallText = 16;
        private static float medText = 36;
        private static float largeText = 52;
        public InboxBlah BlahData {get; set;}

        public bool IsAnimating { get; set; }

        public BlahRollItem()
        {
            InitializeComponent();
            IsAnimating = false;
            BlahData = null;
            BlahImage.Loaded += BlahImage_Loaded;
            
        }

        public void Initialize(InboxBlah theBlah)
        {
            BlahData = theBlah;
            //BlahBackground.Fill = GetBlahFrameBrush(BlahData);
            TopBorder.BorderBrush = GetBlahFrameBrush(BlahData);
            User curUser = App.BlahguaAPI.CurrentUser;

            if ((curUser != null) && (curUser._id == theBlah.A))
            {
                OwnedBlahIndicator.Stroke = TopBorder.BorderBrush;
            }
            else
                ((Grid)OwnedBlahIndicator.Parent).Children.Remove(OwnedBlahIndicator);
            //BlahBackground.Opacity = .4;
            if (theBlah.B == null)
                BadgeIcon.Visibility = Visibility.Collapsed;
            
            TimeSpan diff = DateTime.Now - theBlah.Created;

            if (diff.TotalHours > 30)
                NewBlahIcon.Visibility = Visibility.Collapsed;

            TextArea.Text = BlahData.T;
        }

   
        void BlahImage_Loaded(object sender, RoutedEventArgs e)
        {
            if (BlahData.M != null)
            {
                BlahImage.ImageOpened += Image_loaded;
                string imageBase = BlahData.M[0];
                string imageSize = BlahData.ImageSize;
                string imageURL = App.BlahguaAPI.GetImageURL(imageBase, imageSize);
                BlahImage.Opacity = 0;
                BlahImage.Source = new BitmapImage(new Uri(imageURL, UriKind.Absolute));    
            } 
        }

        void Image_loaded(object sender, RoutedEventArgs e)
        {
            BlahImage.Opacity = 1;
            if ((BlahData.T != null) && (BlahData.T != ""))
            {
                BlahBackground.Opacity = .8;
                TextArea.Foreground = (Brush)App.Current.Resources["BrushBlahguaBlack"];
                BlahBackground.Visibility = Visibility.Collapsed;
                TextArea.Visibility = Visibility.Collapsed;
            }
            else
            {
                BlahBackground.Visibility = Visibility.Collapsed;
                TextArea.Visibility = Visibility.Collapsed;
            }
        }

        private Brush GetBlahBrush(InboxBlah theBlah)
        {
            Brush newBrush;


            switch (App.BlahguaAPI.CurrentBlahTypes.GetTypeName(theBlah.Y))
            {
                case "leaks":
                    newBrush = (Brush)App.Current.Resources["BaseBrushLeaks"];
                    break;
                case "polls":
                    newBrush =  (Brush)App.Current.Resources["BaseBrushPolls"];
                    break;
                case "asks":
                    newBrush =  (Brush)App.Current.Resources["BaseBrushAsks"];
                    break;
                case "predicts":
                    newBrush =  (Brush)App.Current.Resources["BaseBrushPredicts"];
                    break;
                default:
                    newBrush =  (Brush)App.Current.Resources["BaseBrushSays"];
                    break;
            }

            return newBrush;
        }

        private Brush GetBlahFrameBrush(InboxBlah theBlah)
        {
            Brush newBrush;


            switch (App.BlahguaAPI.CurrentBlahTypes.GetTypeName(theBlah.Y))
            {
                case "leaks":
                    newBrush =  (Brush)App.Current.Resources["HighlightBrushLeaks"];
                    break;
                case "polls":
                    newBrush =  (Brush)App.Current.Resources["HighlightBrushPolls"];
                    break;
                case "asks":
                    newBrush =  (Brush)App.Current.Resources["HighlightBrushAsks"];
                    break;
                case "predicts":
                    newBrush =  (Brush)App.Current.Resources["HighlightBrushPredicts"];
                    break;
                default:
                    newBrush =  (Brush)App.Current.Resources["HighlightBrushSays"];
                    break;
            }


            return newBrush;
        }


        public void ScaleTextToFit()
        {
            switch (BlahData.displaySize)
            {
                case 3:
                    TextArea.FontSize = smallText;
                    break;
                case 2:
                    TextArea.FontSize = medText;
                    break;
                case 1:
                    TextArea.FontSize = largeText;
                    break;

            }
            TextArea.TextTrimming = TextTrimming.WordEllipsis;
            /*
            float curFontSize = minTextSize;
            double lastSize = 0;
            TextArea.FontSize = curFontSize;
            
            TextArea.Measure(new Size(this.Width, Double.PositiveInfinity));
            double curSize = TextArea.DesiredSize.Height;

            while ((curSize < this.Height)) 
            {
                curFontSize++;
                if (curFontSize > maxTextSize)
                {
                    curFontSize--;
                    break;
                }
                else
                {
                    TextArea.FontSize = curFontSize;
                    lastSize = curSize;
                    TextArea.Measure(new Size(this.Width, Double.PositiveInfinity));
                    curSize = TextArea.DesiredSize.Height;
                }
            }
            curFontSize--;
            TextArea.FontSize = curFontSize;
            TextArea.TextTrimming = TextTrimming.WordEllipsis;
             */

        }
    }
}
