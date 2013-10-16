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


namespace WinPhoneBlahgua
{
    public partial class MainPage : PhoneApplicationPage
    {
         Inbox   blahList;
        DispatcherTimer scrollTimer = new DispatcherTimer();

        private string curGroupId = "";
        int[] rowSequence = new int[]{4,32,31,4,1,33,4,2,4,32,1,4,31,32,33,31,4,33,1,31,4,32,33,1,4,2};
        int screenMargin = 4;
        int blahMargin = 8;
        double smallBlahSize, mediumBlahSize, largeBlahSize;
        bool AtScrollEnd = false;
        Channel CurrentChannel = null;
        int FramesPerSecond = 30;
        BlahViewer blahPage = null;
     
        
 
        // Constructor
        public MainPage()
        {
            Loaded += new RoutedEventHandler(MainPage_Loaded); 
            InitializeComponent();
            
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
                    BlahContainer.Background = new SolidColorBrush(Color.FromArgb(255, 20, 20, 20));
                    AtScrollEnd = true;
                    FetchNextBlahList();
                }

            }
           
        }

        private void InitService()
        {
            App.BlahguaAPI.Initialize(DoServiceInited); 

            
        }

   

        private void SetCurrentChannel(int curChannel)
        {
            ChannelTitleBar.SelectedIndex = curChannel;
            
        }

        private void FetchInitialBlahList()
        {
            App.BlahguaRest.GetInbox(curGroupId, (newBlahList) =>
                {
                    blahList = newBlahList;
                    blahList.PrepareBlahs();
                    RenderInitialBlahs();
                    scrollTimer.Start();
                   

                });
        }

        private void FetchNextBlahList()
        {
            App.BlahguaRest.GetInbox(curGroupId, (newBlahList) =>
            {
                blahList = newBlahList;
                blahList.PrepareBlahs();
                InsertAdditionalBlahs();

                BlahContainer.Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
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

            InitService();

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
        }

        void OpenBlahItem(BlahRollItem curBlah)
        {
            scrollTimer.Stop();
            Mask.Visibility = Visibility.Visible ;
            App.BlahguaRest.FetchFullBlah(curBlah.BlahData.I, (theBlah) =>
                {
                    App.BlahguaRest.GetUserDescription(theBlah.A, (theDesc) =>
                        {
                            theBlah.Description = theDesc;
                            OpenFullBlah(theBlah);
                        });
                });

        }

        void OpenFullBlah(Blah theBlah)
        {
            if (theBlah != null)
            {
                // openthe page
                if (blahPage == null)
                    blahPage = new BlahViewer();
                LayoutRoot.Children.Add(blahPage);
                Canvas.SetZIndex(blahPage, 10);
                blahPage.CurrentBlah = theBlah;

                SlideTransition trans = new SlideTransition { Mode = SlideTransitionMode.SlideUpFadeIn };
                ITransition transition = trans.GetTransition(blahPage);
                transition.Completed += delegate
                {
                    transition.Stop();

                };
                transition.Begin();
               
            }
            else
            {
                MessageBox.Show("Blah failed to load");
            }
        }

        public void ClosePage()
        {
            SlideTransition trans = new SlideTransition { Mode = SlideTransitionMode.SlideDownFadeOut };
            ITransition transition = trans.GetTransition(blahPage);
            transition.Completed += delegate
            {
                transition.Stop();
                LayoutRoot.Children.Remove(blahPage);
                Mask.Visibility = Visibility.Collapsed;
                scrollTimer.Start();
            };
            transition.Begin();
           
        }

        void DoServiceInited(bool didIt)
        {
            ChannelTitleBar.SelectionChanged += ChannelTitleBar_SelectionChanged;
            ChannelTitleBar.ItemsSource = App.BlahguaAPI.CurrentChannelList;
        }

        void ChannelTitleBar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentChannel = (Channel)e.AddedItems[0];
            curGroupId = CurrentChannel._id;
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

   




    }
}