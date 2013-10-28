using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace WinPhoneBlahgua
{
    public class BadgeRecord
    {
        public string N {get; set;}

        public BadgeRecord() 
        { 
        }

    }

    public class BadgeReference
    {
        string badgeId = "";
        public string BadgeName {get; set;}

        public BadgeReference()
        {
            
        }

        public BadgeReference(string theID)
        {
            ID = theID;
        }

        public string ID
        {
            get { return badgeId; }
            set
            {
                badgeId = value;
                UpdateBadgeName();
            }
        }

        private void UpdateBadgeName()
        {
            App.BlahguaAPI.GetBadgeName(ID, (theName) =>
                {
                    BadgeName = theName;
                }
            );
        }

      
    }

    public class BadgeList : ObservableCollection<BadgeReference>
    {
    }


    public class InboxBlah
    {
        public string I { get; set; }
        public long c { get; set; }
        public string T { get; set; }
        public string Y { get; set; }
        public string G { get; set; }
        public string A { get; set; }
        public List<string> M { get; set; }
        public string B { get; set; }
        public double S { get; set; }
        public int displaySize { get; set; }

        public InboxBlah()
        {
        }

        public InboxBlah(InboxBlah otherBlah)
        {
            I = otherBlah.I;
            c = otherBlah.c;
            T = otherBlah.T;
            Y = otherBlah.Y;
            G = otherBlah.G;
            A = otherBlah.A;
            M = otherBlah.M;
            B = otherBlah.B;
            S = otherBlah.S;
            displaySize = otherBlah.displaySize;
        }

        public InboxBlah(Blah otherBlah)
        {
            c = otherBlah.c.ToJavaScriptMilliseconds();
            I = otherBlah._id;
            T = otherBlah.T;
            Y = otherBlah.Y;
            G = otherBlah.G;
            A = otherBlah.A;
            M = otherBlah.M;
            if (otherBlah.B != null)
                B = "B";
            displaySize = 2;
        }

        public DateTime Created
        {
            get
            {
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                origin = System.TimeZoneInfo.ConvertTime(origin, TimeZoneInfo.Local);
                return origin.AddSeconds(c / 1000);
            }
        }

        public string ImageSize
        {
            get
            {
                switch (displaySize)
                {
                    case 1:
                        return "C";

                    case 2:
                        return "B";
     
                    default:
                        return "A";
                      
                }
            }
        }

    }

    public class Inbox : List<InboxBlah>
    {
        public InboxBlah PopBlah(int blahSize)
        {
            foreach (InboxBlah curBlah in this)
            {
                if (curBlah.displaySize == blahSize)
                {
                    this.Remove(curBlah);
                    return curBlah;
                }
            }
            return null;

        }

        private void ComputeSizes()
        {
            int numLarge = 4;
            int numMedium = 16;

            this.Sort((obj1, obj2) =>
            {
                return obj1.S.CompareTo(obj2.S);
            });


            int i = 0;
            while (i < numLarge)
            {
                this[i++].displaySize = 1;
            }

            while (i < (numMedium + numLarge))
            {
                this[i++].displaySize = 2;
            }

            while (i < this.Count)
            {
                this[i++].displaySize = 3;
            }
        }

        private void Shuffle()
        {
        }

        private void EnsureInboxSize()
        {
            int curIndex = 0;

            while (this.Count < 100)
            {
                this.Add(new InboxBlah(this[curIndex++]));
            }
        }

        public void PrepareBlahs()
        {
            EnsureInboxSize();
            ComputeSizes();
            Shuffle();
        }


    }


    public class BlahType
    {
        public string _id { get; set; }
        public string N { get; set; }
        public string c { get; set; }
        public int C { get; set; }
    }


    public class BlahTypeList : List<BlahType> 
    {
        public string GetTypeName(string typeId)
        {
            return this.First(i => i._id == typeId).N;
        }
    }

    [DataContract]
    public class PollItem
    {
        [DataMember]
        public string G {get; set;}

        [DataMember]
        public string T {get; set;}

        private int _maxVotes = 0;
        private int _totalVotes = 0;
        private int _itemVotes = 0;
        private bool _isUserVote = false;


        public PollItem(string theText)
        {
            G = theText;
        }

        public PollItem(string theText, int numVotes, int maxVotes, int totalVotes, bool isUserVote)
        {
            G = theText;
            _maxVotes = maxVotes;
            _itemVotes = numVotes;
            _isUserVote = isUserVote;
        }

        public int MaxVotes
        {
            get { return _maxVotes; }
            set { _maxVotes = value; }
        }

        public int TotalVotes
        {
            get { return _totalVotes; }
            set { _totalVotes = value; }
        }

        public double ComputedWidth
        {
            get 
            { 
                double votes = 0;
                if (_maxVotes > 0)
                votes = 360.0 * ((double)_itemVotes / (double)_maxVotes);
                return Math.Max(5, votes); 
            }
        }

        public string VotePercent
        {
            get 
            {
                int percent = 0;

                if (_totalVotes > 0)
                    percent = (int)(((double)_itemVotes / (double)_totalVotes) * 100);

                if (percent > 0)
                    return percent.ToString() + "%";
                else
                    return "no votes"; // no votes
            }
        }

        public string VoteString
        {
            get
            {
                if (_isUserVote)
                    return "\uf046";
                else
                    return "\uf096";
            }
        }


        public int Votes
        {
            get { return _itemVotes; }
            set { _itemVotes = value; }
        }

        public bool IsUserVote
        {
            get { return _isUserVote; }
            set { _isUserVote = value; }
        }
    }

    public class PollItemList : ObservableCollection<PollItem>
    {
    }

    public class BlahCreateRecord
    {
        public List<string> B { get; set; }
        public string F { get; set; }
        public DateTime E { get; set; } // expiration date
        public string G { get; set; } // group ID
        public List<string> M { get; set; } // image IDs
        public int H { get; set; } // poll option count
        public PollItemList I { get; set; } // poll text
        public string T { get; set; } // blah text
        public string Y { get; set; } // type ID
        public bool XX { get; set; } // wehter or not the blah is private
       

        public BlahCreateRecord()
        {
            XX = true;
            Y = App.BlahguaAPI.CurrentBlahTypes.First<BlahType>(n => n.N == "says")._id;
            E = DateTime.Now + new TimeSpan(30, 0, 0, 0);

            I = new PollItemList();
            I.Add(new PollItem("first choice"));
            I.Add(new PollItem("second choice"));
        }

        public bool UseProfile
        {
            get { return !XX; }
            set
            {
                XX = (!value);
            }
        }


        public BlahType BlahType
        {
            get 
            {
                return App.BlahguaAPI.CurrentBlahTypes.First<BlahType>(n => n._id == Y);
            }
            set
            {
                Y = value._id;
            }
        }

        public string UserImage
        {
            get
            {
                if (XX)
                {
                    return "/Images/unknown-user.png";    
                }
                else
                {
                    return App.BlahguaAPI.CurrentUser.UserImage;
                }
            }
        }

        public string UserName
        {
            get
            {
                if (XX)
                {
                    return "Someone";
                }
                else
                {
                    return App.BlahguaAPI.CurrentUser.N;
                }
            }
        }

        public string UserDescriptionString
        {
            get
            {
                if (XX)
                {
                    return "An anonymous person";
                }
                else
                {
                    return App.BlahguaAPI.CurrentUser.DescriptionString;
                }
            }
        }

        public BadgeList Badges
        {
            get
            {
                if (B == null)
                {
                    return null;
                }
                else
                {
                    BadgeList badges = new BadgeList();
                    foreach (string badgeId in B)
                    {
                        badges.Add(new BadgeReference(badgeId));
                    }

                    return badges;
                }
            }
            set
            {
                if ((value == null) || (value.Count == 0))
                {
                    B = null;
                }
                else
                {
                    B = new List<string>();
                    foreach (BadgeReference curBadge in value)
                    {
                        B.Add(curBadge.ID);
                    }
                }
            }
        }
    }

    public class StatDayRecord
    {
        public int C { get; set; }
        public int D { get; set; }
        public int O { get; set; }
        public int P { get; set; }
        public int U { get; set; }
        public int V { get; set; }
        public string _id { get; set; }

        public StatDayRecord()
        {
            C = D = O = P = U = V = 0;
        }

        public StatDayRecord(DateTime theDate)
        {
            string timeStr = theDate.ToString("yyMMdd");
            _id = timeStr;
            C = D = O = P = U = V = 0;
        }

        public DateTime StatDate
        {
            get
            {
                string datePart = _id.Substring(_id.Length - 6);
                string statYear = datePart.Substring(0, 2);
                string statMonth = datePart.Substring(2, 2);
                string statDay = datePart.Substring(4, 2);
                return new DateTime(2000 + int.Parse(statYear), int.Parse(statMonth), int.Parse(statDay));
            }
        }
    }

    public class Stats : ObservableCollection<StatDayRecord>
    {

        List<int> _openList = null;
        List<int> _commentList = null;
        List<int> _viewList = null;

        bool _hasComments;
        bool _hasViews;
        bool _hasOpens;

        public List<int> Opens
        {
            get
            {
                if (_openList == null)
                {
                    bool hasVotes = false;
                    int curVote;
                    _openList = new List<int>();
                    foreach (StatDayRecord dayRec in this)
                    {
                        curVote = dayRec.O;
                        if (curVote > 0)
                            hasVotes = true;
                        _openList.Add(curVote);
                    }

                    _hasOpens = hasVotes;
                }


                return _openList;
            }
        }

        public List<int> Impressions
        {
            get
            {
                if (_viewList == null)
                {
                    bool hasVotes = false;
                    int curVote;
                    _viewList = new List<int>();
                    foreach (StatDayRecord dayRec in this)
                    {
                        curVote = dayRec.V;
                        if (curVote > 0)
                            hasVotes = true;
                        _viewList.Add(curVote);
                    }

                    _hasViews = hasVotes;
                }


                return _viewList;
            }
        }

        public List<int> Comments
        {
            get
            {
                if (_commentList == null)
                {
                    bool hasVotes = false;
                    int curVote;
                    _commentList = new List<int>();
                    foreach (StatDayRecord dayRec in this)
                    {
                        curVote = dayRec.O;
                        if (curVote > 0)
                            hasVotes = true;
                        _commentList.Add(curVote);
                    }

                    _hasComments = hasVotes;
                }


                return _commentList;
            }
        }

        public bool HasOpens
        {
            get { return _hasOpens; }
        }

        public bool HasViews
        {
            get { return _hasViews; }
        }

        public bool HasComments
        {
            get { return _hasComments; }
        }
    }

    public class DemographicRecord
    {
        public DemoProfileSummaryRecord _o { get; set; }
        public DemoProfileSummaryRecord _b { get; set; }
        public DemoProfileSummaryRecord _c { get; set; }
        public DemoProfileSummaryRecord _d { get; set; }
        public DemoProfileSummaryRecord _u { get; set; }
        public DemoProfileSummaryRecord _v { get; set; }
    }

    public class DemoProfileSummaryRecord
    {
        public DemoDictionary J { get; set; }
        public DemoDictionary C { get; set; }
        public DemoDictionary B { get; set; }
        public DemoDictionary E { get; set; }
        public DemoDictionary D { get; set; }
        public DemoDictionary Z { get; set; }
    }

    [DataContract]
    public class DemoDictionary
    {
        [DataMember(Name = "0")]
        public int val_0 { get; set; }
        [DataMember(Name = "1")]
        public int val_1 { get; set; }
        [DataMember(Name = "2")]
        public int val_2 { get; set; }
        [DataMember(Name = "3")]
        public int val_3 { get; set; }
        [DataMember(Name = "4")]
        public int val_4 { get; set; }
        [DataMember(Name = "5")]
        public int val_5 { get; set; }
        [DataMember(Name = "6")]
        public int val_6 { get; set; }
        [DataMember(Name = "7")]
        public int val_7 { get; set; }
        [DataMember(Name = "8")]
        public int val_8 { get; set; }
        [DataMember(Name = "9")]
        public int val_90 { get; set; }
        [DataMember(Name = "-1")]
        public int val_Unknown { get; set; }
    }

    public class UserPollVote
    {
        public int W {get; set;}
    }

    public class UserPredictionVote
    {
        public string D {get; set;}
        public string Z {get; set;}
    }

    [DataContract]
    public class Blah
    {
        [DataMember]
        public string A { get; set; }

        [DataMember]        
        public string F { get; set; }

        [DataMember]
        public string G { get; set; }

        [DataMember]
        public int O { get; set; }

        [DataMember]
        public double S { get; set; }

        [DataMember]
        public string T { get; set; }

        [DataMember]
        public int V { get; set; }

        [DataMember]
        public string Y { get; set; }

        [DataMember]
        public string _id { get; set; }

        [DataMember]
        public DateTime c { get; set; }

        [DataMember]
        public DateTime u { get; set; }

        [DataMember]
        public List<string> B { get; set; }

        [DataMember]
        public List<string> M { get; set; }

        [DataMember]
        public PollItemList I { get; set; }

        [DataMember]
        public List<int> J { get; set; }

        [DataMember]
        public DateTime E { get; set; }

        [DataMember(Name = "1")]
        public int _1 { get; set; }

        [DataMember(Name = "2")]
        public int _2 { get; set; }

        [DataMember(Name = "3")]
        public int _3 { get; set; }

        [DataMember(Name = "4")]
        public int _4 { get; set; }

        [DataMember(Name = "5")]
        public int _5 { get; set; }

        [DataMember(Name = "6")]
        public int _6 { get; set; }

        [DataMember]
        public bool XX { get; set; }

        [DataMember]
        public DemographicRecord _d { get; set; }

        [DataMember]
        public Stats L { get; set; }

        [DataMember]
        public int uv { get; set; }

        [DataMember]
        public int P { get; set; }

        [DataMember]
        public int D { get; set; }

        [DataMember]
        public int C { get; set; }


        public bool IsPollInited = false;
        public bool IsPredictInited = false;
        public UserDescription Description {get; set;}
        public CommentList Comments { get; set; }
        private PollItemList _predictionItems;
        private PollItemList _expPredictionItems;



        public Blah()
        {
            uv = 0;
            O = 0;
            V = 0;
            C = 0;
            D = 0;
            P = 0;
            _1 = 0;
            _2 = 0;
            _3 = 0;
            _4 = 0;
            _5 = 0;
            _6 = 0;
            L = null;
        }

        public string ConversionString
        {
            get
            {
                double theRate = 0;

                if (V > 0)
                    theRate = (double)O / (double)V;

                return theRate.ToString("p2");
            }
        }

        public string ImpressionString
        {
            get
            {
                string theStr = "opened " + O + " time";
                if (O != 1)
                    theStr += "s";
                theStr += " out of " + V + " impression";
                if (V != 1)
                    theStr += "s";
                return theStr;
            }
        }

        public PollItemList PredictionItems
        {
            get
            {
                return _predictionItems;
            }
        }

        public PollItemList ExpPredictionItems
        {
            get
            {
                return _expPredictionItems;
            }
        }

        public void UpdateUserPredictionVote(UserPredictionVote theVote)
        {
            int totalExpVotes = _1 + _2 + _3;
            int maxExpVote = Math.Max(Math.Max(_1, _2), _3);
            int totalVotes = _4 + _5 + _6;
            int maxVote = Math.Max(Math.Max(_4, _5), _6);
            _predictionItems = new PollItemList();
            _predictionItems.Add(new PollItem("I agree", _4, maxVote, totalVotes, false));
            _predictionItems.Add(new PollItem("I disagree", _5, maxVote, totalVotes, false));
            _predictionItems.Add(new PollItem("It is unclear", _6, maxVote, totalVotes, false));

            _expPredictionItems = new PollItemList();
            _expPredictionItems.Add(new PollItem("The prediction was right", _1, maxExpVote, totalExpVotes, false));
            _expPredictionItems.Add(new PollItem("The prediction was wrong", _2, maxExpVote, totalExpVotes, false));
            _expPredictionItems.Add(new PollItem("It is unclear", _3, maxExpVote, totalExpVotes, false));


            if (theVote == null)
            {
                // user is not signed in
            }
            else
            {
                switch (theVote.D)
                {
                    case "y":
                        _predictionItems[0].IsUserVote = true;
                        break;
                    case "n":
                        _predictionItems[1].IsUserVote = true;
                        break;
                    case "u":
                        _predictionItems[2].IsUserVote = true;
                        break;
                }

                switch (theVote.Z)
                {
                    case "y":
                        _expPredictionItems[0].IsUserVote = true;
                        break;
                    case "n":
                        _expPredictionItems[1].IsUserVote = true;
                        break;
                    case "u":
                        _expPredictionItems[2].IsUserVote = true;
                        break;
                }
            }

            IsPredictInited = true;
        }

        public void UpdateUserPollVote(UserPollVote theVote)
        {
            int maxVote = 0;
            int userVote = -1;
            int totalVotes = 0;

            if (theVote != null)
                userVote = theVote.W;

            foreach (int curVote in J)
            {
                totalVotes += curVote;
                if (curVote > maxVote)
                    maxVote = curVote;
            }
            PollItem curPollItem = null;

            for (int i = 0; i < I.Count; i++)
            {
                curPollItem = I[i];
                curPollItem.MaxVotes = maxVote;
                curPollItem.Votes = J[i];
                curPollItem.IsUserVote = (userVote == i);
                curPollItem.TotalVotes = totalVotes;
            }

            IsPollInited = true;
        }
  

        public string ElapsedTimeString
        {
            get
            {
                return Utilities.ElapsedDateString(c);
            }
        }

        
  
        public string UserName
        {
            get
            {
                if ((!XX) && (Description != null) && (Description.K != null))
                    return Description.K;
                else
                    return "Someone";
            }
        }

        public string UserDescriptionString
        {
            get
            {
                if ((!XX) && (Description != null) && (Description.d != null))
                    return Description.d;
                else
                    return "An anonymous user";
            }
        }

        public string UserImage
        {
            get
            {
                if ((!XX) && (Description != null) && (Description.m != null))
                    return App.BlahguaAPI.GetImageURL(Description.m, "A");
                else
                    return "/Images/unknown-user.png";
            }
        }

        public string ImageURL
        {
            get
            {
                if (M != null)
                {
                    string imageName = M[0];
                    return App.BlahguaAPI.GetImageURL(M[0], "D");
                }
                else
                    return null;
            }

        }

        public string TypeName
        {
            get
            {
                string typeName = App.BlahguaAPI.CurrentBlahTypes.GetTypeName(Y);
                return typeName;
            }
        }


        public string ChannelName
        {
            get
            {
                string channelName = App.BlahguaAPI.CurrentChannelList.ChannelName(G);
                return channelName;
            }
        }

        
    }


}
