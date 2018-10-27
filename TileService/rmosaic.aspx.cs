using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TileService
{
    public partial class rmosaic : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Param param = new Param(Request);
                UrlVisitor urlVisitor = new UrlVisitor();
                if (Request["z"] != null)
                    urlVisitor.VisitServers(Config.raster_servers, GetPreferredRasterServerIndex(), "RasterService/bing_mosaic_fixed.aspx" + Request.Url.Query);
                else
                    urlVisitor.VisitServers(Config.raster_servers, GetPreferredRasterServerIndex(), "RasterService/bing_mosaic_any.aspx" + Request.Url.Query);
                if (urlVisitor.isSuccess)
                {
                    if (urlVisitor.isImageResult)
                        OutputImage(urlVisitor.imageResult.ToArray(), false);
                    else
                        OutputHtml(urlVisitor.textResult);
                }
                else
                {
                    OutputError("Remote server error: " + urlVisitor.errMessage);
                }
            }
            catch (Exception ex)
            {
                OutputError("Internal error: " + ex.Message);
            }
        }
    }
}