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
    public partial class BlahViewer : UserControl
    {
        Blah theBlah;
        public BlahViewer()
        {
            theBlah = null;
            InitializeComponent();
            Loaded += BlahViewer_Loaded;
        }

        void BlahViewer_Loaded(object sender, RoutedEventArgs e)
        {
            CloseBox.Tap += CloseBox_Tap;
        }

        void CloseBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ((MainPage)((Grid)this.Parent).Parent).ClosePage();
        }

        public Blah CurrentBlah
        {
            get
            {
                return theBlah;
            }

            set
            {
                theBlah = value;
                BindToBlah();
            }
        }

        void BindToBlah()
        {
            if (theBlah != null)
            {
                this.DataContext = theBlah;
            }
            else
            {
                // clear everything
            }

        }
    }
}
