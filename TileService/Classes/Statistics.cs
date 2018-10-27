using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TileService
{
    public static class Statistics
    {
        private static DateTime initDate = DateTime.Now;

        private const int maxZoomLevel = 22;

        private static int serverCount = Config.vector_servers.Length;

        private static CountTime tileTotal = new CountTime();

        private static CountTime[] tileByLevel = new CountTime[maxZoomLevel];

        private static CountTime layerGroupTotal = new CountTime();

        private static CountTime[] layerGroupByServer = new CountTime[serverCount];

        private static object thisLock = new object();

        public static void LayerGroupRequested(long timeCost, int serverVisited)
        {
            lock (thisLock)
            {
                try
                {
                    layerGroupTotal.Increase(timeCost);
                    if (serverVisited < serverCount)
                        layerGroupByServer[serverVisited].Increase(timeCost);
                }
                catch (Exception){}
            }
        }

        public static void TileRequested(long timeCost, int level)
        {
            lock (thisLock)
            {
                try
                {
                    tileTotal.Increase(timeCost);
                    if (level < maxZoomLevel)
                        tileByLevel[level].Increase(timeCost);
                }
                catch (Exception) { }
            }
        }

        public static string GetText()
        {
            string result = "";
            lock (thisLock)
            {
                try
                {
                    result += "Statistics since " + initDate + "<br>\r\n";
                    result += "--------------------- List of downed servers -----------------------<br>\r\n";
                    foreach (KeyValuePair<string, DateTime> pair in UrlVisitor.serverDownTime)
                    {
                        result += pair.Key + " " + pair.Value + "<br>\r\n";
                    }
                    result += "--------------------- Tile Requests -----------------------<br>\r\n";
                    result += "<table><tr><th>Category</th><th>Count</th><th>Average</th><th>Maximum</th></tr>";
                    for (int i = 0; i < maxZoomLevel; i++ )
                        result += String.Format("<tr><td>Level {0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>", i, tileByLevel[i].count, tileByLevel[i].averageTime, tileByLevel[i].maxTime);
                    result += String.Format("<tr><td>All levels</td><td>{0}</td><td>{1}</td><td>{2}</td></tr>", tileTotal.count, tileTotal.averageTime, tileTotal.maxTime);
                    result += "</table><br>\r\n";

                    result += "--------------------- LayerGroup Requests -----------------------<br>\r\n";
                    result += String.Format("Total: {0}({1}ms)<br>\r\n", layerGroupTotal.count, layerGroupTotal.averageTime);
                    for (int i = 0; i < serverCount; i++)
                    {
                        result += String.Format("Server {0}: {1}({2}ms)<br>\r\n", i, layerGroupByServer[i].count, layerGroupByServer[i].averageTime);
                    }
                }
                catch (Exception ex)
                {
                    result = "Exception: " + ex.Message;
                }
            }
            return result;
        }

        static Statistics()
        {
            for (int i = 0; i < serverCount; i++)
                layerGroupByServer[i] = new CountTime();
            for (int i = 0; i < maxZoomLevel; i++)
                tileByLevel[i] = new CountTime();
        }

        public static void Reset()
        {
            lock (thisLock)
            {
                initDate = DateTime.Now;
                tileTotal.Reset();
                layerGroupTotal.Reset();
                for (int i = 0; i < serverCount; i++)
                    layerGroupByServer[i].Reset();
                for (int i = 0; i < maxZoomLevel; i++)
                    tileByLevel[i].Reset();
            }
        }
    }

    public class CountTime
    {
        public int count = 0;
        public long time = 0;
        public long maxTime = 0;

        public long averageTime
        {
            get { return count == 0 ? 0 : time / count; }
        }

        public void Increase(long time)
        {
            this.count++;
            this.time += time;
            this.maxTime = Math.Max(maxTime, time);
        }

        public void Reset()
        {
            count = 0;
            time = 0;
            maxTime = 0;
        }
    }
}