using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TileService
{
    public partial class utm : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Param param = new Param(Request);
                UrlVisitor urlVisitor = new UrlVisitor();
                urlVisitor.VisitServers(Config.raster_servers, GetPreferredRasterServerIndex(), "RasterService/utm.aspx" + Request.Url.Query);
                if (urlVisitor.isSuccess)
                {
                    if (urlVisitor.isImageResult)
                        OutputImage(urlVisitor.imageResult.ToArray(), false);
                    else
                        OutputHtml(urlVisitor.textResult);
                }
                else
                {
                    OutputError("Remote error: " + urlVisitor.errMessage);
                }
            }
            catch (Exception ex)
            {
                OutputError("Internal error: " + ex.Message);
            }
        }
    }
}