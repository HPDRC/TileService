using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;

namespace TileService
{
    public static class Config
    {
        public static byte[] fully_transparent_256_tile;

        public static string[] raster_servers;

        public static int[] raster_server_weights;

        public static string[] vector_servers;

        public static int client_cache_minutes;

        public static int vector_max_level;

        public static int server_down_recheck_minutes;

        public static Dictionary<string, StyleSet> styleSets = new Dictionary<string, StyleSet>();

        static Config()
        {
            try
            {
                server_down_recheck_minutes = Helper.GetIntConfig("server_down_recheck_minutes");

                /////////////////////////////// raster config //////////////////////////////////////

                // get raster server list
                raster_servers = Helper.GetStringConfig("raster_server_list").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (raster_servers.Length <= 0)
                    throw new Exception("Wrong configuration: no raster server is found, check web.config");

                // get raster server visit proportions
                string[] weightString = Helper.GetStringConfig("raster_server_weights").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (weightString.Length != raster_servers.Length)
                    throw new Exception("Wrong configuration: raster server and proportion count doesn't match, check web.config");
                raster_server_weights = new int[raster_servers.Length];
                try
                {
                    for (int i = 0; i < raster_server_weights.Length; i++)
                        raster_server_weights[i] = Int32.Parse(weightString[i]);
                }
                catch (Exception)
                {
                    throw new Exception("Wrong configuration: raster server weight must be an integer, check web.config");
                }

                // modify proportion array to sum array
                for (int i = 1; i < raster_server_weights.Length; i++)
                    raster_server_weights[i] += raster_server_weights[i - 1];
                if (raster_server_weights[raster_server_weights.Length - 1] != 100)
                    throw new Exception("Wrong configuration: sum of raster server weight must be 100, check web.config");


                /////////////////////////////// vector config //////////////////////////////////////

                vector_max_level = Helper.GetIntConfig("vector_max_level");
                client_cache_minutes = Helper.GetIntConfig("client_cache_minutes");

                // get vector server list
                vector_servers = Helper.GetStringConfig("vector_server_list").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (vector_servers.Length <= 0)
                    throw new Exception("Wrong configuration: no vector server is found, check web.config");


                string styleset_list = Helper.GetStringConfig("styleset_list");
                foreach (string stylesetName in styleset_list.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    styleSets.Add(stylesetName, new StyleSet(stylesetName));
                }

                /////////////////////////////// generate fully transparent tile //////////////////////////////////////
                Bitmap bm = new Bitmap(256, 256, PixelFormat.Format32bppPArgb);
                using (MemoryStream stream = new MemoryStream())
                {
                    bm.Save(stream, ImageFormat.Png);
                    stream.Close();
                    fully_transparent_256_tile = stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in Config class: " + ex.Message);
            }
        }
    }
}