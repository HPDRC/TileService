using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace TileService
{
    public static class Helper
    {
        public static string GetStringConfig(string name)
        {
            if (ConfigurationManager.AppSettings[name] == null)
                throw new Exception("Config " + name + " doesn't exist. Please check web.config file");
            return ConfigurationManager.AppSettings[name];
        }

        public static int GetIntConfig(string name)
        {
            int result;
            if (!Int32.TryParse(GetStringConfig(name), out result))
                throw new Exception("Config " + name + " is not an integer. Please check web.config file");
            return result;
        }
    }
}