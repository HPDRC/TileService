using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TileService
{
    public partial class rmix : raster
    {
        override protected void HandleImageResult(UrlVisitor urlVisitor)
        {
            if (!urlVisitor.isImageEmpty)
                OutputImage(urlVisitor.imageResult.ToArray(), false);
            else
                Server.Transfer("vector.aspx" + Request.Url.Query + "&styleset=map&layers=osm_land,osm_landuse,osm_water,osm_buildings,osm_roads,osm_road_names,osm_place_names");
        }
    }
}