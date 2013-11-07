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
        public double S { get; set; }
        public double K { get; set; }
        public string _id { get; set; }
        public DateTime c { get; set; }
        public DateTime u { get; set; }

        private CommentList _commentHistory;
        private BlahList _postHistory;
        private UserInfoObject _info = null;



        public UserInfoObject UserInfo
        {
            get { return _info; }
            set { _info = value; }
        }

        public string AccountName
        {
            get
            {
                return N;
            }
            set
            {
                N = value;
            }
        }

        public string UserName
        {
            get
            {
                if (App.BlahguaAPI.CurrentUserDescription != null)
                    return App.BlahguaAPI.CurrentUserDescription.K;
                else
                    return "Someone";
            }
        }

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

        public string DescriptionString
        {
            get
            {
                if (App.BlahguaAPI.CurrentUserDescription != null)
                    return App.BlahguaAPI.CurrentUserDescription.d;
                else
                    return null;
            }
        }

        public CommentList CommentHistory
        {
            get  {  return _commentHistory; }
            set { _commentHistory = value; }
        }

        public BlahList PostHistory
        {
            get { return _postHistory; }
            set { _postHistory = value; }
        }

        public string UserDescriptionString
        {
            get { return DescriptionString; }
        }

        public BadgeList Badges
        {
            get
            {
                if (B == null)
                {
                    return null;
                }
                else
                {
                    BadgeList badges = new BadgeList();
                    foreach (string badgeId in B)
                    {
                        badges.Add(new BadgeReference(badgeId));
                    }

                    return badges;
                }
            }
            set
            {
                if ((value == null) || (value.Count == 0))
                {
                    B = null;
                }
                else
                {
                    B = new List<string>();
                    foreach (BadgeReference curBadge in value)
                    {
                        B.Add(curBadge.ID);
                    }
                }
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
