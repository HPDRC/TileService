using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace TileService
{
    public class UrlVisitor
    {
        public bool isSuccess = false;
        public string errMessage = "";

        public bool isTextResult = false;
        public string textResult = "";

        public bool isImageResult = false;
        public MemoryStream imageResult = new MemoryStream(4096);

        public bool isImageEmpty = false;

        public long responseTime = 0;
        private Stopwatch watch = new Stopwatch();

        public static Dictionary<string, DateTime> serverDownTime = new Dictionary<string, DateTime>();

        private void Visit(string url, string server, bool forceVisit)
        {
            if (!IsServerAlive(server) && !forceVisit)
            {
                isSuccess = false;
                errMessage = "server is down and skipped";
                return;
            }

            watch.Restart();
            try
            {
                // clear previous results
                isSuccess = false;
                errMessage = "";
                isTextResult = false;
                textResult = "";
                isImageResult = false;
                imageResult.Position = 0;
                isImageEmpty = false;

                // get response from url
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();

                // if response is text
                if (response.ContentType.Contains("text") || response.ContentType.Contains("xml") || response.ContentType.Contains("json"))
                {
                    isSuccess = true;
                    isTextResult = true;
                    StreamReader sr = new StreamReader(responseStream);
                    textResult = sr.ReadToEnd();
                }
                // else if response is image
                else if (response.ContentType.Contains("image"))
                {
                    isSuccess = true;
                    isImageResult = true;
                    responseStream.CopyTo(imageResult);
                    isImageEmpty = (response.Headers["IsTileEmpty"] == "1") ? true : false;
                }
                else
                    throw new Exception("response is neither text nor image");

                NotifyServerUp(server);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                errMessage = "Exception: " + ex.Message;
                NotifyServerDown(server);
            }
            watch.Stop();
            responseTime = watch.ElapsedMilliseconds;
        }

        private bool IsServerAlive(string server)
        {
            lock (serverDownTime)
            {
                if (!serverDownTime.ContainsKey(server) || (DateTime.Now - serverDownTime[server]).TotalMinutes > Config.server_down_recheck_minutes)
                    return true;
                return false;
            }
        }

        private void NotifyServerDown(string server)
        {
            lock (serverDownTime)
            {
                serverDownTime[server] = DateTime.Now;
            }
        }

        private void NotifyServerUp(string server)
        {
            lock (serverDownTime)
            {
                serverDownTime[server] = DateTime.Now.AddYears(-1);
            }
        }

        /// <summary>
        /// Try to get tile from multiple servers one by one, preferred server will be tried first, others will then be visited if preferred one fails.
        /// Return which server is actually used
        /// </summary>
        /// <param name="serverList"></param>
        /// <param name="preferredServerIndex"></param>
        /// <param name="queryString"></param>
        public int VisitServers(string[] serverList, int preferredServerIndex, string queryString)
        {
            // if still has alive servers, do not force visit
            bool forceVisit = true;
            foreach (string server in serverList)
            {
                if (IsServerAlive(server))
                    forceVisit = false;
            }

            Visit("http://" + serverList[preferredServerIndex] + "/" + queryString, serverList[preferredServerIndex], forceVisit);
            if (isSuccess)
                return preferredServerIndex;

            // go to other servers
            for (int i = 0; i < serverList.Length; i++)
            {
                if (i == preferredServerIndex)
                    continue;
                Visit("http://" + serverList[i] + "/" + queryString, serverList[i], forceVisit);
                if (isSuccess)
                    return i;
            }

            // when all servers fail
            return -1;
        }
    }
}