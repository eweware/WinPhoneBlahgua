using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinPhoneBlahgua
{
    public class Channel
    {
        public int B { get; set; }
        public string D { get; set; }
        public int F { get; set; }
        public string G { get; set; }
        public int I { get; set; }
        public int L { get; set; }
        public string M { get; set; }
        public string N { get; set; }
        public int R { get; set; }
        public string S { get; set; }
        public int U { get; set; }
        public int V { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
        public string _id { get; set; }
        public string c { get; set; }
        public string u { get; set; }


        public Channel()
        {

        }

        public string ChannelName
        {
            get { return N; }
        }

        public string ChannelId
        {
            get { return _id; }
        }

        public string ChannelTypeId
        {
            get { return Y; }
        }

        public string ChannelTypeName
        {
            get
            {
                return App.BlahguaAPI.CurrentChannelTypeList.ChannelTypeName(Y);
            }
        }

        public string ChannelImageUrl
        {
            get 
            {
                string baseURL = "https://s3-us-west-2.amazonaws.com/beta.blahgua.com/images/groups/";
                return baseURL + ChannelName + ".png";
                return "https://";
            }
        }
    }

    public class ChannelList : List<Channel>
    {
        public string ChannelName(string channelId)
        {
            return this.Where(i => i._id == channelId).FirstOrDefault().N;
        }
    }

    public class ChannelType
    {
        public string _id { get; set; }
        public string N { get; set; }
        public string c { get; set; }

    }

    public class ChannelTypeList : List<ChannelType>
    {
        public string ChannelTypeName(string channelId)
        {
            return this.Where(i => i._id == channelId).FirstOrDefault().N;
        }
    }

}
