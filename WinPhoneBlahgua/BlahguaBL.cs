using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace WinPhoneBlahgua
{
    public class BlahguaAPIObject : INotifyPropertyChanged
    {
        ChannelList curChannelList = null;
        ChannelTypeList curChannelTypes = null;
        BlahTypeList blahTypeList = null;
        BlahguaRESTservice BlahguaRest = null;
        Blah currentBlah = null;
        User currentUser = null;
        Channel currentChannel = null;
        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void BlahguaInit_callback(bool didIt);

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public BlahguaAPIObject()
        {
            BlahguaRest = new BlahguaRESTservice();    
        }

        public void Initialize(BlahguaInit_callback callBack)
        {
            BlahguaRest.GetChannelTypes((cTypeList) =>
            {
                curChannelTypes = cTypeList;

                BlahguaRest.GetPublicChannels((chanList) =>
                {
                    BlahguaRest.GetBlahTypes((bTypeList) =>
                    {
                        blahTypeList = bTypeList;
                        curChannelList = chanList;
                        CurrentChannel = curChannelList[0];
                        callBack(true);
                    });
                });
            });
        }

        public ChannelList CurrentChannelList
        {
            get { return curChannelList; }
        }

        public ChannelTypeList CurrentChannelTypeList
        {
            get { return curChannelTypes; }
        }

        public BlahTypeList CurrentBlahTypes
        {
            get { return blahTypeList; }
        }

         public string GetImageURL(string baseURL, string size)
        {
            string fullURL;
            fullURL = BlahguaRest.ImageBaseURL + baseURL + "-" + size + ".jpg";

            return fullURL;
        }
        
        public string GetImageURL(string baseURL)
        {
            string fullURL;
            fullURL = BlahguaRest.ImageBaseURL + baseURL + ".jpg";

            return fullURL;
        }

        public Blah CurrentBlah
        {
            get { return currentBlah; }
        }

        public User CurrentUser
        {
            get { return currentUser; }
        }

        public Channel CurrentChannel
        {
            get { return currentChannel; }
            set
            {
                if (currentChannel != value)
                {
                    currentChannel = value;
                    OnPropertyChanged("CurrentChannel");
                }
            }
        }

        public void SetCurrentBlahFromId(string blahId, Blah_callback callback)
        {
            BlahguaRest.FetchFullBlah(blahId, (theBlah) =>
            {
                BlahguaRest.GetUserDescription(theBlah.A, (theDesc) =>
                {
                    theBlah.Description = theDesc;
                    currentBlah = theBlah;
                    callback(theBlah);
                });
            });
        }

        public void GetInbox(Inbox_callback callback)
        {
            BlahguaRest.GetInbox(CurrentChannel._id, callback);
        }

    }
}
