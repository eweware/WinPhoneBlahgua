using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace WinPhoneBlahgua
{
    public class Comment
    {
        public string _id { get; set; }
        public string B { get; set; }
        public string T { get; set; }
        public string A { get; set; }
        public double S { get; set; }
        public DateTime c { get; set; }
        public List<string> BD { get; set; }
        public string CID { get; set; }
        public string XX { get; set; }
        public string U { get; set; }
        public string D { get; set; }

        public DateTime u { get; set; }
        public List<string> M { get; set; }
        public CommentList subComments = null;
        public string K { get; set; }
        public string d { get; set; }
        public List<string> _m { get; set; }

        public string AuthorName
        {
            get
            {
                if (K != null)
                    return K;
                else
                    return "Someone";
            }
        }

        public string AuthorImage
        {
            get
            {
                if (_m != null)
                {
                    string imageName = _m[0];
                    return App.BlahguaAPI.GetImageURL(_m[0], "A");
                }
                else
                    return "/Images/unknown-user.png";
            }
        }

        public int UpVoteCount
        {
            get
            {
                int vote = 0;
                if (U != null)
                    vote = int.Parse(U);

                return vote;
            }
        }

        public int DownVoteCount
        {
            get
            {
                int vote = 0;
                if (D != null)
                    vote = int.Parse(D);

                return vote;
            }
        }

        public string UserDescriptionString
        {
            get
            {
                if (d != null)
                    return d;
                else
                    return "An anonymous user";
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

        public string ElapsedTimeString
        {
            get
            {
                return Utilities.ElapsedDateString(c);
            }
        }

    }



    public class CommentList : ObservableCollection<Comment>
    {

    }
}
