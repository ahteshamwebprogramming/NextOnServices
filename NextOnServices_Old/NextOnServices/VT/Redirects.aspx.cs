using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Services;

public partial class Redirects : System.Web.UI.Page
{
    private static Random random = new Random();
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    [WebMethod]
    public static string GenerateRandomLink(CharLimit formData)
    {
        string randomLink = "";
        string x = formData.Charl;
        int value;
        if (int.TryParse(x, out value))
        {
            if (value >= 8 && value <= 16)
            {
                randomLink = RandomString(value);
                return randomLink;
            }
            else
            {
                return "InvalidNumber101";
            }
        }
        else
        {
            return "";
        }
    }

    [WebMethod]
    public static List<CharLimit> SaveCompleteLink(CharLimit formData)
    {
        List<CharLimit> charLimits = new List<CharLimit>();
        if (formData.Code != "" && formData.Code != null)
        {
            ClsDAL clsDAL = new ClsDAL();
            DataSet ds = clsDAL.ManageCompleteRedirects(formData.Code, 0, 0);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dataRow in ds.Tables[0].Rows)
                    {
                        charLimits.Add(new CharLimit
                        {
                            RetVal = Convert.ToInt32(dataRow["RetVal"])
                        });
                    }
                }
            }
        }
        else
        {
            charLimits.Add(new CharLimit
            {
                RetVal = -2
            });
        }
        return charLimits;
    }
    [WebMethod]
    public static List<CharLimit> FetchCompleteLink(CharLimit formData)
    {
        List<CharLimit> charLimits = new List<CharLimit>();
        ClsDAL clsDAL = new ClsDAL();
        DataSet ds = clsDAL.ManageCompleteRedirects("", formData.opt, 0);
        if (ds.Tables.Count > 0)
        {
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dataRow in ds.Tables[0].Rows)
                {
                    charLimits.Add(new CharLimit
                    {
                        Code = Convert.ToString(dataRow["Code"]),
                        Enable = Convert.ToInt32(dataRow["Enable"]),
                        Id = Convert.ToInt32(dataRow["Id"])
                    });
                }
            }
        }
        return charLimits;
    }

    [WebMethod]
    public static List<CharLimit> UpdateStatusCompleteLink(CharLimit formData)
    {
        List<CharLimit> charLimits = new List<CharLimit>();
        ClsDAL clsDAL = new ClsDAL();
        DataSet ds = clsDAL.ManageCompleteRedirects("", formData.opt, formData.Id);
        if (ds.Tables.Count > 0)
        {
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dataRow in ds.Tables[0].Rows)
                {
                    charLimits.Add(new CharLimit
                    {
                        RetVal = Convert.ToInt32(dataRow["RetVal"])
                    });
                }
            }
        }
        return charLimits;
    }


    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
public class CharLimit
{
    public string Charl { get; set; }
    public string Link { get; set; }
    public string Code { get; set; }
    public int RetVal { get; set; }
    public int opt { get; set; }
    public int Enable { get; set; }
    public int Id { get; set; }
}