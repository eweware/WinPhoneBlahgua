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
        public DateTime C { get; set; } // dob

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
        public string F { get; set; } // GPS Location

        [DataMember]
        public string J { get; set; }  // country

        [DataMember]
        public string _id { get; set; }

        [DataMember]
        public DateTime c { get; set; }

        [DataMember]
        public DateTime u { get; set; }

        public static ProfileSchema Schema {get; set; }

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
        

        public String Gender
        {
            get 
            { 
                return Schema.B.DT[B];
            }
            set
            {

                B = Schema.B.GetKeyForValue(value);
                OnPropertyChanged("Gender");
            }
        }

        public String Income
        {
            get
            {
                return Schema.E.DT[E];
            }
            set
            {
                E = Schema.E.GetKeyForValue(value);
                OnPropertyChanged("Income");
            }
        }

        public String Country
        {
            get
            {
                return Schema.J.DT[J];
            }
            set
            {
                J = Schema.J.GetKeyForValue(value);
                OnPropertyChanged("Country");
            }
        }


        public String Race
        {
            get
            {
                return Schema.D.DT[D];
            }
            set
            {
                D = Schema.D.GetKeyForValue(value);
                OnPropertyChanged("Race");
            }
        }

        public String GPSLocation
        {
            get { return F; }
            set 
            { 
                F = value;
                OnPropertyChanged("GPSLocation");
            }
        }

        public String City
        {
            get { return G; }
            set 
            { 
                G = value;
                OnPropertyChanged("City");
            }
        }

        public String State
        {
            get { return H; }
            set 
            { 
                H = value;
                OnPropertyChanged("State");
            }
        }

        public String Zipcode
        {
            get { return I; }
            set 
            { 
                I = value;
                OnPropertyChanged("Zipcode");
            }
        }

        public DateTime DOB
        {
            get { return C; }
            set 
            { 
                C = value;
                OnPropertyChanged("DOB");
            }
        }
    }
}
