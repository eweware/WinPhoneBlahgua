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
        public DemoDictionary J { get; set; }
        public DemoDictionary C { get; set; }
        public DemoDictionary B { get; set; }
        public DemoDictionary E { get; set; }
        public DemoDictionary D { get; set; }
        public DemoDictionary Z { get; set; }

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

        public int GetValue(string valName)
        {
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
    }

    public class ProfileSchemaWrapper
    {
        public ProfileSchema fieldNameToSpecMap { get; set; } 
    }
}
