using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinPhoneBlahgua
{
    public class User
    {
        public List<string> M { get; set; }
        public List<string> B { get; set; }
        public string N { get; set; }
        public double S;
        public string _id { get; set; }
        public DateTime c { get; set; }
        public DateTime u { get; set; }

        public string UserImage
        {
            get
            {
                if (M != null)
                    return App.BlahguaAPI.GetImageURL(M[0], "A");
                else
                    return "Images\\unknown-user.png";
            }
        }
    }

    public class UserDescription
    {
        public string K { get; set; }
        public string i { get; set; }
        public string d { get; set; }
        public string m { get; set; }
    }

    public class CommentAuthorDescription
    {
        public string K { get; set; }
        public string d { get; set; }
        public string i { get; set; }
        public List<string> _m { get; set; }
    }

    public class CommentAuthorDescriptionList : List<CommentAuthorDescription>
    {
    }
}
