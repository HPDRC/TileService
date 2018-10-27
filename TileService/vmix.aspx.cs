using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TileService
{
    public partial class vmix : vector
    {
        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // get param
                ParamVector param = new ParamVector(Request);

                // check if raster is available
                UrlVisitor urlVisitor = new UrlVisitor();
                urlVisitor.VisitServers(Config.raster_servers, 0, "RasterService/bing_tile_exist.aspx" + Request.Url.Query);
                if (urlVisitor.isSuccess && urlVisitor.isTextResult && urlVisitor.textResult.Trim()=="0")
                {
                    // when raster is not available at this location, return fully transparent tile
                    OutputImage(Config.fully_transparent_256_tile, true);
                }
                else
                {
                    // when anything goes wrong, just return regular vector tile
                    GenerateVectorTile(param);
                }
            }
            catch (Exception ex)
            {
                OutputError("Internal error: " + ex.Message);
            }
        }
    }
}