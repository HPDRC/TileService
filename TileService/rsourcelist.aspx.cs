using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TileService
{
    public partial class rsourcelist : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Param param = new Param(Request);
                UrlVisitor urlVisitor = new UrlVisitor();
                urlVisitor.VisitServers(Config.raster_servers, GetPreferredRasterServerIndex(), "RasterService/bing_sourcelist.aspx" + Request.Url.Query);
                if (urlVisitor.isSuccess)
                    OutputJson(urlVisitor.textResult);
                else
                    OutputJson("{\"success\":false, \"error_message\":\"remote server error: " + urlVisitor.errMessage + "\", \"sources\":[]}");
            }
            catch (Exception ex)
            {
                OutputJson("{\"success\":false, \"error_message\":\"remote server error: " + ex.Message + "\", \"sources\":[]}");
            }
        }
    }
}