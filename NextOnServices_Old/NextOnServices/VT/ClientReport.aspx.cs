using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ClientReport : System.Web.UI.Page
{
    // ClsDAL objDAL = new ClsDAL();
    string Status = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Respodents.Visible = false;
            lblmsg.Visible = false;

            //  GetCountries();
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<CountryDetails> GetCountries()
    {
        ClsDAL objDAL = new ClsDAL();
        List<CountryDetails> returnData = new List<CountryDetails>();
        DataSet dsclient = objDAL.ClientMGR(0, "CNT");

        if (dsclient.Tables[0].Rows.Count > 0)
        {

            foreach (DataRow dtrow in dsclient.Tables[0].Rows)
            {


                returnData.Add(new CountryDetails
                {
                    ID = System.Convert.ToInt32(dtrow["Id"]),

                    Country = dtrow["Country"].ToString()

                });

            }
        }
        return returnData;
    }





    public class CountryDetails
    {
        public int ID { get; set; }
        public string Country { get; set; }


    }
}