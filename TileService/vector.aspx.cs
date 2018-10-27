using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TileService
{
    public partial class vector : PageBase
    {
        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // get parameters
                ParamVector param = new ParamVector(Request);
                GenerateVectorTile(param);                
            }
            catch (Exception ex)
            {
                OutputError("Internal error: " + ex.Message);
            }
        }

        protected void GenerateVectorTile(ParamVector param)
        {
            // check max level
            if (param.z > Config.vector_max_level)
            {
                OutputError("Requested tile is beyond max level");
                return;
            }

            // layers will be rendered in groups
            // group requests will be sent to multiple servers in turns: the first group goes to the first server, the second goes to the second, and so on
            UrlVisitor urlVisitor = new UrlVisitor();
            PngImage img = new PngImage(param.styleset.tileWidth, param.styleset.tileHeight, param.styleset.bgColor);

            // get group queryStrings
            string[] queryStrings = param.GetQueryStringsByRenderGroup();

            long totalResponseTime = 0;
            for (int i = 0; i < queryStrings.Length; i++)
            {
                if (queryStrings[i] == "")
                    continue;
                int serverVisited = urlVisitor.VisitServers(Config.vector_servers, i % Config.vector_servers.Length, queryStrings[i]);
                if (urlVisitor.isSuccess)
                {
                    if (urlVisitor.isImageResult)
                    {
                        Statistics.LayerGroupRequested(urlVisitor.responseTime, serverVisited);
                        totalResponseTime += urlVisitor.responseTime;
                        img.Add(urlVisitor.imageResult);
                    }
                    else
                    {
                        OutputError("Remote error: remote server returns text where image is expected: " + urlVisitor.textResult);
                        return;
                    }
                }
                else
                {
                    OutputError("Remote error: " + urlVisitor.errMessage);
                    return;
                }
            }

            // merge images into one
            OutputImage(img.Save(), true);
            Statistics.TileRequested(totalResponseTime, param.z);
        }
        
    }
}