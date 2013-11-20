using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Text;
using RestSharp;
using Microsoft.Phone.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;


namespace WinPhoneBlahgua
{
    public delegate void ChannelList_callback(ChannelList theList);
    public delegate void ChannelTypeList_callback(ChannelTypeList theList);
    public delegate void Inbox_callback(Inbox theList);
    public delegate void BlahTypes_callback(BlahTypeList theList);
    public delegate void Blah_callback(Blah theBlah);
    public delegate void Comment_callback(Comment theBlah);
    public delegate void UserDescription_callback(UserDescription theDesc);
    public delegate void Comments_callback(CommentList theList);
    public delegate void Blahs_callback(BlahList theList);
    public delegate void CommentAuthorDescriptionList_callback(CommentAuthorDescriptionList theList);
    public delegate void string_callback(String theResult);
    public delegate void User_callback(User theResult);
    public delegate void BadgeAuthorities_callback(BadgeAuthorityList theResult);
    public delegate void int_callback(int theResult);
    public delegate void BadgeRecord_callback(BadgeRecord theResult);
    public delegate void PredictionVote_callback(UserPredictionVote theResult);
    public delegate void PollVote_callback(UserPollVote theResult);  
    public delegate void Stats_callback(Stats theResult);
    public delegate void UserInfo_callback(UserInfoObject theResult);
    public delegate void ProfileSchema_callback(ProfileSchema theResult);
    public delegate void ProfileSchemaWrapper_callback(ProfileSchemaWrapper theResult);
    public delegate void Profile_callback(UserProfile theResult);
    public delegate void bool_callback(bool theResult);


    public class BlahguaRESTservice
    {
        public Dictionary<string, string> groupNames = null;
        public Dictionary<string, string> userGroupNames = null;
        public Dictionary<string, string> blahTypes = null;
        private bool usingQA = false;
        private RestClient apiClient;
        private string imageBaseURL = "";


        public BlahguaRESTservice()
        {
#if DEBUG
            usingQA = true;
#endif
            if (usingQA)
            {
                System.Console.WriteLine("Using QA Server");
                apiClient = new RestClient("http://qa.rest.blahgua.com:8080/v2");
                imageBaseURL = "https://s3-us-west-2.amazonaws.com/qa.blahguaimages/image/";
            }
            else
            {
                System.Console.WriteLine("Using Production Server");
                apiClient = new RestClient("https://beta.blahgua.com/v2");
                imageBaseURL = "https://s3-us-west-2.amazonaws.com/blahguaimages/image/";
            }

            apiClient.CookieContainer = new CookieContainer();
           
        }



        public string ImageBaseURL
        {
            get { return imageBaseURL; }
        }

        public void GetBlahComments(string blahId, Comments_callback callback)
        {
            RestRequest request = new RestRequest("comments", Method.GET);
            request.AddParameter("blahId", blahId);
            apiClient.ExecuteAsync(request, (response) =>
            {
                CommentList commentList = null;

                DataContractJsonSerializer des = new DataContractJsonSerializer(typeof(CommentList));
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                object theObj = des.ReadObject(stream);
                stream.Close();

                if (theObj != null)
                    commentList = (CommentList)theObj;

                callback(commentList);
            });
        }

        public void GetUserStatsInfo(DateTime startDate, DateTime endDate, UserInfo_callback callback)
        {
            string startDateString = Utilities.CreateDateString(startDate, false);
            string endDateString = Utilities.CreateDateString(endDate, false);

            RestRequest request = new RestRequest("users/info", Method.GET);
            request.AddParameter("stats", "true");
            request.AddParameter("s", startDateString);
            request.AddParameter("e", endDateString);

            apiClient.ExecuteAsync(request, (response) =>
            {
                UserInfoObject infoObj = null;

                DataContractJsonSerializer des = new DataContractJsonSerializer(typeof(UserInfoObject));
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                object theObj = des.ReadObject(stream);
                stream.Close();

                if (theObj != null)
                    infoObj = (UserInfoObject)theObj;

                callback(infoObj);
            });

        }

