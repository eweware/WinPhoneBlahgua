using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using Microsoft.Phone.Info;
using System.Net;
using System.IO;

namespace WinPhoneBlahgua
{
    public class GoogleAnalytics
    {
        private RestClient restClient;
        private string trackingId = "UA-35868442-2";
        private string accountId = "35868442";
        private string version = "1";
        private string clientId;
        private bool sessionStarted = false;
        private string userAgentStr;
        private int counter = 1;

        public GoogleAnalytics (string uniqueId)
        {
            restClient = new RestClient("http://www.google-analytics.com");
            restClient.CookieContainer = new CookieContainer();
            clientId = uniqueId;

            ManifestAppInfo am = new ManifestAppInfo(); // gets appmanifest as per link above
            string maker = Microsoft.Phone.Info.DeviceStatus.DeviceManufacturer;
            string model = Microsoft.Phone.Info.DeviceStatus.DeviceName;

            userAgentStr = string.Format("{0} {1} {2} AppVersion {3}", maker, model, "WP7", am.Version);
        }

        public void PostPageView(string pageURL)
        {
            RestRequest request = new RestRequest("collect", Method.POST);
            //request.AddHeader("Accept", "*/*");
            request.AddHeader("User-Agent", userAgentStr);
            request.AddParameter("v", version);
            request.AddParameter("tid", trackingId);
            request.AddParameter("cid", clientId);
            request.AddParameter("t", "pageview");
            request.AddParameter("dp", pageURL);
            request.AddParameter("dl", "https://beta.blahgua.com/");
            request.AddParameter("_s", counter++);

            restClient.ExecuteAsync(request, (response) =>
            {
                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    System.Console.WriteLine("OK.");
                }
            });
        }

         public void StartSession()
        {
            counter = 1;
            RestRequest request = new RestRequest("collect", Method.POST);
            //request.AddHeader("Accept", "*/*");
            request.AddHeader("User-Agent", userAgentStr);
            request.AddParameter("v", version);
            request.AddParameter("tid", trackingId);
            request.AddParameter("cid", clientId);
            request.AddParameter("sc", "start");
            request.AddParameter("t", "pageview");
            request.AddParameter("dl", "https://beta.blahgua.com/");
            request.AddParameter("_s", counter++);

            restClient.ExecuteAsync(request, (response) =>
            {
                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    System.Console.WriteLine("OK.");
                }
            });
        }

         public void EndSession()
         {
             RestRequest request = new RestRequest("collect", Method.POST);
             //request.AddHeader("Accept", "*/*");
             request.AddHeader("User-Agent", userAgentStr);
             request.AddParameter("v", version);
             request.AddParameter("tid", trackingId);
             request.AddParameter("cid", clientId);
             request.AddParameter("sc", "end");
             request.AddParameter("t", "pageview");
             request.AddParameter("dl", "https://beta.blahgua.com/");
             request.AddParameter("_s", counter++);

             restClient.ExecuteAsync(request, (response) =>
             {
                 if (response.StatusCode == HttpStatusCode.Accepted)
                 {
                     System.Console.WriteLine("OK.");
                 }
             });
         }


    }
}
