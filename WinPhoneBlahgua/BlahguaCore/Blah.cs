using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

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


    public class BlahCreateRecord
    {
        public List<string> B { get; set; }
        public string F { get; set; }
        public DateTime E { get; set; } // expiration date
        public string G { get; set; } // group ID
        public List<string> M { get; set; } // image IDs
        public int H { get; set; } // poll option count
        public List<string> I { get; set; } // poll text
        public string T { get; set; } // blah text
        public string Y { get; set; } // type ID
        public bool XX { get; set; } // wehter or not the blah is private
       

        public BlahCreateRecord()
        {
            XX = true;
            Y = App.BlahguaAPI.CurrentBlahTypes.First<BlahType>(n => n.N == "says")._id;
            E = DateTime.Now + new TimeSpan(30, 0, 0, 0);

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

    public class Blah
    {
        public string A { get; set; }
        public string F { get; set; }
        public string G { get; set; }
        public int O { get; set; }
        public double S { get; set; }
        public string T { get; set; }
        public int V { get; set; }
        public string Y { get; set; }
        public string _id { get; set; }
        public DateTime c { get; set; }
        public DateTime u { get; set; }
        public List<string> B { get; set; }
        public List<string> M { get; set; }
        public List<string> I { get; set; }
        public bool XX { get; set; }
        public DateTime E { get; set; }


        public UserDescription Description {get; set;}
        public CommentList Comments { get; set; }



        public Blah()
        {

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
