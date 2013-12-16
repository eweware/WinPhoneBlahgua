using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace BibleBlahgua
{

    public class BadgeRecord
    {
        public string A { get; set; } // authority ID - aka endpoint
        public string D { get; set; } // authority display name
        public string I { get; set; } // authorty badge id
        public string N { get; set; } // badge display name
        public string U { get; set; } // user id
        public string X { get; set; } // expiration date (jscript ticks)
        public string Y { get; set; }  // badge type
        public string _id { get; set; } // badge ID
        public DateTime c { get; set; } // created
        public DateTime u { get; set; }  // updated

        public BadgeRecord()
        {
        }

        public DateTime ExpirationDate
        {
            get
            {
                return new DateTime(1970, 1, 1).AddTicks(Convert.ToInt64(X) * 10000);
            }
        }

    }

    public class BadgeReference
    {
        public string BadgeName { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime CreationDate { get; set; }
        public string AuthorityEndpoint { get; set; }
        public string AuthorityDisplayName { get; set; }

        private string badgeId = "";

        public BadgeReference()
        {
            BadgeName = "";
        }

        public BadgeReference(string theID)
        {
            BadgeName = "";
            ID = theID;
        }

        public string ID
        {
            get { return badgeId; }
            set
            {
                badgeId = value;
                UpdateBadge();
            }
        }

        public string BadgeImage
        {
            get
            {
                return "/Images/badge_standalone_30x31.png";
            }
        }

        public void UpdateBadge()
        {
            App.BlahguaAPI.GetBadgeInfo(ID, (theBadge) =>
                {
                    BadgeName = theBadge.N;
                    CreationDate = theBadge.c;
                    ExpirationDate = theBadge.ExpirationDate;
                    AuthorityDisplayName = theBadge.D;
                    AuthorityEndpoint = theBadge.A;

                }
            );
        }
    }

    public class BadgeList : ObservableCollection<BadgeReference>
    {
    }
    public class BadgeAuthority
    {
        public string _id { get; set; }
        public DateTime c { get; set; }
        public string N { get; set; } // the name
        public string T { get; set; } // types of badges
        public string D { get; set; }  // description
        public string E { get; set; }  // home page
        public string R { get; set; }  // access point
    }

    public class BadgeAuthorityList : List<BadgeAuthority>
    {
        
    }
}
