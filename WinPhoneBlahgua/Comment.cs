using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinPhoneBlahgua
{
    public class Comment
    {
        public string _id { get; set; }
        public string B { get; set; }
        public string T { get; set; }
        public string A { get; set; }
        public double S { get; set; }
        public string c { get; set; }
        public List<string> BD { get; set; }
        public string CID { get; set; }
        public bool? XX { get; set; }
        public int? C { get; set; }
        public int? U { get; set; }
        public string u { get; set; }
    }

    public class CommentList : List<Comment>
    {

    }
}
