using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Phone.Tasks;
using System.IO.IsolatedStorage;

namespace WinPhoneBlahgua
{


    public class BlahguaAPIObject : INotifyPropertyChanged
    {
        class SavedUserInfo
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        ChannelList curChannelList = null;
        ChannelTypeList curChannelTypes = null;
        BlahTypeList blahTypeList = null;
        BlahguaRESTservice BlahguaRest = null;
        Blah currentBlah = null;
        User currentUser = null;
        Channel currentChannel = null;
        BlahCreateRecord createRec = null;
        CommentCreateRecord createCommentRec = null;
        public UserDescription CurrentUserDescription = null;


        public event PropertyChangedEventHandler PropertyChanged;
        public bool AutoLogin { set; get; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string UserPassword2 { get; set; }
        public bool NewAccount { get; set; }
        bool inited = false;
        public Blah NewBlahToInsert { get; set; }
        private Dictionary<string, string> intBadgeMap = new Dictionary<string,string>();

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
            NewBlahToInsert = null;
        }

        SavedUserInfo GetSavedUserInfo()
        {
            SavedUserInfo theInfo = new SavedUserInfo();
            theInfo.UserName = (string)SafeLoadSetting("username", "");
            theInfo.Password = (string)SafeLoadSetting("password", "");
            return theInfo;
        }

        void SaveUserInfo(string name, string password)
        {
            SafeSaveSetting("username", name);
            SafeSaveSetting("password", password);
        }

        void LoadSettings()
        {
            AutoLogin = (bool)SafeLoadSetting("AutoLogin", true);
        }

        void SaveSettings()
        {
            SafeSaveSetting("AutoLogin", AutoLogin);
        }

