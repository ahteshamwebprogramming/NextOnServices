using System;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

public partial class PixelTrackingFire : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //check2();
    }

    public void check2()
    {
        Response.Redirect("https://www.google.com");
    }

    [WebMethod]
    [ScriptMethod]
    public static void Check()
    {
        HttpContext.Current.Response.Redirect("https://www.google.com");
    }
}