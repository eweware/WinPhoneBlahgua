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
        private static SolidColorBrush saysBrush = new SolidColorBrush(Color.FromArgb(102, 125, 181, 227));
        private static SolidColorBrush leaksBrush = new SolidColorBrush(Color.FromArgb(102, 248, 120, 88));
        private static SolidColorBrush pollsBrush = new SolidColorBrush(Color.FromArgb(102, 248, 184, 0));
        private static SolidColorBrush predictsBrush = new SolidColorBrush(Color.FromArgb(102, 250, 250, 100));
        private static SolidColorBrush asksBrush = new SolidColorBrush(Color.FromArgb(102, 122, 208, 0));
        private static SolidColorBrush saysFrameBrush = new SolidColorBrush(Color.FromArgb(255, 125, 181, 227));
        private static SolidColorBrush leaksFrameBrush = new SolidColorBrush(Color.FromArgb(255, 248, 120, 88));
        private static SolidColorBrush pollsFrameBrush = new SolidColorBrush(Color.FromArgb(255, 248, 184, 0));
        private static SolidColorBrush predictsFrameBrush = new SolidColorBrush(Color.FromArgb(255, 250, 250, 100));
        private static SolidColorBrush asksFrameBrush = new SolidColorBrush(Color.FromArgb(255, 122, 208, 0));

        public BlahRollItem()
        {
            InitializeComponent();
            BlahData = null;
        }

        public void Initialize(InboxBlah theBlah)
        {
            BlahData = theBlah;
            Background.Fill = GetBlahBrush(BlahData);
            TopBorder.BorderBrush = GetBlahFrameBrush(BlahData);
            if (BlahData.M != null)
            {
                string imageBase = BlahData.M[0];
                string imageSize = BlahData.ImageSize;
                string imageURL = App.BlahguaRest.GetImageURL(imageBase, imageSize);
                Background.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri(imageURL, UriKind.Absolute)) };
                if ((BlahData.T != null) && (BlahData.T != ""))
                {
                    // Put the text on a grey background...
                }
            }

            TextArea.Text = BlahData.T;
        }

        private Brush GetBlahBrush(InboxBlah theBlah)
        {
            Brush newBrush;


            switch (App.BlahguaAPI.CurrentBlahTypes.GetTypeName(theBlah.Y))
            {
                case "leaks":
                    newBrush = leaksBrush;
                    break;
                case "polls":
                    newBrush = pollsBrush;
                    break;
                case "asks":
                    newBrush = asksBrush;
                    break;
                case "predicts":
                    newBrush = predictsBrush;
                    break;
                default:
                    newBrush = saysBrush;
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
                    newBrush = leaksFrameBrush;
                    break;
                case "polls":
                    newBrush = pollsFrameBrush;
                    break;
                case "asks":
                    newBrush = asksFrameBrush;
                    break;
                case "predicts":
                    newBrush = predictsFrameBrush;
                    break;
                default:
                    newBrush = saysFrameBrush;
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
