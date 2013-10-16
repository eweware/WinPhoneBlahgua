using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Text;
using RestSharp;


namespace WinPhoneBlahgua
{
    public class BlahguaRESTservice
    {
        public Dictionary<string, string> groupNames = null;
        public Dictionary<string, string> userGroupNames = null;
        public Dictionary<string, string> blahTypes = null;
        private CookieContainer sessionCookie = null;
        private bool usingQA = false;
        private RestClient apiClient;
        private string imageBaseURL = "";


        public BlahguaRESTservice()
        {
            if (usingQA)
            {
                apiClient = new RestClient("http://qa.rest.blahgua.com:8080/v2");
                imageBaseURL = "https://s3-us-west-2.amazonaws.com/qa.blahguaimages/image/";
            }
            else
            {
                apiClient = new RestClient("https://beta.blahgua.com/v2");
                imageBaseURL = "https://s3-us-west-2.amazonaws.com/blahguaimages/image/";
            }
        }

        public delegate void ChannelList_callback(ChannelList theList);
        public delegate void ChannelTypeList_callback(ChannelTypeList theList);
        public delegate void Inbox_callback(Inbox theList);
        public delegate void BlahTypes_callback(BlahTypeList theList);
        public delegate void Blah_callback(Blah theBlah);
        public delegate void UserDescription_callback(UserDescription theDesc);


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

        public void GetUserDescription(string userId, UserDescription_callback callback)
        {
            RestRequest request = new RestRequest("users/descriptor", Method.POST);
            request.RequestFormat = DataFormat.Json;
            //request.AddParameter("I", userId);
            request.AddBody(new { I = userId });
            apiClient.ExecuteAsync<UserDescription>(request, (response) =>
                {
                    callback(response.Data);
                });

        }



        public void FetchFullBlah(string blahId, Blah_callback callback)
        {
            RestRequest request = new RestRequest("blahs/" + blahId, Method.GET);
            apiClient.ExecuteAsync<Blah>(request, (response) =>
            {
                callback(response.Data);
            });
        }

        public string GetImageURL(string baseURL, string size)
        {
            string fullURL;
            fullURL = imageBaseURL + baseURL + "-" + size + ".jpg";

            return fullURL;
        }

        public string GetImageURL(string baseURL)
        {
            string fullURL;
            fullURL = imageBaseURL + baseURL + ".jpg";

            return fullURL;
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



        /*

        public bool AddFileToObject(string objectId, string fName, string objType)
        {
            string fileName = DefaultFolderPath + "\\" + fName;
            if (File.Exists(fileName))
            {
                NameValueCollection nvc = new NameValueCollection();
                nvc.Add("objectType", objType);
                nvc.Add("primary", "true");
                nvc.Add("objectId", objectId);

                if (ImportToQA)
                    HttpUploadFile("http://qa.rest.blahgua.com:8080/v2/images/upload", fileName, "file", "application/octet-stream", nvc);
                else
                    HttpUploadFile("https://beta.blahgua.com/v2/images/upload", fileName, "file", "application/octet-stream", nvc);
                return true;
            }
            else
                return false;
        }

        private void HttpUploadFile(string url, string file, string paramName, string contentType, NameValueCollection nvc)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.CookieContainer = sessionCookie;
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

            Stream rs = wr.GetRequestStream();

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, paramName, file, contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                //log.Debug(string.Format("File uploaded, server response is: {0}", reader2.ReadToEnd()));
                wresp.Close();
                wresp.Dispose();
                wresp = null;
            }
            catch (Exception ex)
            {
                //log.Error("Error uploading file", ex);
                if (wresp != null)
                {
                    wresp.Close();
                    wresp.Dispose();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }
        }
         */

    }
}
