using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
public partial class VT_AddParameter : System.Web.UI.Page
{
    static NextOnServicesEntities3 db = new NextOnServicesEntities3();
    protected void Page_Load(object sender, EventArgs e)
    {
    }
    [WebMethod]
    public static List<Project> GetProjects()
    {
        List<Project> p = db.Projects.Where(x => x.IsActive == 1).ToList();
        return p;
    }
}