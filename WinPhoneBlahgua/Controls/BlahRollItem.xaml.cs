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

namespace WinPhoneBlahgua
{
    public partial class BlahRollItem : UserControl
    {
        private static float minTextSize = 12;
        private static float maxTextSize = 72;
        public InboxBlah BlahData {get; set;}

        public BlahRollItem()
        {
            InitializeComponent();
            BlahData = null;
        }

        public void Initialize(InboxBlah theBlah)
        {
            BlahData = theBlah;
            BlahBackground.Fill = GetBlahFrameBrush(BlahData);
            TopBorder.BorderBrush = GetBlahFrameBrush(BlahData);
            if (BlahData.M != null)
            {
                string imageBase = BlahData.M[0];
                string imageSize = BlahData.ImageSize;
                string imageURL = App.BlahguaAPI.GetImageURL(imageBase, imageSize);
                BlahImage.Source = new BitmapImage(new Uri(imageURL, UriKind.Absolute));
                if ((BlahData.T != null) && (BlahData.T != ""))
                {
                    // Put the text on a grey background...
                    BlahBackground.Opacity = .8;
                    TextArea.Foreground = (Brush)App.Current.Resources["BrushBlahguaWhite"];
                }
                else
                {
                    BlahBackground.Visibility = Visibility.Collapsed;
                    TextArea.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                BlahBackground.Opacity = .4;
                BlahImage.Visibility = Visibility.Collapsed;
            }

            TextArea.Text = BlahData.T;
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

        }
    }
}
