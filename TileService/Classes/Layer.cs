using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TileService
{
    public class Layer
    {
        public string name;
        public int minZoom, maxZoom;

        public Layer(string description)
        {
            string[] sections = description.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            name = sections[0].Trim();
            minZoom = Int32.Parse(sections[1].Trim());
            maxZoom = Int32.Parse(sections[2].Trim());
        }

        public bool IsVisibleAt(int zoom)
        {
            return zoom >= minZoom && zoom <= maxZoom;
        }
    }
}