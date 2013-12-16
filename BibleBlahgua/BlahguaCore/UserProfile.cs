using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;


namespace BibleBlahgua
{
    [DataContract]
    public class UserProfile : INotifyPropertyChanged
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
        public string B { get; set; } // gender

        [DataMember]
        public string C { get; set; } // dob

        [DataMember]
        public string D { get; set; } // race

        [DataMember]
        public string E { get; set; }  // income

        [DataMember]
        public string G { get; set; } // city

        [DataMember]
        public string H { get; set; } // state

        [DataMember]
        public string I { get; set; } // zipcode

        [DataMember]
        public string J { get; set; }  // country


        [DataMember]
        public string _id { get; set; }

        [DataMember]
        public DateTime c { get; set; }

        [DataMember]
        public DateTime u { get; set; }

        public static ProfileSchema Schema {get; set; }

        public UserProfile()
        {
            A = null;
            B = "-1";
            D = "-1";
            E = "-1";
            G = null;
            H = null;
            I = null;
            J = "-1";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                try
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
                catch (Exception exp)
                {
                    // do nothing for now...
                }
            }
        }

        public string Nickname
        {
            get { return A; }
            set
            {
                A = value;
                OnPropertyChanged("Nickname");
            }
        }


        public bool GenderPerm
        {
            get { return Gender_perm > 0; }
            set 
            {
                Gender_perm = value ? 2 : 0;
                OnPropertyChanged("GenderPerm");
            }
        }

        public bool DOBPerm
        {
            get { return DOB_perm > 0; }
            set
            {
                DOB_perm = value ? 2 : 0;
                OnPropertyChanged("DOBPerm");
            }
        }

        public bool IncomePerm
        {
            get { return Income_perm > 0; }
            set
            {
                Income_perm = value ? 2 : 0;
                OnPropertyChanged("IncomePerm");
            }
        }

        public bool GPSPerm
        {
            get { return GPS_perm > 0; }
            set
            {
                GPS_perm = value ? 2 : 0;
                OnPropertyChanged("GPSPerm");
            }
        }

        public bool CityPerm
        {
            get { return City_perm > 0; }
            set
            {
                City_perm = value ? 2 : 0;
                OnPropertyChanged("CityPerm");
            }
        }

        public bool StatePerm
        {
            get { return State_perm > 0; }
            set
            {
                State_perm = value ? 2 : 0;
                OnPropertyChanged("StatePerm");
            }
        }

        public bool ZipcodePerm
        {
            get { return Zipcode_perm > 0; }
            set
            {
                Zipcode_perm = value ? 2 : 0;
                OnPropertyChanged("ZipcodePerm");
            }
        }

        public bool CountryPerm
        {
            get { return Country_perm > 0; }
            set
            {
                Country_perm = value ? 2 : 0;
                OnPropertyChanged("CountryPerm");
            }
        }

        public bool RacePerm
        {
            get { return Race_perm > 0; }
            set
            {
                Race_perm = value ? 2 : 0;
                OnPropertyChanged("RacePerm");
            }
        }
        

        public String Gender
        {
            get 
            {
                if (B != null)
                    return Schema.B.DT[B];
                else
                    return Schema.B.DT["-1"];
            }
            set
            {
                if (value != Gender)
                {
                    B = Schema.B.GetKeyForValue(value);
                    OnPropertyChanged("Gender");
                }
            }
        }

        public String Income
        {
            get
            {
                if (E != null)
                    return Schema.E.DT[E];
                else
                    return Schema.E.DT["-1"];
            }
            set
            {
                if (value != Income)
                {
                    E = Schema.E.GetKeyForValue(value);
                    OnPropertyChanged("Income");
                }
            }
        }

        public String Country
        {
            get
            {
                if (J != null)
                    return Schema.J.DT[J];
                else
                    return Schema.J.DT["-1"];
            }
            set
            {
                if (value != Country)
                {
                    J = Schema.J.GetKeyForValue(value);
                    OnPropertyChanged("Country");
                }
            }
        }


        public String Race
        {
            get
            {
                if (D != null)
                    return Schema.D.DT[D];
                else
                    return Schema.D.DT["-1"];
            }
            set
            {
                if (value != Race)
                {
                    D = Schema.D.GetKeyForValue(value);
                    OnPropertyChanged("Race");
                }
            }
        }

       

        public String City
        {
            get 
            {
                if (G != null)
                    return G;
                else
                    return "";
            }
            set 
            {
                if (G != value)
                {
                    G = value;
                    OnPropertyChanged("City");
                }
            }
        }

        public String State
        {
            get
            {
                if (H != null)
                    return H;
                else
                    return "";
            }
            set 
            {
                if (H != value)
                {
                    H = value;
                    OnPropertyChanged("State");
                }
            }
        }

        public String Zipcode
        {
            get
            {
                if (I != null)
                    return I;
                else
                    return "";
            }
            set 
            {
                if (I != value)
                {
                    I = value;
                    OnPropertyChanged("Zipcode");
                }
            }
        }

        public DateTime? DOB
        {
            get 
            {
                if (C != null)
                    return DateTime.Parse(C);
                else
                    return null;
            }
            set 
            {
                string newDate = ((DateTime)value).ToString("yyyy-MM-dd");
                if (C != newDate)
                {
                    C = newDate;
                    OnPropertyChanged("DOB");
                }
            }
        }
    }
}
