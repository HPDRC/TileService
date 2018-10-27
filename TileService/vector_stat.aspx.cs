using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TileService
{
    public partial class vector_stat : System.Web.UI.Page
    {
        protected string description = Statistics.GetText();

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnResetStatus_Click(object sender, EventArgs e)
        {
            Statistics.Reset();
        }
    }
}