using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;

namespace TileService
{
    public class PageBase : System.Web.UI.Page
	{
        protected void OutputError(string text)
        {
            Response.StatusDescription = text;
            Response.ContentType = "text/plain";
        }

        protected void OutputImage(byte[] buffer, bool isPNG)
        {
            Response.Cache.SetExpires(DateTime.Now + new TimeSpan(0, Config.client_cache_minutes, 0));
            Response.ContentType = isPNG ? "image/png" : "image/jpeg";
            Response.BinaryWrite(buffer);
        }

        protected void OutputHtml(string html)
        {
            Response.ContentType = "text/html";
            Response.Output.Write(html);
        }

        protected void OutputJson(string json)
        {
            Response.ContentType = "application/json";
            Response.Output.Write(json);
        }

        /// <summary>
        /// pick a random server as preferred server
        /// </summary>
        /// <returns></returns>
        protected int GetPreferredRasterServerIndex()
        {
            Random rand = new Random();
            int r = rand.Next(1, 100);
            for (int i = 0; i < Config.raster_servers.Length; i++)
            {
                if (r <= Config.raster_server_weights[i])
                    return i;
            }
            return 0;
        }
	}
}