        public void GetBadgeAuthorities(BadgeAuthorities_callback callback)
        {
            RestRequest request = new RestRequest("badges/authorities", Method.GET);
            apiClient.ExecuteAsync<BadgeAuthorityList>(request, (response) =>
            {
                callback(response.Data);
            });
        }


        public void RecordImpressions(Dictionary<string, int> impressionMap, int_callback callback)
        {
            RestRequest request = new RestRequest("blahs/counts", Method.PUT);
            string jsonString = "";
            foreach (string curKey in impressionMap.Keys)
            {
                if (jsonString != "")
                    jsonString += ", ";
                jsonString += "\"" + curKey + "\": " + impressionMap[curKey].ToString();
            }

            jsonString  = "{\"V\":{" + jsonString + "}}";
            request.AddParameter("application/json", jsonString, ParameterType.RequestBody); 
            apiClient.ExecuteAsync(request, (response) =>
            {
                if (callback != null)
                    callback(impressionMap.Count);
            });
        }

        public void AddBlahOpen(string blahId)
        {
            RestRequest request = new RestRequest("blahs/" + blahId + "/stats", Method.PUT);
            string jsonString = "{\"O\": 1}";
           
            request.AddParameter("application/json", jsonString, ParameterType.RequestBody);
            apiClient.ExecuteAsync(request, (response) =>
            {
               
            });
        }

        public void CreateBadgeForUser(string authorityId, string badgeTypeId, string_callback callback)
        {
            RestRequest request = new RestRequest("badges", Method.POST);
            request.AddHeader("Accept", "*/*");
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { I = authorityId });
          
            apiClient.ExecuteAsync(request, (response) =>
            {
                callback(response.Content);
            });


        }

        

        public void GetBlahWithStats(string blahId, DateTime startDate, DateTime endDate, Blah_callback callback)
        {
            string startDateString = Utilities.CreateDateString(startDate, false);
            string endDateString = Utilities.CreateDateString(endDate, false);

            RestRequest request = new RestRequest("blahs/" + blahId, Method.GET);
            request.AddParameter("stats", "true");
            request.AddParameter("s", startDateString);
            request.AddParameter("e", endDateString);

            apiClient.ExecuteAsync(request, (response) =>
            {
                Blah theBlah = null;
                DataContractJsonSerializer des = new DataContractJsonSerializer(typeof(Blah));
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                object theObj = des.ReadObject(stream);
                stream.Close();

                if (theObj != null)
                    theBlah = (Blah)theObj;

                callback(theBlah);
            });




        }

        public void GetUserComments(string userId, Comments_callback callback)
        {
            RestRequest request = new RestRequest("comments", Method.GET);
            request.AddParameter("authorId", userId);
            apiClient.ExecuteAsync(request, (response) =>
            {
                CommentList commentList = null;

                DataContractJsonSerializer des = new DataContractJsonSerializer(typeof(CommentList));
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                object theObj = des.ReadObject(stream);
                stream.Close();

                if (theObj != null)
                    commentList = (CommentList)theObj;

                callback(commentList);
            });
        }

        public void GetUserBlahs(Blahs_callback callback)
        {
            RestRequest request = new RestRequest("blahs", Method.GET);
            apiClient.ExecuteAsync(request, (response) =>
            {
                BlahList blahList = null;
                DataContractJsonSerializer des = new DataContractJsonSerializer(typeof(BlahList));
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                object theObj = des.ReadObject(stream);
                stream.Close();

                if (theObj != null)
                    blahList = (BlahList)theObj;

                callback(blahList);
            });
        }

        public void AddUserToChannel(string channelId, string_callback callback)
        {
            RestRequest request = new RestRequest("userGroups", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { G = channelId });

            apiClient.ExecuteAsync(request, (response) =>
            {
                callback(response.Content);
            });
        }
               
