using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;


namespace WinPhoneBlahgua
{
    [DataContract]
    public class UserProfile
    {
        [DataMember(Name = "0")]
        public int Nickname_perm { get; set; }

        [DataMember(Name = "1")]
        public int Gender_perm { get; set; }

        [DataMember(Name = "2")]
        public int DOB_perm { get; set; }

        [DataMember(Name = "3")]
        public int Race_perm { get; set; }

        [DataMember(Name = "4")]
        public int Income_perm { get; set; }

        [DataMember(Name = "5")]
        public int GPS_perm { get; set; }

        [DataMember(Name = "6")]
        public int City_perm { get; set; }

        [DataMember(Name = "7")]
        public int State_perm { get; set; }

        [DataMember(Name = "8")]
        public int Zipcode_perm { get; set; }

        [DataMember(Name = "9")]
        public int Country_perm { get; set; }

        [DataMember]
        public string A { get; set; }

        [DataMember]
        public int B { get; set; } // gender

        [DataMember]
        public DateTime C { get; set; } // dob

        [DataMember]
        public int D { get; set; } // race

        [DataMember]
        public int E { get; set; }  // income

        [DataMember]
        public string G { get; set; } // city

        [DataMember]
        public string H { get; set; } // state

        [DataMember]
        public string I { get; set; } // zipcode

        [DataMember]
        public string F { get; set; } // GPS Location

        [DataMember]
        public string J { get; set; }  // country

        [DataMember]
        public string _id { get; set; }

        [DataMember]
        public DateTime c { get; set; }

        [DataMember]
        public DateTime u { get; set; }
    }
}
