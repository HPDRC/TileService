using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TileService
{
    public class Param
    {
        protected HttpRequest request;

        public string projection;

        public Param(HttpRequest request)
        {
            this.request = request;
            projection = GetStringParam("projection").ToLower();
            if (!(projection == "bing" || projection == "utm" || projection == "nasa"))
                throw new Exception("param projection should be bing or utm or nasa");
        }

        protected string GetStringParam(string paramName)
        {
            string paramValue = request[paramName];
            if (paramValue == null)
                throw new Exception("param \"" + paramName + "\" is missing");
            return paramValue;
        }

        protected int GetIntParam(string paramName)
        {
            string paramValue = GetStringParam(paramName);

            try
            {
                return Int32.Parse(paramValue);
            }
            catch (Exception)
            {
                throw new Exception("param \"" + paramName + "\" must be an integer");
            }
        }
    }
}