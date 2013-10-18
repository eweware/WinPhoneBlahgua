using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Threading;
using System.ComponentModel;


namespace WinPhoneBlahgua
{
    public partial class MainPage : PhoneApplicationPage
    {
         Inbox   blahList;
        DispatcherTimer scrollTimer = new DispatcherTimer();

        int[] rowSequence = new int[]{4,32,31,4,1,33,4,2,4,32,1,4,31,32,33,31,4,33,1,31,4,32,33,1,4,2};
        int screenMargin = 4;
        int blahMargin = 8;
        double smallBlahSize, mediumBlahSize, largeBlahSize;
        bool AtScrollEnd = false;
        int FramesPerSecond = 60;
     
        // Constructor
        public MainPage()
        {
            Loaded += new RoutedEventHandler(MainPage_Loaded); 
            InitializeComponent();
            this.DataContext = null;


            
        }


        void BlahScroller_MouseMove(object sender, MouseEventArgs e)
        {
            DetectScrollAtEnd();
        }

        void DetectScrollAtEnd()
        {
            if ((BlahScroller.VerticalOffset == BlahScroller.ScrollableHeight) && (BlahContainer.Children.Count > 10))
            {
                if (!AtScrollEnd)
                {
                    AtScrollEnd = true;
                    FetchNextBlahList();
                }

            }
           
        }

        private void InitService()
        {
            App.BlahguaAPI.Initialize(DoServiceInited); 

            
        }


        private void FetchInitialBlahList()
        {
            App.BlahguaAPI.GetInbox((newBlahList) =>
                {
                    blahList = newBlahList;
                    blahList.PrepareBlahs();
                    RenderInitialBlahs();
                    scrollTimer.Start();
                   

                });
        }

        private void FetchNextBlahList()
        {
            App.BlahguaAPI.GetInbox((newBlahList) =>
            {
                blahList = newBlahList;
                blahList.PrepareBlahs();
                InsertAdditionalBlahs();
                AtScrollEnd = false;

            });
        }

        private void ScrollBlahRoll(object sender, EventArgs e)
        {
            double curOffset = BlahScroller.VerticalOffset;
            curOffset += 1.0;
            BlahScroller.ScrollToVerticalOffset(curOffset);
            DetectScrollAtEnd();
        }

        
        

        private void RenderInitialBlahs()
        {
            double curTop = screenMargin;
            smallBlahSize = (480 - ((screenMargin * 2) + (blahMargin * 3))) / 4;
            mediumBlahSize = smallBlahSize + smallBlahSize + blahMargin;
            largeBlahSize = mediumBlahSize + mediumBlahSize + blahMargin;

            foreach (int rowType in rowSequence)
            {
                curTop = InsertRow(rowType, curTop);
                curTop += blahMargin;
            }

            BlahContainer.Height = curTop + screenMargin;
        }

        private void ClearBlahs()
        {
            BlahContainer.Children.Clear();
        }

        private void InsertAdditionalBlahs()
        {
            double curTop = BlahContainer.Height;
            foreach (int rowType in rowSequence)
            {
                curTop = InsertRow(rowType, curTop);
                curTop += blahMargin;
            }

            BlahContainer.Height = curTop + blahMargin;
        }

        private void InsertElementForBlah(InboxBlah theBlah, double xLoc, double yLoc, double width, double height)
        {
            BlahRollItem newBlahItem = new BlahRollItem();
            newBlahItem.Initialize(theBlah);
            newBlahItem.Width = width;
            newBlahItem.Height = height;
            Canvas.SetLeft(newBlahItem, xLoc);
            Canvas.SetTop(newBlahItem, yLoc);

            BlahContainer.Children.Add(newBlahItem);
                newBlahItem.ScaleTextToFit();

        }

        private void HandleScrollStart(object sender, ManipulationStartedEventArgs e)
        {
            scrollTimer.Stop();
        }

        bool alreadyHookedScrollEvents = false;

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            TiltEffect.TiltableItems.Add(typeof(BlahRollItem));
            if (alreadyHookedScrollEvents)
                return;

