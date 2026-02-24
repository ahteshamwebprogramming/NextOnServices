using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UserProfile : System.Web.UI.Page
{
    ClsDAL objdal = new ClsDAL();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString["ID"] != null)
            {
                string id = Request.QueryString["ID"].ToString();
                getdetails(id);
            }
            else if (Session["UserId"] != null)
            {
                string id = Session["UserId"].ToString();
                getdetails(id);
            }

            else
            {
                Response.Redirect("UsersDetails.aspx");
            }
        }

    }


    public void getdetails(string id)
    {
        DataTable dt = objdal.getuserprofiledetails(id);
        populatedetails(dt);
    }
    public void populatedetails(DataTable dt)
    {
        if (dt.Rows.Count > 0)
        {
            foreach (DataRow dr in dt.Rows)
            {
                lblUserId.Text = dr["Username"].ToString();
                lblMobile.Text = dr["contactnumber"].ToString();
                lblEmail.Text = dr["emailid"].ToString();
                lblAddress.Text = dr["Address"].ToString();
                lblCountry.Text = dr["Country"].ToString();
                lblRatings.Text = dr["Rating"].ToString();
                lblProjects.Text = dr["Project"].ToString();
            }
        }
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static string checkuser(string username, string password, string Device)
    {
       
        Page page = new Page();
       
        var currentScriptManager = System.Web.UI.ScriptManager.GetCurrent(page);
        
        ClsDAL objdal = new ClsDAL();
        
        UserProfile up = new UserProfile();
        
        HttpContext.Current.Session["Device"] = Device;
        
        string Device1 = HttpContext.Current.Session["Device"].ToString();
        
        try
        {
            if (string.IsNullOrEmpty(username))
            {
                // HttpContext.Current.Response.Write("<script>alert('Error');</script>");
                //ScriptManager.RegisterClientScriptBlock(up.Page, up.Page.GetType(), "alert", "alert('User Id is empty');", true);

                return "1";
            }

            if (string.IsNullOrEmpty(password))
            {
                // lblmsg.Text = "Please insert the Password";
                //ScriptManager.RegisterClientScriptBlock(up.Page, up.Page.GetType(), "alert", "alert('Password is empty');", true);
                //txtPassword.Focus();
                return "2";
            }


            SqlDataReader dr = objdal.ValidateUser(username, password);
            if (dr.Read())
            {

                HttpContext.Current.Session["pwd"] = password;
                HttpContext.Current.Session["UN"] = username;
                HttpContext.Current.Session["CN"] = Convert.ToString(dr["ContactNumber"]);


                HttpContext.Current.Session["Status"] = Convert.ToString(dr["Status"]);
                HttpContext.Current.Session["UserId"] = Convert.ToInt64(dr["ID"]);
                HttpContext.Current.Session["UserType"] = Convert.ToString(dr["UserType"]);
                //HttpContext.Current.Session["UserType"] = Convert.ToString(dr["u_type"]);

                dr.Close();
                // objDal.AddAuditTrail(Session["UserName"].ToString(), "LOGIN", "Y", Request.ServerVariables["remote_addr"].ToString());
                //if(Convert.ToString(Session["UserType"]).Trim() =="S")
                //HttpContext.Current.Response.Redirect("Dashboard.aspx", false);
                //else
                //    Response.Redirect("HomeUser.aspx", false);
                return "3";
            }
            else
            {
                return "4";
                //Page page1 = HttpContext.Current.CurrentHandler as Page;
                // page1.ClientScript.RegisterClientScriptBlock(page1.GetType(), "alert", "alert('Invalid Credentials');", true);
                //  HttpContext.Current.Response.Write("alert('Error')");


                //ScriptManager.RegisterClientScriptBlock(up.Page, up.Page.GetType(), "alert", "alert('Invalid Credentials');", true);
                //  lblmsg.Text = "Invalid Credentials, Pls contact to Administrator";
                // ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "alert('Invalid Credentials');", true);
                // SendAlert("Invalid Credentials");
                //  return;
                // lblMsg.Text = "Invalid Credentials";
            }
        }
        catch (SystemException ex)
        {
            // objDal.AddAuditTrail(Session["UserName"].ToString(), "LOGIN", "N", Request.ServerVariables["remote_addr"].ToString());
            // HttpContext.Current.Response.Write("<script>alert('There is some error while performing operation')</script>");
            ClsDAL.WriteErrorLog("Error occured in Loginuser page on btnLogin_Click()event : " + ex.Message);
            return "5";
        }
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static string logout()
    {
        HttpContext.Current.Session["pwd"] = null;
        HttpContext.Current.Session["UN"] = null;
        HttpContext.Current.Session["CN"] = null;


        HttpContext.Current.Session["Status"] = null;
        HttpContext.Current.Session["UserId"] = null;
        if (HttpContext.Current.Session["UserId"] == null)
        {
            return "1";
        }
        else
            return "2";
        //HttpContext.Current.Response.Redirect("Index.html");
    }
}