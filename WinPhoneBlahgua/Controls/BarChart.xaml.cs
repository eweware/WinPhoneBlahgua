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
    public partial class BarChart : UserControl
    {
        public BarChart()
        {
            InitializeComponent();
        }

        public string ChartTitle
        {
            get { return (string)GetValue(ChartTitleProperty); }
            set { SetValue(ChartTitleProperty, value); }
        }

        public static readonly DependencyProperty ChartTitleProperty =
        DependencyProperty.Register(
            "ChartTitle",
            typeof(string),
            typeof(BarChart),
            new PropertyMetadata("untitled chart", OnChartTitlePropertyChanged));

        private static void OnChartTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BarChart source = d as BarChart;
            string value = (string)e.NewValue;

            // Do stuff when new value is assigned.
        }
    }
}
