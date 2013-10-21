using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace WinPhoneBlahgua
{


    public class BlahguaAPIObject : INotifyPropertyChanged
    {
        ChannelList curChannelList = null;
        ChannelTypeList curChannelTypes = null;
        BlahTypeList blahTypeList = null;
        BlahguaRESTservice BlahguaRest = null;
        Blah currentBlah = null;
        User currentUser = null;
        Channel currentChannel = null;
        BlahCreateRecord createRec = null;

        public event PropertyChangedEventHandler PropertyChanged;
        public bool AutoLogin { set; get; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string UserPassword2 { get; set; }
        public bool NewAccount { get; set; }
        bool inited = false;

        public delegate void BlahguaInit_callback(bool didIt);

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public BlahguaAPIObject()
        {
            BlahguaRest = new BlahguaRESTservice();    
        }

        public void Initialize(BlahguaInit_callback callBack)
        {
            if (!inited)
            {
                BlahguaRest.GetChannelTypes((cTypeList) =>
                {
                    curChannelTypes = cTypeList;

                    BlahguaRest.GetPublicChannels((chanList) =>
                    {
                        BlahguaRest.GetBlahTypes((bTypeList) =>
                        {
                            blahTypeList = bTypeList;
                            blahTypeList.Remove(blahTypeList.First(i => i.N == "ad"));
                            curChannelList = chanList;
                            CurrentChannel = curChannelList[0];
                            inited = true;
                            callBack(true);
                        });
                    });
                });
            }
            else
            {
                callBack(true);
            }
        }

        public ChannelList CurrentChannelList
        {
            get { return curChannelList; }
            set
            {
                curChannelList = value;
                OnPropertyChanged("CurrentChannelList");
            }
                
        }

        public BlahCreateRecord CreateRecord
        {
            get { return createRec; }
            set
            {
                createRec = value;
                OnPropertyChanged("CreateRecord");
            }
        }

        public ChannelTypeList CurrentChannelTypeList
        {
            get { return curChannelTypes; }
        }

        public BlahTypeList CurrentBlahTypes
        {
            get { return blahTypeList; }
        }

         public string GetImageURL(string baseURL, string size)
        {
            string fullURL;
            fullURL = BlahguaRest.ImageBaseURL + baseURL + "-" + size + ".jpg";

            return fullURL;
        }
        
        public string GetImageURL(string baseURL)
        {
            string fullURL;
            fullURL = BlahguaRest.ImageBaseURL + baseURL + ".jpg";

            return fullURL;
        }

        public Blah CurrentBlah
        {
            get { return currentBlah; }
        }

        public User CurrentUser
        {
            get { return currentUser; }
            set
            {
                currentUser = value;
                OnPropertyChanged("CurrentUser");
            }
        }

        public Channel CurrentChannel
        {
            get { return currentChannel; }
            set
            {
                if (currentChannel != value)
                {
                    currentChannel = value;
                    OnPropertyChanged("CurrentChannel");
                }
            }
        }

        public void GoPrevChannel()
        {
            int curIndex = curChannelList.IndexOf(currentChannel);
            curIndex--;
            if (curIndex < 0)
                curIndex = curChannelList.Count - 1;
            CurrentChannel = curChannelList[curIndex];
        }

        public void GoNextChannel()
        {
            int curIndex = curChannelList.IndexOf(currentChannel);
            curIndex++;
            if (curIndex >= curChannelList.Count)
                curIndex =  0;
            CurrentChannel = curChannelList[curIndex];
        }

        public void SetCurrentBlahFromId(string blahId, Blah_callback callback)
        {
            BlahguaRest.FetchFullBlah(blahId, (theBlah) =>
            {
                BlahguaRest.GetUserDescription(theBlah.A, (theDesc) =>
                {
                    theBlah.Description = theDesc;
                    currentBlah = theBlah;
                    callback(theBlah);
                });
            });
        }

        public void GetInbox(Inbox_callback callback)
        {
            BlahguaRest.GetInbox(CurrentChannel._id, callback);
        }

        public void LoadBlahComments(Comments_callback callback)
        {
            BlahguaRest.GetBlahComments(currentBlah._id, (comments) =>
                {
                    if (comments != null)
                    {
                        List<string> authorIds = GetCommentAuthorIds(comments);
                        BlahguaRest.GetCommentAuthorDescriptions(authorIds, (descList) =>
                            {
                                ApplyAuthorDescriptions(comments, descList);
                                comments = ThreadComments(comments);
                                currentBlah.Comments = comments;
                                callback(comments);

                            }
                        );
                    }
                   
                }
            );
        }

        void ApplyAuthorDescriptions(CommentList commList, CommentAuthorDescriptionList descList)
        {
            foreach (Comment curComm in commList)
            {
                foreach (CommentAuthorDescription curDesc in descList)
                {
                    if (curDesc.i == curComm.A)
                    {
                        curComm.K = curDesc.K;
                        curComm.d = curDesc.d;
                        curComm._m = curDesc._m;
                    }
                }
            }
        }

        List<string> GetCommentAuthorIds(CommentList theList)
        {
            List<string> authorList = new List<string>();

            foreach (Comment curComment in theList)
            {
                if (!authorList.Contains(curComment.A))
                {
                    authorList.Add(curComment.A);
                }
            }

            return authorList;
        }

  

        CommentList ThreadComments(CommentList comments)
        {
            CommentList threadedList = new CommentList();
            foreach (Comment curComment in comments)
            {
                if (curComment.CID != null)
                {
                    Comment parent = comments.First(comment => comment._id == curComment.CID);
                    if (parent.subComments == null)
                        parent.subComments = new CommentList();
                    parent.subComments.Add(curComment);
                }
                else
                {
                    threadedList.Add(curComment);
                }
            }
            return threadedList;
        }

        public void SignIn(string userName, string password, bool saveIt, string_callback callBack)
        {
            BlahguaRest.SignIn(userName, password, (resultStr) =>
                {
                    if (resultStr == "")
                    {
                        PrepareForNewUser(callBack);
                    }
                    else
                    {
                        callBack(resultStr);
                    }
                }
            );
        }

        public void Register(string userName, string password, bool saveIt, string_callback callBack)
        {
            BlahguaRest.Register(userName, password, (resultStr) =>
                {
                    if (resultStr == "")
                    {
                        SignIn(userName, password, saveIt, callBack);
                    }
                    else
                    {
                        callBack(resultStr);
                    }
                }
            );
        }

        void PrepareForNewUser(string_callback callBack)
        {
            BlahguaRest.GetUserChannels((chanList) =>
                {
                    CurrentChannel = curChannelList[0];
                    CurrentChannelList = chanList;
                    BlahguaRest.GetUserInfo((newUser) =>
                        {
                            CurrentUser = newUser;
                            callBack(null);
                        }
                    );
                    
                }
            );  
        }

    }
}