        public object SafeLoadSetting(string setting, object defVal)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains(setting))
                return settings[setting];
            else
            {
                settings.Add(setting, defVal);
                return defVal;
            }
        }

        public void SafeSaveSetting(string setting, object val)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains(setting))
                settings[setting] = val;
            else
            {
                settings.Add(setting, val);
            }
        }

        public void Initialize(BlahguaInit_callback callBack)
        {
            LoadSettings();
            if (!inited)
            {
                BlahguaRest.GetChannelTypes((cTypeList) =>
                {
                    curChannelTypes = cTypeList;

                    BlahguaRest.GetBlahTypes((bTypeList) =>
                    {
                        blahTypeList = bTypeList;
                        blahTypeList.Remove(blahTypeList.First(i => i.N == "ad"));

                        if (AutoLogin)
                        {
                            SavedUserInfo info = GetSavedUserInfo();
                            if (info.UserName != "")
                            {
                                SignIn(info.UserName, info.Password, true, (theErrStr) =>
                                    {
                                        if (theErrStr == null)
                                        {
                                            inited = true;
                                            callBack(true);
                                        }
                                        else
                                            CompletePublicSignin(callBack);
                                    }
                                );
                            }
                            else 
                                CompletePublicSignin(callBack);
                        }
                        else
                            CompletePublicSignin(callBack);  

                    });
                });
            }
            else
            {
                callBack(true);
            }
        }

        private void CompletePublicSignin(BlahguaInit_callback callback)
        {
            BlahguaRest.GetPublicChannels((chanList) =>
                           {
                               curChannelList = chanList;
                               CurrentChannel = curChannelList[0];
                               inited = true;
                               callback(true);
                           });
        }

        public void UploadPhoto(System.IO.Stream photo, string fileName, string_callback callback)
        {
            BlahguaRest.UploadPhoto(photo, fileName, (newPhotoId) =>
                {
                    callback(newPhotoId);
                }
            );
        }

        public void GetBadgeName(string badgeId, string_callback callback)
        {
            if (intBadgeMap.ContainsKey(badgeId))
                callback(intBadgeMap[badgeId]);
            else
            {
                BlahguaRest.GetBadgeInfo(badgeId, (theBadge) =>
                    {
                        string badgeName = theBadge.N;
                        intBadgeMap[badgeId] = badgeName;
                        callback(badgeName);
                    }
                );
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

        public CommentCreateRecord CreateCommentRecord
        {
            get { return createCommentRec; }
            set
            {
                createCommentRec = value;
                OnPropertyChanged("CreateCommentRecord");
            }
        }

        public void CreateBlah(Blah_callback callback)
        {
            CreateRecord.G = CurrentChannel.ChannelId;
            if (CreateRecord.BlahType.N == "polls")
            {
                CreateRecord.H = CreateRecord.I.Count;
            }
            else
                CreateRecord.I = null;

            BlahguaRest.CreateBlah(CreateRecord, (theBlah) =>
                {
                    CreateRecord = new BlahCreateRecord();
                    callback(theBlah);
                }
                );

        }

        public void CreateComment(Comment_callback callback)
        {
            CreateCommentRecord.B = CurrentBlah._id;

            BlahguaRest.CreateComment(CreateCommentRecord, (theComment) =>
            {
                CreateCommentRecord = new CommentCreateRecord();
                callback(theComment);
            }
                );

        }

        public void EnsureUserDescription(string_callback callback)
        {
            if (CurrentUserDescription == null)
            {
                BlahguaRest.GetUserDescription(CurrentUser._id, (theDesc) =>
                    {
                        CurrentUserDescription = theDesc;
                        callback(CurrentUserDescription.d);
                    }
                );
            }
            else
            {
                callback( CurrentUser.DescriptionString);
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
                if (theBlah != null)
                {
                    BlahguaRest.GetUserDescription(theBlah.A, (theDesc) =>
                    {
                        theBlah.Description = theDesc;
                        currentBlah = theBlah;
                        callback(theBlah);
                    });
                }
                else
                    callback(null);
            });
        }

        public void SetBlahVote(int newVote, int_callback callback)
        {
            BlahguaRest.SetBlahVote(CurrentBlah._id, newVote, callback);
        }

        public void SetCommentVote(Comment theComment, int newVote, int_callback callback)
        {
            BlahguaRest.SetCommentVote(theComment._id, newVote, callback);
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


        public void GetUserPollVote(PollVote_callback callback)
        {
            if (!CurrentBlah.IsPollInited)
            {
                if (CurrentUser != null)
                {
                    BlahguaRest.GetUserPollVote(CurrentBlah._id, (theVote) =>
                        {
                            CurrentBlah.UpdateUserPollVote(theVote);
                            callback(theVote);
                        }
                        );
                }
                else
                {
                    CurrentBlah.UpdateUserPollVote(null);
                    callback(null);
                }
            }
            else
                callback(null);
        }

        public void SetUserPollVote(int theOption, PollVote_callback callback)
        {
            BlahguaRest.SetUserPollVote(CurrentBlah._id, theOption, callback);
        }

        public void LoadBlahStats(Stats_callback callback)
        {
            if (CurrentBlah.L != null)
                callback(CurrentBlah.L);
            else
            {
                DateTime endDate = DateTime.Now;
                DateTime startDate = endDate - new TimeSpan(7, 0, 0, 0);
                BlahguaRest.GetBlahWithStats(CurrentBlah._id, startDate, endDate, (blahWithStats) =>
                {
                    if (blahWithStats != null)
                        CurrentBlah.L = blahWithStats.L;
                    else
                        CurrentBlah.L = null;

                    callback(CurrentBlah.L);

                }
                );

            }
        }

        public void GetUserPredictionVote(PredictionVote_callback callback)
        {
            if (!CurrentBlah.IsPredictInited)
            {
                if (CurrentUser != null)
                {
                    BlahguaRest.GetUserPredictionVote(CurrentBlah._id, (theVote) =>
                    {
                        CurrentBlah.UpdateUserPredictionVote(theVote);
                        callback(theVote);
                    }
                        );
                }
                else
                {
                    CurrentBlah.UpdateUserPredictionVote(null);
                    callback(null);
                }
            }
            else
                callback(null);
        }

        public void SetUserPredictionVote(string userVote, PredictionVote_callback callback)
        {
            BlahguaRest.SetUserPredictionVote(CurrentBlah._id, userVote, false, callback);
        }

        public void SetUserExpPredictionVote(string userVote, PredictionVote_callback callback)
        {
            BlahguaRest.SetUserPredictionVote(CurrentBlah._id, userVote, true, callback);
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
                        if (saveIt)
                        {
                            AutoLogin = true;
                            SaveUserInfo(userName, password);
                            SaveSettings();
                        }
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
                    CurrentChannelList = chanList;
                    CurrentChannel = curChannelList[0];

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
