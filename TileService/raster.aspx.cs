using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Net;
using System.IO;
using System.Threading;

namespace TileService
{
    public partial class raster : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Param param = new Param(Request);
                UrlVisitor urlVisitor = new UrlVisitor();
                urlVisitor.VisitServers(Config.raster_servers, GetPreferredRasterServerIndex(), "RasterService/bing_tile.aspx" + Request.Url.Query);
                if (urlVisitor.isSuccess)
                {
                    if (urlVisitor.isImageResult)
                        HandleImageResult(urlVisitor);
                    else
                        OutputHtml(urlVisitor.textResult);
                }
                else
                {
                    OutputError("Remote error: " + urlVisitor.errMessage);
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                OutputError("Internal error: " + ex.Message);
            }
        }

        virtual protected void HandleImageResult(UrlVisitor urlVisitor)
        {
            OutputImage(urlVisitor.imageResult.ToArray(), false);
        }
    }
}