        public void GetPublicChannels(ChannelList_callback callback)
        {
            RestRequest request = new RestRequest("groups/featured", Method.GET);
            apiClient.ExecuteAsync<ChannelList>(request, (response) =>
            {
                if (response.Data != null)
                {
                    ChannelList newList = new ChannelList();
                    foreach (Channel curChan in response.Data)
                    {
                        if (curChan.R > 0)
                            newList.Add(curChan);
                    }

                    newList.Sort((obj1, obj2) =>
                    {
                        return obj1.R.CompareTo(obj2.R);
                    });

                    callback(newList);
                }
                else
                    callback(null);
            });

        }

        public void GetProfileSchema(ProfileSchema_callback callback)
        {
            RestRequest request = new RestRequest("users/profile/schema", Method.GET);
            apiClient.ExecuteAsync<ProfileSchemaWrapper>(request, (response) =>
            {
                callback(response.Data.fieldNameToSpecMap);    
            });

        }

        public void GetUserProfile(Profile_callback callback)
        {
            RestRequest request = new RestRequest("users/profile/info", Method.GET);
            apiClient.ExecuteAsync(request, (response) =>
            {
                UserProfile profile = null;
                DataContractJsonSerializer des = new DataContractJsonSerializer(typeof(UserProfile));
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                object theObj = des.ReadObject(stream);
                stream.Close();

                if (theObj != null)
                    profile = (UserProfile)theObj;

                callback(profile);
            });

        }

        public void UpdateUserName(string userName, string_callback callback)
        {
            RestRequest request = new RestRequest("users/profile/info", Method.PUT);

            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { A = userName});