            alreadyHookedScrollEvents = true;
            // Visual States are always on the first child of the control template 
            FrameworkElement element = VisualTreeHelper.GetChild(BlahScroller, 0) as FrameworkElement;
            if (element != null)
            {
                VisualStateGroup group = FindVisualState(element, "ScrollStates");
                if (group != null)
                {
                    group.CurrentStateChanging += ScrollStateHandler;
                }
            }

            scrollTimer.Interval = new TimeSpan(TimeSpan.TicksPerSecond / FramesPerSecond);
            scrollTimer.Tick += ScrollBlahRoll;

            BlahScroller.MouseMove += BlahScroller_MouseMove;

            BlahContainer.Tap += BlahContainer_Tap;
            BlahContainer.ManipulationCompleted += BlahContainer_ManipulationCompleted;
            App.BlahguaAPI.PropertyChanged += new PropertyChangedEventHandler(On_API_PropertyChanged);

            InitService();
            
        }

        void BlahContainer_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            Point finalVel = e.FinalVelocities.LinearVelocity;
            Point finalDelta = e.TotalManipulation.Translation;

            if ((Math.Abs(finalDelta.X) > Math.Abs(finalDelta.Y)) &&
                (Math.Abs(finalVel.X) > Math.Abs(finalVel.Y)))
            {
                if (finalDelta.X < 0)
                    App.BlahguaAPI.GoNextChannel();
                else
                    App.BlahguaAPI.GoPrevChannel();

            }
            else
            {
                if (!scrollTimer.IsEnabled)
                    scrollTimer.Start();
            }
        }



        void On_API_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CurrentChannel":
                    OnChannelChanged();
                    break;
            }
        }

        void BlahContainer_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (e.OriginalSource != sender)
            {
                FrameworkElement curEl = (FrameworkElement)e.OriginalSource;

                while (curEl.GetType() != typeof(BlahRollItem))
                {
                    curEl = (FrameworkElement)curEl.Parent;
                }

                BlahRollItem curBlah = (BlahRollItem)curEl;
                OpenBlahItem(curBlah);
            }
            if (!scrollTimer.IsEnabled)
                scrollTimer.Start();
        }

        void OpenBlahItem(BlahRollItem curBlah)
        {
            scrollTimer.Stop();
            App.BlahguaAPI.SetCurrentBlahFromId(curBlah.BlahData.I, OpenFullBlah);
            

        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            scrollTimer.Stop();
            base.OnNavigatingFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (App.BlahguaAPI.CurrentUser != null)
            {
                UserInfoBtn.Visibility = Visibility.Visible;
                NewBlahBtn.Visibility = Visibility.Visible;
                SignInBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                UserInfoBtn.Visibility = Visibility.Collapsed;
                NewBlahBtn.Visibility = Visibility.Collapsed;
                SignInBtn.Visibility = Visibility.Visible;
            }   

            scrollTimer.Start();
        }


        void OpenFullBlah(Blah theBlah)
        {
            if (theBlah != null)
            {
                NavigationService.Navigate(new Uri("/BlahDetails.xaml", UriKind.Relative));

                // openthe page
                /*
                if (blahPage == null)
                    blahPage = new BlahViewer();
                blahPage.CurrentBlah = theBlah;
                OpenPage(blahPage);
                 * */
               
            }
            else
            {
                MessageBox.Show("Blah failed to load");
            }
        }

       

        void DoServiceInited(bool didIt)
        {
            
            this.DataContext = App.BlahguaAPI;
        }

        void OnChannelChanged()
        {
            ClearBlahs();
            FetchInitialBlahList();


        }

        private void ScrollStateHandler(object sender, VisualStateChangedEventArgs args)
        {
            if (args.NewState.Name == "NotScrolling")
                scrollTimer.Start();
            DetectScrollAtEnd();

        }

        VisualStateGroup FindVisualState(FrameworkElement element, string name)
        {
            if (element == null)
                return null;

            IList groups = VisualStateManager.GetVisualStateGroups(element);
            foreach (VisualStateGroup group in groups)
                if (group.Name == name)
                    return group;

            return null;
        }

        T FindSimpleVisualChild<T>(DependencyObject element) where T : class
        {
            while (element != null)
            {

                if (element is T)
                    return element as T;

                element = VisualTreeHelper.GetChild(element, 0);
            }

            return null;
        }

        private double InsertRow(int rowType, double topLoc)
        {
            double newTop = topLoc;
            switch (rowType)
            {
                case 1:
                    newTop = InsertRowType1(topLoc);
                    break;
                case 2:
                    newTop = InsertRowType2(topLoc);
                    break;
                case 31:
                    newTop = InsertRowType31(topLoc);
                    break;
                case 32:
                    newTop = InsertRowType32(topLoc);
                    break;
                case 33:
                    newTop = InsertRowType33(topLoc);
                    break;
                case 4:
                    newTop = InsertRowType4(topLoc);
                    break;
            }

            return newTop;
        }

        private double InsertRowType1(double topLoc)
        {
            double newTop = topLoc;
            InboxBlah nextBlah = blahList.PopBlah(1);
            InsertElementForBlah(nextBlah, screenMargin, topLoc, largeBlahSize, mediumBlahSize);
            newTop += mediumBlahSize;


            return newTop;
        }

        private double InsertRowType2(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah = blahList.PopBlah(2);
            InsertElementForBlah(nextBlah, curLeft, topLoc, mediumBlahSize, mediumBlahSize);
            curLeft += mediumBlahSize + blahMargin;
            nextBlah = blahList.PopBlah(2);
            InsertElementForBlah(nextBlah, curLeft, topLoc, mediumBlahSize, mediumBlahSize);
            newTop += mediumBlahSize;

            return newTop;
        }


        private double InsertRowType31(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah = blahList.PopBlah(2);
            InsertElementForBlah(nextBlah, curLeft, topLoc, mediumBlahSize, mediumBlahSize);
            curLeft += mediumBlahSize + blahMargin;
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc + smallBlahSize + blahMargin, smallBlahSize, smallBlahSize);
            curLeft += smallBlahSize + blahMargin;
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc + smallBlahSize + blahMargin, smallBlahSize, smallBlahSize);

            newTop += mediumBlahSize;

            return newTop;
        }


        private double InsertRowType32(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah;
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc + smallBlahSize + blahMargin, smallBlahSize, smallBlahSize);
            curLeft += smallBlahSize + blahMargin;

            nextBlah = blahList.PopBlah(2);
            InsertElementForBlah(nextBlah, curLeft, topLoc, mediumBlahSize, mediumBlahSize);
            curLeft += mediumBlahSize + blahMargin;

            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc + smallBlahSize + blahMargin, smallBlahSize, smallBlahSize);
            
           
            newTop += mediumBlahSize;

            return newTop;
        }


        private double InsertRowType33(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah;
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc + smallBlahSize + blahMargin, smallBlahSize, smallBlahSize);
            curLeft += smallBlahSize + blahMargin;

            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc + smallBlahSize + blahMargin, smallBlahSize, smallBlahSize);
            curLeft += smallBlahSize + blahMargin;

            nextBlah = blahList.PopBlah(2);
            InsertElementForBlah(nextBlah, curLeft, topLoc, mediumBlahSize, mediumBlahSize);
            curLeft += mediumBlahSize + blahMargin;

            newTop += mediumBlahSize;
            return newTop;
        }


        private double InsertRowType4(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah;

            for (int i = 0; i < 4; i++)
            {

                nextBlah = blahList.PopBlah(3);
                InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
                curLeft += smallBlahSize + blahMargin;
            }
            newTop += smallBlahSize;

            return newTop;
        }

        private void DoSignIn(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Signin.xaml", UriKind.Relative));    
        }

        private void NewBlahBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UserInfoBtn_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void NewBlahBtn_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

   




    }
}