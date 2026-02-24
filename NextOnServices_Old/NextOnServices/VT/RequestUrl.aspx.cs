using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class RequestUrl : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString.Count > 0)
        {
            string ID = Convert.ToString(Request.QueryString["identifier"]);
            string Status = Convert.ToString(Request.QueryString["Status"]);
            ClsDAL.WriteErrorLog(ID + "  " + Status);
        }
    }
}