using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinPhoneBlahgua
{
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
