using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinPhoneBlahgua
{
    public class  BlahguaBL
    {
        ChannelList curChannelList = null;
        ChannelTypeList curChannelTypes = null;
        BlahTypeList blahTypeList = null;

        public delegate void BlahguaInit_callback(bool didIt);

        public BlahguaBL()
        {
            
        }

        public void Initialize(BlahguaInit_callback callBack)
        {
            App.BlahguaRest.GetChannelTypes((cTypeList) =>
            {
                curChannelTypes = cTypeList;

                App.BlahguaRest.GetPublicChannels((chanList) =>
                {
                    curChannelList = chanList;
                    App.BlahguaRest.GetBlahTypes((bTypeList) =>
                    {
                        blahTypeList = bTypeList;
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


    }
}