            apiClient.ExecuteAsync(request, (response) =>
            {
                callback("ok");
            });

        }

        public void DeleteUserImage(string_callback callback)
        {
            RestRequest request = new RestRequest("users/images", Method.DELETE);


            apiClient.ExecuteAsync(request, (response) =>
            {
                callback("ok");
            });

        }



        public void UpdateUserProfile(UserProfile theProfile, string_callback callback)
        {
            RestRequest request = new RestRequest("users/profile/info", Method.PUT);
            MemoryStream ms = new MemoryStream();
 
            // Serializer the User object to the stream.
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(UserProfile));
            ser.WriteObject(ms, theProfile);
            byte[] json = ms.ToArray();
            ms.Close();
            string dataStr = Encoding.UTF8.GetString(json, 0, json.Length);
            request.AddParameter("application/json", dataStr, ParameterType.RequestBody); 

            apiClient.ExecuteAsync(request, (response) =>
            {
                callback("ok");
            });

        }

        public void GetUserChannels(ChannelList_callback callback)
        {
            RestRequest request = new RestRequest("userGroups", Method.GET);
            apiClient.ExecuteAsync<ChannelList>(request, (response) =>
                {
                    if (response.Data != null)
                    {
                        ChannelList newList = response.Data;
                        newList.Sort((obj1, obj2) =>
                        {
                            return Math.Abs(obj1.R).CompareTo(Math.Abs(obj2.R));
                        });

                        callback(newList);
                    } 
                    else
                        callback(null);
                });

        }

        public void SignIn(string userName, string passWord, string_callback callback)
        {
            RestRequest request = new RestRequest("users/login", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { N = userName, pwd = passWord });

            apiClient.ExecuteAsync(request, (response) =>
            {
                if (response.Content == "")
                    callback("");
                else
                    callback(response.StatusDescription);
            });

        }

        public void CheckUserSignIn(bool_callback callback)
        {
            RestRequest request = new RestRequest("users/login/check", Method.GET);

            apiClient.ExecuteAsync<SigninStatus>(request, (response) =>
            {
                if ((response.Data != null) && (response.Data.loggedIn == "Y"))
                    callback(true);
                else
                    callback(false);
            });

        }

        public void SignOut(string_callback callback)
        {
            RestRequest request = new RestRequest("users/logout", Method.POST);
            request.RequestFormat = DataFormat.Json;
            apiClient.ExecuteAsync(request, (response) =>
            {
                callback(response.Content);
            });

        }

        public void Register(string userName, string passWord, string_callback callback)
        {
            RestRequest request = new RestRequest("users", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { N = userName, pwd = passWord });
            apiClient.ExecuteAsync<User>(request, (response) =>
            {
                if (response.Data == null)
                    callback(response.StatusDescription);
                else
                    callback("");
            });

        }
        
        public void CreateBlah(BlahCreateRecord theBlah , Blah_callback callback)
        {
            RestRequest request = new RestRequest("blahs", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(theBlah);
            apiClient.ExecuteAsync(request, (response) =>
            {
                Blah newBlah = null;
                DataContractJsonSerializer des = new DataContractJsonSerializer(typeof(Blah));
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                object theObj = des.ReadObject(stream);
                stream.Close();

                if (theObj != null)
                    newBlah = (Blah)theObj;

                callback(newBlah);
            });
        }

        public void CreateComment(CommentCreateRecord theComment, Comment_callback callback)
        {
            RestRequest request = new RestRequest("comments", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(theComment);
            apiClient.ExecuteAsync<Comment>(request, (response) =>
            {
                callback(response.Data);
            });
        }

        public void SetBlahVote(string blahId, int userVote, int_callback callback)
        {
            RestRequest request = new RestRequest("blahs/" + blahId + "/stats", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { uv = userVote });
            apiClient.ExecuteAsync<int>(request, (response) =>
            {
                callback(userVote);
            });
        }

        public void SetCommentVote(string commentId, int userVote, int_callback callback)
        {
            RestRequest request = new RestRequest("comments/" + commentId, Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { C = userVote });
            apiClient.ExecuteAsync<int>(request, (response) =>
            {
                callback(userVote);
            });
        }


        public void UploadPhoto(Stream photoStream, string fileName, string_callback callback)
        {
            var request = new RestRequest("images/upload", Method.POST);
            request.AddHeader("Accept", "*/*");
            request.AlwaysMultipartFormData = true;
            request.AddParameter("objectType", "X");
            request.AddParameter("objectId", "");
            request.AddParameter("primary", "true");
            request.AddFile("file", ReadToEnd(photoStream), fileName, "image/jpeg");

            apiClient.ExecuteAsync(request, (response) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    callback(response.Content);
                }
                else
                {
                    //error ocured during upload
                    callback(null);
                }
            });

        }

        public void UploadObjectPhoto(string objectId, string objectType, Stream photoStream, string fileName, string_callback callback)
        {
            var request = new RestRequest("images/upload", Method.POST);
            request.AddHeader("Accept", "*/*");
            request.AlwaysMultipartFormData = true;
            request.AddParameter("objectType", objectType);
            request.AddParameter("objectId", objectId);
            request.AddParameter("primary", "true");
            request.AddFile("file", ReadToEnd(photoStream), fileName, "image/jpeg");

            apiClient.ExecuteAsync(request, (response) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    callback(response.Content);
                }
                else
                {
                    //error ocured during upload
                    callback(null);
                }
            });

        }

        //method for converting stream to byte[]
        public byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = stream.Position;
            stream.Position = 0;

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }


        public void GetUserInfo(User_callback callback)
        {
            RestRequest request = new RestRequest("users/info", Method.GET);
            apiClient.ExecuteAsync<User>(request, (response) =>
            {
                callback(response.Data);
            });
        }

        public void GetRecoveryEmail(string_callback callback)
        {
            RestRequest request = new RestRequest("users/account", Method.GET);
            apiClient.ExecuteAsync(request, (response) =>
            {
                callback(response.Content);
            });
        }

        public void SetRecoveryEmail(string newMail, string_callback callback)
        {
            RestRequest request = new RestRequest("users/account", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { E = newMail });

            apiClient.ExecuteAsync(request, (response) =>
            {
                callback(response.Content);
            });
        }

        public void UpdatePassword(string newPassword, string_callback callback)
        {
            RestRequest request = new RestRequest("users/update/password", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { P = newPassword });

            apiClient.ExecuteAsync(request, (response) =>
            {
                callback(response.Content);
            });
        }

        public void RecoverUser(string userName, string email, string_callback callback)
        {
            RestRequest request = new RestRequest("users/recover/user", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { U = userName, E = email });

            apiClient.ExecuteAsync(request, (response) =>
            {
                callback(response.Content);
            });
        }

        public void GetUserPollVote(string blahId, PollVote_callback callback)
        {
            RestRequest request = new RestRequest("blahs/" + blahId + "/pollVote", Method.GET);
            apiClient.ExecuteAsync<UserPollVote>(request, (response) =>
            {
                callback(response.Data);
            });
        }

        public void SetUserPollVote(string blahId, int theOption, PollVote_callback callback)
        {
            RestRequest request = new RestRequest("blahs/" + blahId + "/pollVote/" + theOption, Method.PUT);
            apiClient.ExecuteAsync(request, (response) =>
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    GetUserPollVote(blahId, callback);
                }
                else
                    callback(null);
            });
        }

        public void GetUserPredictionVote(string blahId, PredictionVote_callback callback)
        {
            RestRequest request = new RestRequest("blahs/" + blahId + "/predicts", Method.GET);
            apiClient.ExecuteAsync<UserPredictionVote>(request, (response) =>
            {
                callback(response.Data);
            });
        }

        public void SetUserPredictionVote(string blahId, string theVote, bool expired, PredictionVote_callback callback)
        {
            RestRequest request = new RestRequest("blahs/" + blahId + "/predicts", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            if (expired)
                request.AddBody(new { t = "post", v = theVote });
            else
                request.AddBody(new { t = "pre", v = theVote });
            apiClient.ExecuteAsync<UserPredictionVote>(request, (response) =>
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    GetUserPredictionVote(blahId, callback);
                }
                else
                    callback(null);
            });
        }


        public void GetUserDescription(string userId, UserDescription_callback callback)
        {
            RestRequest request = new RestRequest("users/descriptor", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { I = userId });
            apiClient.ExecuteAsync<UserDescription>(request, (response) =>
                {
                    callback(response.Data);
                });

        }

        public void GetCommentAuthorDescriptions(List<string> userIds, CommentAuthorDescriptionList_callback callback)
        {
            RestRequest request = new RestRequest("users/descriptors", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { IDS = userIds });
            apiClient.ExecuteAsync<CommentAuthorDescriptionList>(request, (response) =>
            {
                callback(response.Data);
            });

        }



        public void FetchFullBlah(string blahId, Blah_callback callback)
        {
            RestRequest request = new RestRequest("blahs/" + blahId, Method.GET);
            apiClient.ExecuteAsync(request, (response) =>
            {
                Blah newBlah = null;

                DataContractJsonSerializer des = new DataContractJsonSerializer(typeof (Blah));
  
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
                object theObj = des.ReadObject(stream);
                stream.Close();

                if (theObj != null)
                    newBlah = (Blah)theObj;

                callback(newBlah);
            });
        }

        public void GetBadgeInfo(string badgeId, BadgeRecord_callback callback)
        {
            RestRequest request = new RestRequest("badges/" + badgeId, Method.GET);
            apiClient.ExecuteAsync<BadgeRecord>(request, (response) =>
            {
                callback(response.Data);
            });
        }

       

        public void GetChannelTypes(ChannelTypeList_callback callback)
        {
            RestRequest request = new RestRequest("groupTypes", Method.GET);
            apiClient.ExecuteAsync<ChannelTypeList>(request, (response) =>
            {
                callback(response.Data);
            });

        }

        public void GetBlahTypes(BlahTypes_callback callback)
        {
            RestRequest request = new RestRequest("blahs/types", Method.GET);
            apiClient.ExecuteAsync<BlahTypeList>(request, (response) =>
            {
                callback(response.Data);
            });

        }

        public void GetInbox(string groupId, Inbox_callback callback)
        {
            RestRequest request = new RestRequest("users/inbox", Method.GET);
            request.AddParameter("groupId", groupId);
            apiClient.ExecuteAsync<Inbox>(request, (response) =>
            {
                callback(response.Data);
            });

        }

      

    }
}
