using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Reflection;

namespace WinPhoneBlahgua
{

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
        public CountryDemoDictionary J { get; set; }
        public StandardDemoDictionary C { get; set; }
        public StandardDemoDictionary B { get; set; }
        public StandardDemoDictionary E { get; set; }
        public StandardDemoDictionary D { get; set; }
        public StandardDemoDictionary Z { get; set; }

        public int GetPropertyValue(string propName, string valName)
        {
            PropertyInfo property = GetType().GetProperty(propName);
            if (property != null)
            {
                object propVal = property.GetValue(this, null);
                if (propVal != null)
                    return ((DemoDictionary)propVal).GetValue(valName);
                else
                    return 0;
            }
            else
                return 0;

        }
    }

    [DataContract]
    public abstract class DemoDictionary
    {

        public abstract int GetValue(string valName);
        


    }

   [DataContract]
    public class CountryDemoDictionary : DemoDictionary
    {
       [DataMember]
        public int AU { get; set; }
        [DataMember]
        public int BR { get; set; }
        [DataMember]
        public int CA { get; set; }
       [DataMember]
        public int CN { get; set; }
       [DataMember]
        public int JP { get; set; }
       [DataMember]
        public int SG { get; set; }
       [DataMember]
        public int KR { get; set; }
       [DataMember]
        public int TW { get; set; }
       [DataMember]
        public int TH { get; set; }
       [DataMember]
        public int GB { get; set; }
       [DataMember]
        public int US { get; set; }
        [DataMember(Name = "-1")]
        public int val_Unknown { get; set; }

        public override int GetValue(string propName)
        {
            if (propName == "-1")
                propName = "val_Unknown";

            PropertyInfo property = GetType().GetProperty(propName);
            if (property != null)
            {
                object propVal = property.GetValue(this, null);
                if (propVal != null)
                    return (int)propVal;
                else
                    return 0;
            }
            else
                return 0;
        }

    }


    [DataContract]
    public class StandardDemoDictionary : DemoDictionary 
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

        public override int GetValue(string valName)
        {
            if (valName == "-1")
                valName = "Unknown";

            string propName = "val_" + valName;
            PropertyInfo property = GetType().GetProperty(propName);
            if (property != null)
            {
                object propVal = property.GetValue(this, null);
                if (propVal != null)
                    return (int)propVal;
                else
                    return 0;
            }
            else
                return 0;
        }


    }

    public class SpecMap
    {
        public Dictionary<string, string> DT { get; set; }
        public string E { get; set; }
    }

    public class ProfileSchema
    {
        public SpecMap B { get; set; } // gender;
        public SpecMap D { get; set; } // race
        public SpecMap E { get; set; } // Income
        public SpecMap J { get; set; } // country
        public SpecMap C { get; set; } // age


        public Dictionary<string, string> GetTypesForProperty(string propName)
        {
            PropertyInfo theProp = GetType().GetProperty(propName);
            if (theProp == null)
                return null;
            else
                return  ((SpecMap)theProp.GetValue(this, null)).DT;
        }

        public List<string> GenderChoices
        {
            get
            {
                List<string> choices = new List<string>();

                foreach (string curKey in B.DT.Values)
                {
                    choices.Add(curKey);
                }

                return choices;
            }

        }

        public List<string> RaceChoices
        {
            get
            {
                List<string> choices = new List<string>();
                foreach (string curKey in D.DT.Values)
                {
                    choices.Add(curKey);
                }

                return choices;
            }

        }

        public List<string> IncomeChoices
        {
            get
            {
                List<string> choices = new List<string>();
                foreach (string curKey in E.DT.Values)
                {
                    choices.Add(curKey);
                }

                return choices;
            }

        }

        public List<string> CountryChoices
        {
            get
            {
                List<string> choices = new List<string>();
                foreach (string curKey in J.DT.Values)
                {
                    choices.Add(curKey);
                }

                return choices;
            }

        }

        
    }

    public class ProfileSchemaWrapper
    {
        public ProfileSchema fieldNameToSpecMap { get; set; } 
    }

    public class UserDailyStatRecord
    {
        public int VS { get; set; }
        public int C { get; set; }
        public int DT { get; set; }
        public int O { get; set; }
        public int XO { get; set; }
        public int U { get; set; }
        public int T { get; set; }
        public int dd { get; set; }
        public int V { get; set; }
        public int uu { get; set; }
        public int DD { get; set; }
        public int OC { get; set; }
        public int X { get; set; }
        public int UU { get; set; }
        public int d { get; set; }
        public int oo { get; set; }
        public int XX { get; set; }
        public int OS { get; set; }
        public int o { get; set; }
        public int OO { get; set; }
        public int vv { get; set; }
        public int v { get; set; }
        public int u { get; set; }
        public int t { get; set; }
        public int tt { get; set; }
        public int VV { get; set; }
        public int TT { get; set; }

        public int GetPropertyValue(string propName, int defaultVal = 0)
        {
            PropertyInfo property = GetType().GetProperty(propName);
            if (property != null)
            {
                object propVal = property.GetValue(this, null);
                if (propVal != null)
                    return (int)propVal;
                else
                    return defaultVal;
            }
            else
                return defaultVal;

        }
    }


    public class UserStatRecord
    {
        public double AC { get; set; }
        public double ADD { get; set; }
        public double ADT { get; set; }
        public double AO { get; set; }
        public double AOC { get; set; }
        public double AOO { get; set; }
        public double AOS { get; set; }
        public double AT { get; set; }
        public double ATT { get; set; }
        public double AU { get; set; }
        public double AUU { get; set; }
        public double AV { get; set; }
        public double AVS { get; set; }
        public double AVV { get; set; }
        public double AX { get; set; }
        public double AXO { get; set; }
        public double AXX { get; set; }
        public double Ad { get; set; }
        public double Add { get; set; }
        public double Ao { get; set; }
        public double Aoo { get; set; }
        public double At { get; set; }
        public double Att { get; set; }
        public double Au { get; set; }
        public double Auu { get; set; }
        public double Av { get; set; }
        public double Avv { get; set; }
        public int C { get; set; }
        public int CM { get; set; }
        public int DD { get; set; }
        public int DT { get; set; }
        public int O { get; set; }
        public double OAC { get; set; }
        public double OADD { get; set; }
        public double OADT { get; set; }
        public double OAO { get; set; }
        public double OAOC { get; set; }
        public double OAOO { get; set; }
        public double OAOS { get; set; }
        public double OAT { get; set; }
        public double OATT { get; set; }
        public double OAU { get; set; }
        public double OAUU { get; set; }
        public double OAV { get; set; }
        public double OAVS { get; set; }
        public double OAVV { get; set; }
        public double OAX { get; set; }
        public double OAXO { get; set; }
        public double OAXX { get; set; }
        public double OAd { get; set; }
        public double OAdd { get; set; }
        public double OAo { get; set; }
        public double OAoo { get; set; }
        public double OAt { get; set; }
        public double OAtt { get; set; }
        public double OAu { get; set; }
        public double OAuu { get; set; }
        public double OAv { get; set; }
        public double OAvv { get; set; }
        public int OC { get; set; }
        public int OO { get; set; }
        public int OS { get; set; }
        public double OSC { get; set; }
        public double OSDD { get; set; }
        public double OSDT { get; set; }
        public double OSO { get; set; }
        public double OSOC { get; set; }
        public double OSOO { get; set; }
        public double OSOS { get; set; }
        public double OST { get; set; }
        public double OSTT { get; set; }
        public double OSU { get; set; }
        public double OSUU { get; set; }
        public double OSV { get; set; }
        public double OSVS { get; set; }
        public double OSVV { get; set; }
        public double OSX { get; set; }
        public double OSXO { get; set; }
        public double OSXX { get; set; }
        public double OSd { get; set; }
        public double OSdd { get; set; }
        public double OSo { get; set; }
        public double OSoo { get; set; }
        public double OSt { get; set; }
        public double OStt { get; set; }
        public double OSu { get; set; }
        public double OSuu { get; set; }
        public double OSv { get; set; }
        public double OSvv { get; set; }
        public int S { get; set; }
        public double SC { get; set; }
        public double SDD { get; set; }
        public double SDT { get; set; }
        public int SM { get; set; }
        public double SO { get; set; }
        public double SOC { get; set; }
        public double SOO { get; set; }
        public double SOS { get; set; }
        public int SS { get; set; }
        public double ST { get; set; }
        public double STT { get; set; }
        public double SU { get; set; }
        public double SUU { get; set; }
        public double SV { get; set; }
        public double SVS { get; set; }
        public double SVV { get; set; }
        public double SX { get; set; }
        public double SXO { get; set; }
        public double SXX { get; set; }
        public double Sd { get; set; }
        public double Sdd { get; set; }
        public double So { get; set; }
        public double Soo { get; set; }
        public double St { get; set; }
        public double Stt { get; set; }
        public double Su { get; set; }
        public double Suu { get; set; }
        public double Sv { get; set; }
        public double Svv { get; set; }
        public int T { get; set; }
        public int TT { get; set; }
        public int U { get; set; }
        public int UU { get; set; }
        public int V { get; set; }
        public int VS { get; set; }
        public int VV { get; set; }
        public int X { get; set; }
        public int XO { get; set; }
        public int XX { get; set; }
        public string _id { get; set; }
        public int d { get; set; }
        public int dd { get; set; }
        public List<UserDailyStatRecord> dy { get; set; }
        public string id { get; set; }
        public int n { get; set; }
        public int o { get; set; }
        public int oo { get; set; }
        public int t { get; set; }
        public int tt { get; set; }
        public int u { get; set; }
        public int uu { get; set; }
        public int v { get; set; }
        public int vv { get; set; }
    }


    public class UserInfoObject
    {
        public List<string> B { get; set; }
        public double K { get; set; }
        public List<string> M { get; set; }
        public string N { get; set; }
        public double S { get; set; }
        public DemographicRecord _d { get; set; }
        /* public D1 _d1 { get; set; } */ // we don't care about recent  
        public string _id { get; set; }
        public string c { get; set; }
        public string u { get; set; }
        public List<UserStatRecord> L { get; set; }

        private DateTime _startDate;
        private DateTime _endDate;


        public void SetDateRange(DateTime start, DateTime end)
        {
            _startDate = start;
            _endDate = end;
        }

        public DateTime StartDate
        {
            get { return _startDate; }
        }

        public int GetStatForDate(int whichDay, string statName)
        {
            DateTime theDate = StartDate;
            theDate += new TimeSpan(whichDay, 0, 0, 0);
            return GetStatForDate(theDate, statName);
        }

        public int GetStatForDate(DateTime theDate, string statName)
        {
            string dateString = Utilities.CreateDateString(theDate, true);

            int theDay = theDate.Day - 1;

            if (L != null)
            {
                foreach (UserStatRecord curMonth in L)
                {
                    if (curMonth._id.Substring(curMonth._id.Length - 4) == dateString)
                    {
                        // we have the month... now get the day
                        return curMonth.dy[theDay].GetPropertyValue(statName, 0);
                    }
                }

                return 0;
            }
            else
                return 0;

        }

        public bool HasUserViews
        {
            get
            {
                return true;
            }
        }

        public bool HasUserOpens
        {
            get
            {
                return true;
            }
        }

        public bool HasUserCreates
        {
            get
            {
                return true;
            }
        }

        public bool HasUserComments
        {
            get
            {
                return true;
            }
        }

        public bool HasViews
        {
            get
            {
                return true;
            }
        }

        public bool HasOpens
        {
            get
            {
                return true;
            }
        }

        public bool HasComments
        {
            get
            {
                return true;
            }
        }

        public int DayCount
        {
            get
            {
                TimeSpan elapsedTime = _endDate - _startDate;
                return elapsedTime.Days;
            }
        }

        public DateTime StatDate(int whichDay)
        {
            DateTime theDate = StartDate;
            theDate += new TimeSpan(whichDay, 0, 0, 0);

            return theDate;
        }

        public int UserViews(int whichDay)
        {
            return GetStatForDate(whichDay, "V") + GetStatForDate(whichDay, "v");
        }

        public int UserOpens(int whichDay)
        {
            return GetStatForDate(whichDay, "O") + GetStatForDate(whichDay, "o");
        }

        public int UserCreates(int whichDay)
        {
            return GetStatForDate(whichDay, "X");
        }

        public int UserComments(int whichDay)
        {
            return GetStatForDate(whichDay, "XX");
        }

        public int Views(int whichDay)
        {
            return GetStatForDate(whichDay, "V");
        }

        public int Opens(int whichDay)
        {
            return GetStatForDate(whichDay, "O");
        }

        public int Comments(int whichDay)
        {
            return GetStatForDate(whichDay, "C");
        }

    }
}
