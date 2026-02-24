using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

/// <summary>
/// Summary description for ClsDAL
/// </summary>
public class ClsDAL
{
    public ClsDAL()
    {
        strConn = ConfigurationSettings.AppSettings["strconn"].ToString();
        Connection = new SqlConnection(strConn);
    }
    DataSet dsClient = new DataSet();
    string strConn = "", output = "", strqury = "", strBckupConn;
    SqlConnection Connection = null;
    SqlDataReader drUser = null, drSupplier = null;
    public string ClientMgr(string Company, string Persion, string Number, string Email, string Country, string Notes, int ID, int OPT)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {


            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("uspMgrClient", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            SqlParameter paramOut = new SqlParameter("@Result", SqlDbType.VarChar, 25);
            paramOut.Direction = ParameterDirection.Output;
            cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            cmdsave.Parameters.Add("@Company", SqlDbType.NVarChar, 500).Value = Company;
            cmdsave.Parameters.Add("@CPerson", SqlDbType.NVarChar, 500).Value = Persion;
            cmdsave.Parameters.Add("@CNumber", SqlDbType.NVarChar, 50).Value = Number;
            cmdsave.Parameters.Add("@CEmail", SqlDbType.NVarChar, 500).Value = Email;
            cmdsave.Parameters.Add("@Country", SqlDbType.NVarChar, 50).Value = Country;
            cmdsave.Parameters.Add("@Notes", SqlDbType.NVarChar, 4000).Value = Notes;
            cmdsave.Parameters.Add("@Opt", SqlDbType.Int).Value = OPT;
            cmdsave.Parameters.Add(paramOut);
            drUser = cmdsave.ExecuteReader();
            output = Convert.ToString(paramOut.Value);
            // output = Convert.ToString(results.remark);


            //  }
            return output;




        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");
            return "Trandaction failed";
        }
        finally
        {
            drUser.Close();
            Connection.Close();
        }
    }

    public DataTable getProjectsName()
    {
        DataTable dt = new DataTable();
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            string query = "select ID,PName,Status from Projects where isnull(isactive,0)=1";
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand(query, Connection);
            cmdsave.CommandType = CommandType.Text;

            //cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            //cmdsave.Parameters.Add("@Type", SqlDbType.NVarChar, 50).Value = Type;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);

            adp.Fill(dt);

            // output = Convert.ToString(results.remark);


            //  }
            return dt;




        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {

            Connection.Close();
        }
    }
    public int UpdateProjectStatus(string status, string id, string origin)
    {
        DataTable dt = new DataTable();
        DataSet DsRech = new DataSet();
        try
        {
            string query = "";
            dsClient.Clear();
            if (origin == "Dashboard")
            {
                query = "update Projects set Status=" + status + " where ID=" + id;
            }
            else if (origin == "PPDetails")
            {
                query = "UPDATE dbo.ProjectsUrl SET Status=" + status + " WHERE ID=" + id;
            }
            else if (origin == "UpdateUrlStatus")
            {
                query = "UPDATE dbo.ProjectsUrl SET Status=" + status + " WHERE PID=" + id;
            }
            else if (origin == "Recontact")
            {
                query = "UPDATE dbo.RecontactProjects SET Status=" + status + " WHERE id=" + id;

            }
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand(query, Connection);
            cmdsave.CommandType = CommandType.Text;
            int x = cmdsave.ExecuteNonQuery();
            return x;
        }
        catch (SystemException ex)
        {
            return 0;
        }
        finally
        {
            Connection.Close();
        }
    }
    public int Delete(int opt, string id, int stat)
    {
        DataTable dt = new DataTable();
        DataSet DsRech = new DataSet();
        // string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("Delete_Anything", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@id", SqlDbType.Int).Value = Convert.ToInt32(id);
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = Convert.ToInt32(opt);
            cmdsave.Parameters.Add("@stat", SqlDbType.Int).Value = Convert.ToInt32(stat);
            int x = cmdsave.ExecuteNonQuery();
            return x;
        }
        catch (SystemException ex)
        {
            return 0;
        }
        finally
        {
            Connection.Close();
        }
    }

    public int ChangePasswordd(int opt, string oldpassword, string newpassword, string username, string userid)
    {
        DataTable dt = new DataTable();
        DataSet DsRech = new DataSet();
        // string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("ChangePassword", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = Convert.ToInt32(opt);
            cmdsave.Parameters.Add("@userid", SqlDbType.VarChar, 100).Value = userid;
            cmdsave.Parameters.Add("@username", SqlDbType.VarChar, 100).Value = username;
            cmdsave.Parameters.Add("@oldpassword", SqlDbType.VarChar, 100).Value = oldpassword;
            cmdsave.Parameters.Add("@newpassword", SqlDbType.VarChar, 100).Value = newpassword;
            int x = cmdsave.ExecuteNonQuery();
            return x;
        }
        catch (SystemException ex)
        {
            return 0;
        }
        finally
        {
            Connection.Close();
        }
    }
    public DataTable getStatusbyProjectID(string id)
    {
        DataTable dt = new DataTable();
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            string query = "select Status from Projects where ID=" + id;
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand(query, Connection);
            cmdsave.CommandType = CommandType.Text;
            //cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            //cmdsave.Parameters.Add("@Type", SqlDbType.NVarChar, 50).Value = Type;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(dt);
            // output = Convert.ToString(results.remark);
            //  }
            return dt;
        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");
        }
        finally
        {
            Connection.Close();
        }
    }
    public string UserMgr(string username, string Name, string EmailID, string Password, string Address, string Contact, string role, int ID, int OPT)
    {
        string role1;
        DataSet DsRech = new DataSet();
        string status = "";
        if (role == "Admin")
        {
            role1 = "A";
        }
        else
        {
            role1 = "U";
        }

        try
        {

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("uspMgrUser", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            SqlParameter paramOut = new SqlParameter("@Result", SqlDbType.VarChar, 25);
            paramOut.Direction = ParameterDirection.Output;
            cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            cmdsave.Parameters.Add("@UName", SqlDbType.NVarChar, 500).Value = Name;
            cmdsave.Parameters.Add("@Email", SqlDbType.NVarChar, 500).Value = EmailID;
            cmdsave.Parameters.Add("@Password", SqlDbType.NVarChar, 50).Value = Password;
            cmdsave.Parameters.Add("@Address", SqlDbType.NVarChar, 500).Value = Address;
            cmdsave.Parameters.Add("@Contact", SqlDbType.NVarChar, 50).Value = Contact;
            cmdsave.Parameters.Add("@Role", SqlDbType.NVarChar, 50).Value = role1;
            cmdsave.Parameters.Add("@Opt", SqlDbType.Int).Value = OPT;
            cmdsave.Parameters.Add("@username", SqlDbType.VarChar, 200).Value = username;
            cmdsave.Parameters.Add(paramOut);
            drUser = cmdsave.ExecuteReader();
            output = Convert.ToString(paramOut.Value);
            // output = Convert.ToString(results.remark);


            //  }
            return output;




        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");
            return "Transaction failed";
        }
        finally
        {
            drUser.Close();
            Connection.Close();
        }
    }


    public string SupplierMgr(string Name, string Description, string Number, string Email, string PSize, string Country, string Notes, int ID, int OPT, string Completes, string Terminate, string Overquota, string Security, string Fraud, string Success, string Default, string Failure, string Quality, string OverquotaPixel)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {


            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("uspMgrSppliers", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            SqlParameter paramOut = new SqlParameter("@Result", SqlDbType.VarChar, 25);
            paramOut.Direction = ParameterDirection.Output;
            cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            cmdsave.Parameters.Add("@Name", SqlDbType.NVarChar, 500).Value = Name;
            cmdsave.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value = Description;
            cmdsave.Parameters.Add("@Number", SqlDbType.NVarChar, 50).Value = Number;
            cmdsave.Parameters.Add("@Email", SqlDbType.NVarChar, 500).Value = Email;
            cmdsave.Parameters.Add("@PSize", SqlDbType.NVarChar, 50).Value = PSize;
            cmdsave.Parameters.Add("@Country", SqlDbType.NVarChar, 50).Value = Country;
            cmdsave.Parameters.Add("@Completes", SqlDbType.NVarChar, 4000).Value = Completes;
            //cmdsave.Parameters.Add("@Incomplete", SqlDbType.NVarChar, 4000).Value = incomplete;
            cmdsave.Parameters.Add("@Terminate", SqlDbType.NVarChar, 4000).Value = Terminate;
            cmdsave.Parameters.Add("@Overquota", SqlDbType.NVarChar, 4000).Value = Overquota;
            cmdsave.Parameters.Add("@security", SqlDbType.NVarChar, 4000).Value = Security;
            cmdsave.Parameters.Add("@fraud", SqlDbType.NVarChar, 4000).Value = Fraud;
            cmdsave.Parameters.Add("@Notes", SqlDbType.NVarChar, 4000).Value = Notes;
            cmdsave.Parameters.Add("@Opt", SqlDbType.Int).Value = OPT;
            cmdsave.Parameters.Add("@Success", SqlDbType.NVarChar, 4000).Value = Success;
            cmdsave.Parameters.Add("@Default", SqlDbType.NVarChar, 4000).Value = Default;
            cmdsave.Parameters.Add("@Failure", SqlDbType.NVarChar, 4000).Value = Failure;
            cmdsave.Parameters.Add("@Quality", SqlDbType.NVarChar, 4000).Value = Quality;
            cmdsave.Parameters.Add("@OverquotaPixel", SqlDbType.NVarChar, 4000).Value = OverquotaPixel;
            cmdsave.Parameters.Add(paramOut);
            drUser = cmdsave.ExecuteReader();
            output = Convert.ToString(paramOut.Value);
            // output = Convert.ToString(results.remark);


            //  }
            return output;




        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");
            return "Trandaction failed";
        }
        finally
        {
            drUser.Close();
            Connection.Close();
        }
    }

    public string ProjectMgr(ClsProject objP, int Type)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            //WriteErrorLog("Entered CLSDAL Method");
            SqlCommand cmdsave = new SqlCommand("uspMgrProject", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            SqlParameter paramOut = new SqlParameter("@Result", SqlDbType.VarChar, 25);
            paramOut.Direction = ParameterDirection.Output;
            cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = objP.ID;
            cmdsave.Parameters.Add("@PName", SqlDbType.NVarChar, 500).Value = objP.PName;
            cmdsave.Parameters.Add("@Descriptions", SqlDbType.NVarChar, 500).Value = objP.Descriptions;
            cmdsave.Parameters.Add("@ClientID", SqlDbType.Int).Value = objP.ClientID;
            cmdsave.Parameters.Add("@PManager", SqlDbType.NVarChar, 500).Value = objP.PManager;
            cmdsave.Parameters.Add("@LOI", SqlDbType.NVarChar, 50).Value = objP.LOI;
            cmdsave.Parameters.Add("@IRate", SqlDbType.NVarChar, 50).Value = objP.IRate;
            cmdsave.Parameters.Add("@CPI", SqlDbType.Float).Value = objP.CPI;
            cmdsave.Parameters.Add("@pid", SqlDbType.VarChar, 100).Value = objP.PID;
            cmdsave.Parameters.Add("@SampleSize", SqlDbType.NVarChar, 500).Value = objP.SampleSize;
            cmdsave.Parameters.Add("@Quota", SqlDbType.NVarChar, 50).Value = objP.Quota;
            //  WriteErrorLog("InCLsMethod beforedate");
            cmdsave.Parameters.Add("@SDate", SqlDbType.VarChar, 50).Value = objP.SDate;
            cmdsave.Parameters.Add("@EDate", SqlDbType.VarChar, 50).Value = objP.EDate;
            //  WriteErrorLog("InCLSMethod Afterdate");
            cmdsave.Parameters.Add("@Country", SqlDbType.NVarChar, 50).Value = objP.Country;
            cmdsave.Parameters.Add("@LType", SqlDbType.Int).Value = objP.LType;
            cmdsave.Parameters.Add("@Notes", SqlDbType.NVarChar, 4000).Value = objP.Notes;
            cmdsave.Parameters.Add("@Status", SqlDbType.Int).Value = objP.Status;
            cmdsave.Parameters.Add("@Opt", SqlDbType.Int).Value = Type;
            //WriteErrorLog("InCLS Method after all insertion");
            cmdsave.Parameters.Add(paramOut);
            drUser = cmdsave.ExecuteReader();
            // WriteErrorLog("after Execute error");
            output = Convert.ToString(paramOut.Value);
            return output;
        }
        catch (SystemException ex)
        {
            WriteErrorLog("Exception");
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");
            return "Trandaction failed";
        }
        finally
        {
            drUser.Close();
            Connection.Close();
        }
    }
    public string ProjectMgrEdit(ClsProject objP, int Type)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            //WriteErrorLog("Entered CLSDAL Method");
            SqlCommand cmdsave = new SqlCommand("uspMgrProject", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            SqlParameter paramOut = new SqlParameter("@Result", SqlDbType.VarChar, 25);
            paramOut.Direction = ParameterDirection.Output;
            cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = objP.ID;
            cmdsave.Parameters.Add("@PName", SqlDbType.NVarChar, 500).Value = objP.PName;
            cmdsave.Parameters.Add("@Descriptions", SqlDbType.NVarChar, 500).Value = objP.Descriptions;
            cmdsave.Parameters.Add("@ClientID", SqlDbType.Int).Value = objP.ClientID;
            cmdsave.Parameters.Add("@PManager", SqlDbType.NVarChar, 500).Value = objP.PManager;
            cmdsave.Parameters.Add("@LOI", SqlDbType.NVarChar, 50).Value = objP.LOI;
            cmdsave.Parameters.Add("@IRate", SqlDbType.NVarChar, 50).Value = objP.IRate;
            cmdsave.Parameters.Add("@CPI", SqlDbType.Float).Value = objP.CPI;
            cmdsave.Parameters.Add("@pid", SqlDbType.VarChar, 100).Value = objP.PID;
            cmdsave.Parameters.Add("@SampleSize", SqlDbType.NVarChar, 500).Value = objP.SampleSize;
            cmdsave.Parameters.Add("@Quota", SqlDbType.NVarChar, 50).Value = objP.Quota;
            //  WriteErrorLog("InCLsMethod beforedate");
            cmdsave.Parameters.Add("@SDate", SqlDbType.VarChar, 50).Value = objP.SDate;
            cmdsave.Parameters.Add("@EDate", SqlDbType.VarChar, 50).Value = objP.EDate;
            //  WriteErrorLog("InCLSMethod Afterdate");
            cmdsave.Parameters.Add("@Country", SqlDbType.NVarChar, 50).Value = objP.Country;
            cmdsave.Parameters.Add("@LType", SqlDbType.Int).Value = objP.LType;
            cmdsave.Parameters.Add("@Notes", SqlDbType.NVarChar, 4000).Value = objP.Notes;
            //cmdsave.Parameters.Add("@Status", SqlDbType.Int).Value = objP.Status;
            cmdsave.Parameters.Add("@Opt", SqlDbType.Int).Value = Type;
            //WriteErrorLog("InCLS Method after all insertion");
            cmdsave.Parameters.Add(paramOut);
            drUser = cmdsave.ExecuteReader();
            // WriteErrorLog("after Execute error");
            output = Convert.ToString(paramOut.Value);
            // output = Convert.ToString(results.remark);
            // WriteErrorLog("after taking output");

            //  }
            return output;




        }
        catch (SystemException ex)
        {
            WriteErrorLog("Exception");
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");
            return "Trandaction failed";
        }
        finally
        {
            drUser.Close();
            Connection.Close();
        }
    }
    public DataSet ClientMGR(int ID, string Type)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("UspGetAllUsers", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            cmdsave.Parameters.Add("@Type", SqlDbType.NVarChar, 50).Value = Type;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(dsClient);

            return dsClient;
        }
        catch (SystemException ex)
        {
            ClsDAL.WriteErrorLog("reached managed status catch");

            return null;
        }
        finally
        {
            Connection.Close();
        }
    }
    public DataSet GetRecontactProjects()
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("Recontactmgr", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = 2;
            //cmdsave.Parameters.Add("@Type", SqlDbType.NVarChar, 50).Value = Type;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(dsClient);
            return dsClient;
        }
        catch (SystemException ex)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }
    public DataSet mgrMappingCountries(string ProjectURLID, string CountryID, int OPT)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("mgrCountryIPMapping", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@projecturlid", SqlDbType.Int).Value = Convert.ToInt32(ProjectURLID);
            cmdsave.Parameters.Add("@countryid", SqlDbType.Int).Value = Convert.ToInt32(CountryID);
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = OPT;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(dsClient);
            return dsClient;
        }
        catch (SystemException ex)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }

    public DataSet mgrVendorPreScreening(CLSProjectmapping data)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("mgrVendorPreScreening", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@id", SqlDbType.Int).Value = Convert.ToInt32(data.ID);
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = Convert.ToInt32(data.opt);
            cmdsave.Parameters.Add("@Prescreening", SqlDbType.Int).Value = Convert.ToInt32(data.Prescreening);

            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(dsClient);
            return dsClient;
        }
        catch (SystemException ex)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }

    public DataSet GetSurveyResponse(SurveyResponse data)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            IEnumerable<SurveyResponse> result;
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("fetchPreScreeningResponse_1", Connection);
            cmdsave.CommandType = CommandType.Text;
            cmdsave.Parameters.Add("@sid", SqlDbType.NVarChar, 100).Value = "";
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = Convert.ToInt32(0);
            cmdsave.Parameters.Add("@ProjectId", SqlDbType.Int).Value = Convert.ToInt32(data.Id);

            //command.CommandText = "[dbo].[" + StoredProcedure.ManageWorkerInfo + "]  @CategoryID1='" + worker.CategoryID1 + "',@DOP='" + worker.DOP + "',@OPT='" + worker.OPT + "',@WorkerID=" + worker.WorkerID;

            //cmdsave.s

            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);

            //adp.

            adp.Fill(dsClient);
            return dsClient;
        }
        catch (SystemException ex)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }

    public DataSet FetchRecontactProjects(string id, int OPT)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            //SqlCommand cmdsave = new SqlCommand("FetchRecontactProjectRep", Connection);
            SqlCommand cmdsave = new SqlCommand("RecontactPageDetails", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@id", SqlDbType.Int).Value = Convert.ToInt32(id);
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = OPT;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(dsClient);
            return dsClient;
        }
        catch (SystemException ex)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }
    public DataSet ClientMGRAll(int ID, string Type)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("UspGetAllActiveUsers", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            cmdsave.Parameters.Add("@Type", SqlDbType.NVarChar, 50).Value = Type;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(dsClient);
            return dsClient;
        }
        catch (SystemException ex)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }
    public DataSet GetDashboardByManager(string managerid, string statusid, string Flag)
    {
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("GetDashboardbyManager", Connection);
            cmdsave.CommandTimeout = 0;
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@userid", SqlDbType.Int).Value = Convert.ToInt32(managerid);
            cmdsave.Parameters.Add("@stattype", SqlDbType.Int).Value = Convert.ToInt32(statusid);
            cmdsave.Parameters.Add("@flagtype", SqlDbType.Int).Value = Convert.ToInt32(Flag);
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(dsClient);
            return dsClient;
        }
        catch (SystemException ex)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }
    public string insertcountry(string ITEM)
    {
        DataSet dsClient = new DataSet();
        string status = "";
        try
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("AddCountry", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            //cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            cmdsave.Parameters.Add("@item", SqlDbType.NVarChar, 100).Value = ITEM;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(dsClient);
            return dsClient.Tables[0].Rows[0][0].ToString();
        }
        catch (SystemException ex)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }

    }
    public DataSet ProjectReports(int ID)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("ProjectReports_BK", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;

            cmdsave.Parameters.Add("@projectid", SqlDbType.Int).Value = ID;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);

            adp.Fill(dsClient);

            // output = Convert.ToString(results.remark);


            //  }
            return dsClient;




        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {

            Connection.Close();
        }
    }
    public DataSet GetProjectDetailedView()
    {
        DataSet dsClient1 = new DataSet();
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient1.Clear();

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("getprojectdetailedview", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(dsClient1);

            return dsClient1;
        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");
        }
        finally
        {
            Connection.Close();
        }
    }

    public DataSet PPDManagement(int ID)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("usp_GetProjectView", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;

            cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            //cmdsave.Parameters.Add("@Type", SqlDbType.NVarChar, 50).Value = Type;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);

            adp.Fill(dsClient);

            // output = Convert.ToString(results.remark);


            //  }
            return dsClient;




        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {

            Connection.Close();
        }
    }

    public DataSet SupplierPopManagement(int ID, string Type)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("UspGetAllUsers", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;

            cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            cmdsave.Parameters.Add("@Type", SqlDbType.NVarChar, 50).Value = Type;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);

            adp.Fill(dsClient);

            // output = Convert.ToString(results.remark);


            //  }
            return dsClient;




        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {

            Connection.Close();
        }
    }

    public DataSet GetProjectViewDetails(int PID)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("usp_GetProjectView", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;

            cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = PID;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);

            adp.Fill(dsClient);

            // output = Convert.ToString(results.remark);


            //  }
            return dsClient;




        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {

            Connection.Close();
        }
    }


    public DataSet GetRecontactParametersCalc(int PID, int opt)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("RecontactParametersCalC", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@id", SqlDbType.Int).Value = PID;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(dsClient);
            return dsClient;
        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");
        }
        finally
        {
            Connection.Close();
        }
    }


    public void ManageBlocking(int id, int opt)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("manageVendorBlocking", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@id", SqlDbType.Int).Value = id;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(dsClient);
            // output = Convert.ToString(results.remark);
            //  }
            //return dsClient;
        }
        catch (SystemException ex)
        {
        }
        finally
        {
            Connection.Close();
        }
    }
    public string DeviceControl(int id, int opt, string code)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("DeviceControl", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            cmdsave.Parameters.Add("@Opt", SqlDbType.Int).Value = opt;
            cmdsave.Parameters.Add("@Code", SqlDbType.VarChar, 10).Value = code;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(dsClient);
            return dsClient.Tables[0].Rows[0][0].ToString();
            // output = Convert.ToString(results.remark);
            //  }
            //return dsClient;
        }
        catch (SystemException ex)
        {
            return "Error";
        }
        finally
        {
            Connection.Close();
        }
    }
    public DataSet IPSecurityControlmgr(int id, int opt, string count)
    {
        //ClsDAL.WriteErrorLog(id.ToString());
        //ClsDAL.WriteErrorLog(opt.ToString());
        //ClsDAL.WriteErrorLog(count);
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("IPSecurityControlmgr", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@projectid", SqlDbType.Int).Value = id;
            cmdsave.Parameters.Add("@Opt", SqlDbType.Int).Value = opt;
            cmdsave.Parameters.Add("@count", SqlDbType.VarChar, 20).Value = count;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(dsClient);
            return dsClient;
            // output = Convert.ToString(results.remark);
            //  }
            //return dsClient;
        }
        catch (SystemException ex)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }
    public DataSet GetProjectDetailsForDownload(int PID, int opt)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("usp_mgrdataDownload2", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@projectid", SqlDbType.Int).Value = PID;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(dsClient);
            return dsClient;

        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {
            Connection.Close();
        }
    }

    public DataSet GetRecontactExcelData(int opt, string pro, string country, string supp)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("GetRecontactExcelData", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = Convert.ToInt32(opt);
            cmdsave.Parameters.Add("@pid", SqlDbType.Int).Value = Convert.ToInt32(pro);
            cmdsave.Parameters.Add("@cid", SqlDbType.Int).Value = Convert.ToInt32(country);
            cmdsave.Parameters.Add("@sid", SqlDbType.Int).Value = Convert.ToInt32(supp);
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(dsClient);
            return dsClient;

        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {
            Connection.Close();
        }
    }

    public DataSet GetSupplierViewDetails(int PID)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("usp_GetSupplierDetails", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;

            cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = PID;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);

            adp.Fill(dsClient);

            // output = Convert.ToString(results.remark);


            //  }
            return dsClient;




        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {

            Connection.Close();
        }
    }

    public DataSet ReportManagement(DateTime? sdate, DateTime? edate, int supplierid, int clientid, int countryid, int Id, int report, int opt)
    {
        DataSet Dsrech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("usp_ReportManagement_bk", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;

            cmdsave.Parameters.Add("@supplierid", SqlDbType.Int).Value = supplierid;
            cmdsave.Parameters.Add("@clientid", SqlDbType.Int).Value = clientid;
            cmdsave.Parameters.Add("@countryid", SqlDbType.Int).Value = countryid;
            cmdsave.Parameters.Add("@id", SqlDbType.Int).Value = Id;
            cmdsave.Parameters.Add("@report", SqlDbType.Int).Value = report;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;
            cmdsave.Parameters.Add("@startdate", SqlDbType.DateTime).Value = sdate;
            cmdsave.Parameters.Add("@enddate", SqlDbType.DateTime).Value = edate;
            SqlDataAdapter da = new SqlDataAdapter(cmdsave);
            da.Fill(dsClient);
            return dsClient;
        }
        catch (Exception e)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }
    public DataSet ManagerReportManagement(OverallReportData formdata, DateTime? sdate, DateTime? edate, int Id, int report, int opt)
    {
        DataSet Dsrech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("ManagerWiseReport_BK", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@supplierid", SqlDbType.Int).Value = formdata.supplierid;
            cmdsave.Parameters.Add("@clientid", SqlDbType.Int).Value = formdata.clientid;
            cmdsave.Parameters.Add("@countryid", SqlDbType.Int).Value = formdata.countryid;
            cmdsave.Parameters.Add("@id", SqlDbType.Int).Value = Id;
            cmdsave.Parameters.Add("@report", SqlDbType.Int).Value = report;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;
            cmdsave.Parameters.Add("@startdate", SqlDbType.DateTime).Value = sdate;
            cmdsave.Parameters.Add("@enddate", SqlDbType.DateTime).Value = edate;
            SqlDataAdapter da = new SqlDataAdapter(cmdsave);
            da.Fill(dsClient);
            return dsClient;

        }
        catch (Exception e)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }
    public DataSet GetPieChart()
    {
        DataSet Dsrech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("chartsremain", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(cmdsave);
            da.Fill(dsClient);
            return dsClient;

        }
        catch (Exception e)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }

    public DataSet RemainingTable(DateTime? sdate, DateTime? edate, int supplierid, int clientid, int countryid)
    {
        DataSet ds = new DataSet();
        DataSet Dsrech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("remainingtabled", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;

            cmdsave.Parameters.Add("@supplierid", SqlDbType.Int).Value = supplierid;
            cmdsave.Parameters.Add("@clientid", SqlDbType.Int).Value = clientid;
            cmdsave.Parameters.Add("@countryid", SqlDbType.Int).Value = countryid;
            cmdsave.Parameters.Add("@startdate", SqlDbType.DateTime).Value = sdate;
            cmdsave.Parameters.Add("@enddate", SqlDbType.DateTime).Value = edate;
            SqlDataAdapter da = new SqlDataAdapter(cmdsave);
            da.Fill(ds);
            return ds;

        }
        catch (Exception e)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }

    public DataSet GetResDetails(string id, string type)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("USP_mgrRespondents", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;

            cmdsave.Parameters.Add("@id", SqlDbType.VarChar, 200).Value = id;
            cmdsave.Parameters.Add("@type", SqlDbType.VarChar, 200).Value = type;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);

            adp.Fill(dsClient);

            // output = Convert.ToString(results.remark);


            //  }
            return dsClient;

        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {
            Connection.Close();
        }
    }
    public int UpdateStatus(string id, string status)
    {
        DataTable dt = new DataTable();
        DataSet DsRech = new DataSet();
        // string status = "";
        try
        {
            dsClient.Clear();
            string query = "update supplierprojects set Status='" + status + "' where UID='" + id + "'";
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand(query, Connection);
            cmdsave.CommandType = CommandType.Text;

            //cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            //cmdsave.Parameters.Add("@Type", SqlDbType.NVarChar, 50).Value = Type;
            int x = cmdsave.ExecuteNonQuery();

            return x;

            // output = Convert.ToString(results.remark);


            //  }





        }
        catch (SystemException ex)
        {
            return 0;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {

            Connection.Close();
        }
    }
    public DataSet Respondants(DateTime? sdate, DateTime? edate, int supplierid, int clientid, int countryid)
    {
        DataSet ds = new DataSet();
        DataSet Dsrech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("respondants", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;

            cmdsave.Parameters.Add("@supplierid", SqlDbType.Int).Value = supplierid;
            cmdsave.Parameters.Add("@clientid", SqlDbType.Int).Value = clientid;
            cmdsave.Parameters.Add("@countryid", SqlDbType.Int).Value = countryid;
            cmdsave.Parameters.Add("@startdate", SqlDbType.DateTime).Value = sdate;
            cmdsave.Parameters.Add("@enddate", SqlDbType.DateTime).Value = edate;

            SqlDataAdapter da = new SqlDataAdapter(cmdsave);
            da.SelectCommand.CommandTimeout = 60;
            da.Fill(ds);
            return ds;

        }
        catch (Exception e)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }
    public DataSet UpdateStatusinProjects(string projectid)
    {
        DataSet ds = new DataSet();
        DataSet Dsrech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("statusByPref", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@id", SqlDbType.Int).Value = Convert.ToInt32(projectid);
            SqlDataAdapter da = new SqlDataAdapter(cmdsave);
            da.Fill(ds);
            return ds;
        }
        catch (Exception e)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }
    public DataSet GetRevenueTable(DateTime? sdate, DateTime? edate, int supplierid, int clientid, int countryid)
    {
        DataSet ds = new DataSet();
        DataSet Dsrech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("usp_CalculateRevenue_BK", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;

            cmdsave.Parameters.Add("@supplierid", SqlDbType.Int).Value = supplierid;
            cmdsave.Parameters.Add("@clientid", SqlDbType.Int).Value = clientid;
            cmdsave.Parameters.Add("@countryid", SqlDbType.Int).Value = countryid;
            cmdsave.Parameters.Add("@startdate", SqlDbType.DateTime).Value = sdate;
            cmdsave.Parameters.Add("@enddate", SqlDbType.DateTime).Value = edate;

            SqlDataAdapter da = new SqlDataAdapter(cmdsave);
            da.Fill(ds);
            return ds;

        }
        catch (Exception e)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }
    public DataSet UpdateMultipleStatus(string id, string status1, int type, int pid)
    {
        DataSet ds = new DataSet();
        DataSet Dsrech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("USP_updatestatus", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;

            cmdsave.Parameters.Add("@id", SqlDbType.VarChar, 500).Value = id;
            cmdsave.Parameters.Add("@type", SqlDbType.Int).Value = type;
            cmdsave.Parameters.Add("@status", SqlDbType.VarChar, 100).Value = status1;
            cmdsave.Parameters.Add("@pid", SqlDbType.Int).Value = pid;

            SqlDataAdapter da = new SqlDataAdapter(cmdsave);
            da.Fill(ds);
            return ds;

        }
        catch (Exception e)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }
    public DataSet GetMultipleStatus(int opt, string id)
    {
        DataSet ds = new DataSet();
        DataSet Dsrech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("GetMultpleStatusProc", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;

            cmdsave.Parameters.Add("@id", SqlDbType.VarChar, 500).Value = id;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;


            SqlDataAdapter da = new SqlDataAdapter(cmdsave);
            da.Fill(ds);
            return ds;

        }
        catch (Exception e)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }
    public DataSet GetClientViewDetails(int ID)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }


            SqlCommand cmdsave = new SqlCommand("usp_GetClientsDetails", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;

            cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);

            adp.Fill(dsClient);

            // output = Convert.ToString(results.remark);


            //  }
            return dsClient;




        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {

            Connection.Close();
        }
    }

    public DataSet TokenMgr(string nid, string sid, int opt)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("Tokensmgr", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;
            cmdsave.Parameters.Add("@sid", SqlDbType.VarChar, 10).Value = sid;
            cmdsave.Parameters.Add("@nid", SqlDbType.VarChar, 500).Value = nid;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(dsClient);
            return dsClient;
        }
        catch (SystemException ex)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }


    //public DataSet GetProjectUrl(int CID)
    //{
    //    DataSet DsRech = new DataSet();
    //    string status = "";
    //    try
    //    {
    //        dsClient.Clear();

    //        if (Connection.State == ConnectionState.Closed)
    //        {
    //            Connection.Open();
    //        }
    //        SqlCommand cmdsave = new SqlCommand("UspGetAllUsers", Connection);
    //        cmdsave.CommandType = CommandType.StoredProcedure;

    //        cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
    //        cmdsave.Parameters.Add("@Type", SqlDbType.NVarChar, 50).Value = Type;
    //        SqlDataAdapter adp = new SqlDataAdapter(cmdsave);

    //        adp.Fill(dsClient);

    //        // output = Convert.ToString(results.remark);


    //        //  }
    //        return dsClient;




    //    }
    //    catch (SystemException ex)
    //    {
    //        return null;
    //        // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

    //    }
    //    finally
    //    {

    //        Connection.Close();
    //    }
    //}

    public DataSet GetProjectsUrl(int CID, int PID)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            //SqlCommand cmdsave = new SqlCommand("UspGetAllUsers", Connection);
            //cmdsave.CommandType = CommandType.StoredProcedure;

            //cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            //cmdsave.Parameters.Add("@Type", SqlDbType.NVarChar, 50).Value = Type;
            SqlDataAdapter adp = new SqlDataAdapter("select Url from [dbo].[ProjectsUrl] where CID=" + CID + " and  PID=" + PID, Connection);

            adp.Fill(dsClient);

            // output = Convert.ToString(results.remark);


            //  }
            return dsClient;




        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {

            Connection.Close();
        }
    }
    public DataSet GetOrignalUrl(string SID, string Code, string NID)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("GetOrignalLink", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@SID", SqlDbType.NVarChar, 50).Value = SID;
            cmdsave.Parameters.Add("@Code", SqlDbType.NVarChar, 50).Value = Code;
            cmdsave.Parameters.Add("@ID", SqlDbType.NVarChar, 50).Value = NID;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(dsClient);
            return dsClient;
        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {
            Connection.Close();
        }
    }


    public DataSet GetSupplierPanelSize(int ID, int SID)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("Usp_GetSupplierPanelSize", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;

            cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            cmdsave.Parameters.Add("@SID", SqlDbType.Int).Value = SID;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);

            adp.Fill(dsClient);

            // output = Convert.ToString(results.remark);


            //  }
            return dsClient;




        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {

            Connection.Close();
        }
    }


    public DataSet GetProjectUrl(int ID, int PID)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("Usp_GetProjectsUrl", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;

            cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            cmdsave.Parameters.Add("@PID", SqlDbType.Int).Value = PID;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);

            adp.Fill(dsClient);

            // output = Convert.ToString(results.remark);


            //  }
            return dsClient;




        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {

            Connection.Close();
        }
    }
    public DataSet GetProjectUrlById(int ID)
    {
        DataTable dt = new DataTable();
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("Usp_GetProjectsUrlByID", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;

            cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            // cmdsave.Parameters.Add("@PID", SqlDbType.Int).Value = PID;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);

            adp.Fill(dsClient);

            // output = Convert.ToString(results.remark);


            //  }
            return dsClient;




        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {

            Connection.Close();
        }
    }

    public DataSet SaveTokens(string opt, string id, string token)
    {
        DataTable dt = new DataTable();
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("mgrTokens", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;

            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = Convert.ToInt32(opt);
            cmdsave.Parameters.Add("@projecturlid", SqlDbType.Int).Value = Convert.ToInt32(id);
            cmdsave.Parameters.Add("@token", SqlDbType.VarChar, 100).Value = token;
            // cmdsave.Parameters.Add("@PID", SqlDbType.Int).Value = PID;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(dsClient);
            return dsClient;
        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {

            Connection.Close();
        }
    }

    public string findprojectid(int ID)
    {
        DataTable dt = new DataTable();
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("SELECT PID FROM dbo.ProjectsUrl pu WHERE id=" + ID, Connection);
            cmdsave.CommandType = CommandType.Text;

            // cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            // cmdsave.Parameters.Add("@PID", SqlDbType.Int).Value = PID;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);

            adp.Fill(dsClient);
            if (dsClient.Tables[0].Rows.Count > 0)
            {
                status = dsClient.Tables[0].Rows[0][0].ToString();
            }
            // output = Convert.ToString(results.remark);


            //  }
            return status;




        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {

            Connection.Close();
        }
    }
    public DataSet GetRedirects(int ID)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("getSuppRedirects", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(dsClient);
            return dsClient;
        }
        catch (SystemException ex)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }
    public DataSet GetProjectMappings(int PID)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("Usp_GetMappingProjectDetails", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;

            cmdsave.Parameters.Add("@PID", SqlDbType.Int).Value = PID;

            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);

            adp.Fill(dsClient);

            // output = Convert.ToString(results.remark);


            //  }
            return dsClient;




        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {

            Connection.Close();
        }
    }
    public string Updatestatussupplierprojects(string ID, string action, string sid)
    {
        string result;
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("mgrOverquota", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            SqlParameter paramOut = new SqlParameter("@output", SqlDbType.VarChar, 200);
            paramOut.Direction = ParameterDirection.Output;
            cmdsave.Parameters.Add("@id", SqlDbType.Int).Value = ID;
            cmdsave.Parameters.Add("@action", SqlDbType.VarChar, 200).Value = action;
            cmdsave.Parameters.Add("@ssid", SqlDbType.VarChar, 200).Value = sid;
            cmdsave.Parameters.Add(paramOut);
            // SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            // adp.Fill(dsClient);
            int x = cmdsave.ExecuteNonQuery();
            result = Convert.ToString(paramOut.Value);
            //  }
            return result;
        }
        catch (SystemException ex)
        {
            return "There is some error. Please contact administrator";
        }
        finally
        {
            Connection.Close();
        }
    }


    public string ValidationForTriggeringRecontact(string ID, string action, string sid)
    {
        string result;
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("mgrValidationRecontactlinktrigger", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            SqlParameter paramOut = new SqlParameter("@output", SqlDbType.VarChar, 200);
            paramOut.Direction = ParameterDirection.Output;
            cmdsave.Parameters.Add("@id", SqlDbType.Int).Value = ID;
            cmdsave.Parameters.Add("@action", SqlDbType.VarChar, 200).Value = action;
            cmdsave.Parameters.Add("@ssid", SqlDbType.VarChar, 200).Value = sid;
            cmdsave.Parameters.Add(paramOut);
            // SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            // adp.Fill(dsClient);
            int x = cmdsave.ExecuteNonQuery();
            result = Convert.ToString(paramOut.Value);
            //  }
            return result;
        }
        catch (SystemException ex)
        {
            return "There is some error. Please contact administrator";
        }
        finally
        {
            Connection.Close();
        }
    }


    public DataSet getprojectmapingdetailsbyid(string ID)
    {
        string result;
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("getpromapbyid", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;

            cmdsave.Parameters.Add("@id", SqlDbType.Int).Value = ID;

            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);

            adp.Fill(dsClient);
            return dsClient;
        }
        catch (SystemException ex)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }

    public DataSet AuthenticateIP(string SID, int OPT)
    {
        string result;
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("AutheticateIP", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;

            cmdsave.Parameters.Add("@sid", SqlDbType.VarChar, 10).Value = SID;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = OPT;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(dsClient);
            return dsClient;
        }
        catch (SystemException ex)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }

    public string GetIPFromDatabase(string IP, string Country_Code, string Country, string City, int opt)
    {
        string result;
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("IPManager", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;

            cmdsave.Parameters.Add("@IP", SqlDbType.VarChar, 30).Value = IP;
            cmdsave.Parameters.Add("@Country_Code", SqlDbType.VarChar, 100).Value = Country_Code;
            cmdsave.Parameters.Add("@Country", SqlDbType.VarChar, 100).Value = Country;
            cmdsave.Parameters.Add("@City", SqlDbType.VarChar, 100).Value = City;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(dsClient);
            if (dsClient.Tables.Count > 0)
            {
                return dsClient.Tables[0].Rows[0][0].ToString();
            }
            else
                return "0";
            // return dsClient;
        }
        catch (SystemException ex)
        {
            return "0";
        }
        finally
        {
            Connection.Close();
        }
    }

    public string ProjectMapping(int PID, int CID, int SupplierID, string OLINK, string MLINK, string SID, string code, int ID, int OPT, int respondants, float cpi, string notes, int AddHashing, string ParameterName, string HashingType)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {


            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("usp_GetProjectSupplierMapping", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            SqlParameter paramOut = new SqlParameter("@Result", SqlDbType.VarChar, 25);
            paramOut.Direction = ParameterDirection.Output;
            cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            cmdsave.Parameters.Add("@PID", SqlDbType.Int).Value = PID;
            cmdsave.Parameters.Add("@CID", SqlDbType.Int).Value = CID;
            cmdsave.Parameters.Add("@SuplierID", SqlDbType.Int).Value = SupplierID;
            cmdsave.Parameters.Add("@SID", SqlDbType.NVarChar, 50).Value = SID;
            cmdsave.Parameters.Add("@OLink", SqlDbType.NVarChar, 4000).Value = OLINK;
            cmdsave.Parameters.Add("@MLink", SqlDbType.NVarChar, 4000).Value = MLINK;
            cmdsave.Parameters.Add("@Code", SqlDbType.NVarChar, 40).Value = code;
            cmdsave.Parameters.Add("@Opt", SqlDbType.Int).Value = OPT;
            cmdsave.Parameters.Add("@respondants", SqlDbType.Int).Value = respondants;
            cmdsave.Parameters.Add("@CPI", SqlDbType.Float).Value = cpi;
            cmdsave.Parameters.Add("@Notes", SqlDbType.VarChar, 500).Value = notes;
            cmdsave.Parameters.Add("@AddHashing", SqlDbType.Int).Value = AddHashing;
            cmdsave.Parameters.Add("@ParameterName", SqlDbType.VarChar, 100).Value = ParameterName;
            cmdsave.Parameters.Add("@HashingType", SqlDbType.VarChar, 100).Value = HashingType;
            cmdsave.Parameters.Add(paramOut);
            drUser = cmdsave.ExecuteReader();
            output = Convert.ToString(paramOut.Value);
            // output = Convert.ToString(results.remark);


            //  }
            return output;




        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");
            return "Trandaction failed";
        }
        finally
        {
            drUser.Close();
            Connection.Close();
        }
    }
    public string UpdateProMap(int ID, int respondants, float cpi, string notes, int countryid, int supplierid, int projectid, int opt)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("updatemapping", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            SqlParameter paramOut = new SqlParameter("@output", SqlDbType.VarChar, 25);
            paramOut.Direction = ParameterDirection.Output;
            cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            cmdsave.Parameters.Add("@quota", SqlDbType.Int).Value = respondants;
            cmdsave.Parameters.Add("@cpi", SqlDbType.Float).Value = cpi;
            cmdsave.Parameters.Add("@notes", SqlDbType.VarChar, 500).Value = notes;
            cmdsave.Parameters.Add("@countryid", SqlDbType.Int).Value = countryid;
            cmdsave.Parameters.Add("@supplierid", SqlDbType.Int).Value = supplierid;
            cmdsave.Parameters.Add("@projectid", SqlDbType.Int).Value = projectid;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;

            cmdsave.Parameters.Add(paramOut);
            drUser = cmdsave.ExecuteReader();
            output = Convert.ToString(paramOut.Value);
            return output;
        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");
            return "Trandaction failed";
        }
        finally
        {
            drUser.Close();
            Connection.Close();
        }
    }
    public string UpdateRedirctsInProMap(CLSProjectmapping formdata)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("updatemapping", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            SqlParameter paramOut = new SqlParameter("@output", SqlDbType.VarChar, 25);
            paramOut.Direction = ParameterDirection.Output;
            cmdsave.Parameters.Add("@id", SqlDbType.Int).Value = Convert.ToInt32(formdata.ID);
            cmdsave.Parameters.Add("@trackingtype", SqlDbType.Int).Value = Convert.ToInt32(formdata.TrackingType);
            cmdsave.Parameters.Add("@complete", SqlDbType.VarChar, 500).Value = formdata.Completes;
            cmdsave.Parameters.Add("@terminate", SqlDbType.VarChar, 500).Value = formdata.Terminate;
            cmdsave.Parameters.Add("@overquota", SqlDbType.VarChar, 500).Value = formdata.Overquota;
            cmdsave.Parameters.Add("@security", SqlDbType.VarChar, 500).Value = formdata.Security;
            cmdsave.Parameters.Add("@fraud", SqlDbType.VarChar, 500).Value = formdata.Fraud;
            cmdsave.Parameters.Add("@success", SqlDbType.VarChar, 500).Value = formdata.SUCCESS;
            cmdsave.Parameters.Add("@defaultt", SqlDbType.VarChar, 500).Value = formdata.DEFAULT;
            cmdsave.Parameters.Add("@failure", SqlDbType.VarChar, 500).Value = formdata.FAILURE;
            cmdsave.Parameters.Add("@quality", SqlDbType.VarChar, 500).Value = formdata.QUALITY_TERMINATION;
            cmdsave.Parameters.Add("@overquotaP", SqlDbType.VarChar, 500).Value = formdata.OVER_QUOTA;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = 1;

            cmdsave.Parameters.Add(paramOut);
            drUser = cmdsave.ExecuteReader();
            output = Convert.ToString(paramOut.Value);
            return output;
        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");
            return "Trandaction failed";
        }
        finally
        {
            drUser.Close();
            Connection.Close();
        }
    }


    public string UpdateRecontactProjects(Recontact formdata, int opt)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("updateRecontactProjects", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            SqlParameter paramOut = new SqlParameter("@output", SqlDbType.VarChar, 25);
            paramOut.Direction = ParameterDirection.Output;
            cmdsave.Parameters.Add("@id", SqlDbType.Int).Value = Convert.ToInt32(formdata.ID);
            cmdsave.Parameters.Add("@pdescription", SqlDbType.VarChar, 100).Value = formdata.RecontactDescription;
            cmdsave.Parameters.Add("@loi", SqlDbType.Float).Value = float.Parse(formdata.LOI.ToString());
            cmdsave.Parameters.Add("@cpi", SqlDbType.Float).Value = formdata.CPI;
            cmdsave.Parameters.Add("@ir", SqlDbType.Float).Value = formdata.IR;
            cmdsave.Parameters.Add("@rcq", SqlDbType.Float).Value = formdata.RCQ;
            cmdsave.Parameters.Add("@notes", SqlDbType.VarChar, 500).Value = formdata.Notes;
            //cmdsave.Parameters.Add("@recontactname", SqlDbType.VarChar, 500).Value = formdata.RecontactName;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;

            cmdsave.Parameters.Add(paramOut);
            drUser = cmdsave.ExecuteReader();
            output = Convert.ToString(paramOut.Value);
            return output;
        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");
            return "Trandaction failed";
        }
        finally
        {
            drUser.Close();
            Connection.Close();
        }
    }


    public DataSet MgrQuota(string Quota, string opt, string projecturlid)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("QuotaMgr", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            //SqlParameter paramOut = new SqlParameter("@output", SqlDbType.VarChar, 25);
            //paramOut.Direction = ParameterDirection.Output;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = Convert.ToInt32(opt);
            cmdsave.Parameters.Add("@quota", SqlDbType.VarChar, 100).Value = Quota;
            cmdsave.Parameters.Add("@projecturlid", SqlDbType.Int).Value = Convert.ToInt32(projecturlid);
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            //cmdsave.Parameters.Add(paramOut);
            //drUser = cmdsave.ExecuteReader();
            adp.Fill(DsRech);
            //output = Convert.ToString(paramOut.Value);
            return DsRech;
        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");
            return null;
        }
        finally
        {
            // drUser.Close();
            Connection.Close();
        }
    }
    public string CountryPanelSize(int CountryID, int PanelSize, int SupplierID, int ID, int OPT)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {


            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("uspMgrSampleSize", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            SqlParameter paramOut = new SqlParameter("@Result", SqlDbType.VarChar, 25);
            paramOut.Direction = ParameterDirection.Output;
            cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            cmdsave.Parameters.Add("@SID", SqlDbType.Int).Value = SupplierID;
            cmdsave.Parameters.Add("@CID", SqlDbType.Int).Value = CountryID;
            cmdsave.Parameters.Add("@Size", SqlDbType.Int).Value = PanelSize;
            cmdsave.Parameters.Add("@Opt", SqlDbType.Int).Value = OPT;
            cmdsave.Parameters.Add(paramOut);
            drUser = cmdsave.ExecuteReader();
            output = Convert.ToString(paramOut.Value);
            // output = Convert.ToString(results.remark);


            //  }
            return output;




        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");
            return "Trandaction failed";
        }
        finally
        {
            drUser.Close();
            Connection.Close();
        }
    }

    public string UpdateProjectURL(int CID, int PID, string Url, int ID, int OPT, string notes, string quota, string cpi, int Token)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {


            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("uspMgrProjectUrl", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            SqlParameter paramOut = new SqlParameter("@Result", SqlDbType.VarChar, 25);
            paramOut.Direction = ParameterDirection.Output;
            cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            cmdsave.Parameters.Add("@PID", SqlDbType.Int).Value = PID;
            cmdsave.Parameters.Add("@CID", SqlDbType.Int).Value = CID;
            cmdsave.Parameters.Add("@Url", SqlDbType.NVarChar, 4000).Value = Url;
            cmdsave.Parameters.Add("@Opt", SqlDbType.Int).Value = OPT;
            cmdsave.Parameters.Add("@notes", SqlDbType.VarChar, 500).Value = notes;
            cmdsave.Parameters.Add("@Quota", SqlDbType.VarChar, 500).Value = quota;
            cmdsave.Parameters.Add("@CPI", SqlDbType.Float).Value = float.Parse(cpi);
            cmdsave.Parameters.Add("@token", SqlDbType.Int).Value = Token;
            cmdsave.Parameters.Add(paramOut);
            drUser = cmdsave.ExecuteReader();
            output = Convert.ToString(paramOut.Value);
            // output = Convert.ToString(results.remark);


            //  }
            return output;




        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");
            return "Trandaction failed";
        }
        finally
        {
            drUser.Close();
            Connection.Close();
        }
    }

    public string ProjectUrl(int CID, int PID, string Url, int ID, int OPT, string notes, string quota, string CPI)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {


            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("uspMgrProjectUrl", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            SqlParameter paramOut = new SqlParameter("@Result", SqlDbType.VarChar, 25);
            paramOut.Direction = ParameterDirection.Output;
            cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            cmdsave.Parameters.Add("@PID", SqlDbType.Int).Value = PID;
            cmdsave.Parameters.Add("@CID", SqlDbType.Int).Value = CID;
            cmdsave.Parameters.Add("@Url", SqlDbType.NVarChar, 4000).Value = Url;
            cmdsave.Parameters.Add("@Opt", SqlDbType.Int).Value = OPT;
            cmdsave.Parameters.Add("@notes", SqlDbType.VarChar, 500).Value = notes;
            cmdsave.Parameters.Add("@Quota", SqlDbType.VarChar, 500).Value = quota;
            cmdsave.Parameters.Add("@CPI", SqlDbType.Float).Value = float.Parse(CPI);
            cmdsave.Parameters.Add(paramOut);
            drUser = cmdsave.ExecuteReader();
            output = Convert.ToString(paramOut.Value);
            // output = Convert.ToString(results.remark);


            //  }
            return output;




        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");
            return "Trandaction failed";
        }
        finally
        {
            drUser.Close();
            Connection.Close();
        }
    }


    public string UpdateProjectDetails(string ClientIP, string ClientBrowser, string Status, string SID, string Code, int IsUsed, int Type)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {


            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("Usp_UpdateBrowserDetails", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            SqlParameter paramOut = new SqlParameter("@Result", SqlDbType.VarChar, 25);
            paramOut.Direction = ParameterDirection.Output;
            //  cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = objP.ID;
            cmdsave.Parameters.Add("@IP", SqlDbType.NVarChar, 50).Value = ClientIP;
            cmdsave.Parameters.Add("@Browser", SqlDbType.NVarChar, 50).Value = ClientBrowser;
            cmdsave.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value = Status;
            cmdsave.Parameters.Add("@IsUsed", SqlDbType.Int).Value = IsUsed;
            cmdsave.Parameters.Add("@Code", SqlDbType.NVarChar, 50).Value = Code;
            cmdsave.Parameters.Add("@SID", SqlDbType.NVarChar, 50).Value = SID;
            cmdsave.Parameters.Add("@Opt", SqlDbType.Int).Value = Type;
            cmdsave.Parameters.Add(paramOut);
            drUser = cmdsave.ExecuteReader();
            output = Convert.ToString(paramOut.Value);
            // output = Convert.ToString(results.remark);


            //  }
            return output;




        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");
            return "Trandaction failed";
        }
        finally
        {
            drUser.Close();
            Connection.Close();
        }
    }

    public string SaveSupplierProject(string ClientIP, string ClientBrowser, string Status, string SID, string Code, string UID, string PrevUID, string Device, int opt, string enc)
    {
        //ClsDAL.WriteErrorLog("ClientIP - " + ClientIP + ":: ClientBrowser - " + ClientBrowser + ":: Status - " + Status + ":: SID - " + SID + ":: Code - " + Code + ":: UID - " + UID + ":: PrevUID - " + PrevUID + ":: Device - " + Device + ":: opt - " + opt);
        //DataSet DsRech = new DataSet();
        // string status = "";
        try
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("Usp_GetSupplierLink", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            SqlParameter paramOut = new SqlParameter("@Result", SqlDbType.VarChar, 25);
            paramOut.Direction = ParameterDirection.Output;
            //  cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = objP.ID;
            cmdsave.Parameters.Add("@PMID", SqlDbType.NVarChar, 50).Value = Code;
            cmdsave.Parameters.Add("@ClientIP", SqlDbType.NVarChar, 50).Value = ClientIP;
            cmdsave.Parameters.Add("@ClientBrowser", SqlDbType.NVarChar, 50).Value = ClientBrowser;
            cmdsave.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value = Status;
            // cmdsave.Parameters.Add("@IsUsed", SqlDbType.Int).Value = IsUsed;
            cmdsave.Parameters.Add("@UID", SqlDbType.NVarChar, 50).Value = UID;
            cmdsave.Parameters.Add("@SID", SqlDbType.NVarChar, 50).Value = SID;
            cmdsave.Parameters.Add("@PrevUID", SqlDbType.NVarChar, 50).Value = PrevUID;
            cmdsave.Parameters.Add("@clientdevice", SqlDbType.NVarChar, 50).Value = Device;
            cmdsave.Parameters.Add("@enc", SqlDbType.NVarChar, 250).Value = enc;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;
            // cmdsave.Parameters.Add("@Opt", SqlDbType.Int).Value = Type;
            cmdsave.Parameters.Add(paramOut);
            drSupplier = cmdsave.ExecuteReader();
            output = Convert.ToString(paramOut.Value);
            // output = Convert.ToString(results.remark);


            //  }
            return output;




        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");
            return "Trandaction failed";
        }
        finally
        {
            drSupplier.Close();
            Connection.Close();
        }
    }
    public string SaveSupplierProjectForRecontact(string ClientIP, string ClientBrowser, string Status, string SID, string Code, string UID, string PrevUID, string Device, int opt, string PID, string Var1, string Var2, string Var3, string Var4, string Var5)
    {
        try
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("Usp_GetSupplierLink", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            SqlParameter paramOut = new SqlParameter("@Result", SqlDbType.VarChar, 25);
            paramOut.Direction = ParameterDirection.Output;
            //  cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = objP.ID;
            cmdsave.Parameters.Add("@PMID", SqlDbType.NVarChar, 50).Value = Code;
            cmdsave.Parameters.Add("@ClientIP", SqlDbType.NVarChar, 50).Value = ClientIP;
            cmdsave.Parameters.Add("@ClientBrowser", SqlDbType.NVarChar, 50).Value = ClientBrowser;
            cmdsave.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value = Status;
            // cmdsave.Parameters.Add("@IsUsed", SqlDbType.Int).Value = IsUsed;
            cmdsave.Parameters.Add("@UID", SqlDbType.NVarChar, 50).Value = UID;
            cmdsave.Parameters.Add("@SID", SqlDbType.NVarChar, 50).Value = SID;
            cmdsave.Parameters.Add("@PrevUID", SqlDbType.NVarChar, 50).Value = PrevUID;
            cmdsave.Parameters.Add("@clientdevice", SqlDbType.NVarChar, 50).Value = Device;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;
            cmdsave.Parameters.Add("@newPMID", SqlDbType.NVarChar, 50).Value = PID;
            cmdsave.Parameters.Add("@Var1", SqlDbType.NVarChar, 50).Value = Var1;
            cmdsave.Parameters.Add("@Var2", SqlDbType.NVarChar, 50).Value = Var2;
            cmdsave.Parameters.Add("@Var3", SqlDbType.NVarChar, 50).Value = Var3;
            cmdsave.Parameters.Add("@Var4", SqlDbType.NVarChar, 50).Value = Var4;
            cmdsave.Parameters.Add("@Var5", SqlDbType.NVarChar, 50).Value = Var5;
            // cmdsave.Parameters.Add("@Opt", SqlDbType.Int).Value = Type;
            cmdsave.Parameters.Add(paramOut);
            drSupplier = cmdsave.ExecuteReader();
            output = Convert.ToString(paramOut.Value);
            // output = Convert.ToString(results.remark);


            //  }
            return output;




        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");
            return "Trandaction failed";
        }
        finally
        {
            drSupplier.Close();
            Connection.Close();
        }
    }
    public DataSet GetSupplierUrl(string SID, string Code, int opt)
    {
        DataSet DsRech = new DataSet();

        string status = "";
        try
        {
            dsClient.Clear();

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            string query = "";
            SqlCommand cmdsave = new SqlCommand("Usp_GetSupplierUrl", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.CommandTimeout = 1000;
            cmdsave.Parameters.Add("@SID", SqlDbType.NVarChar, 50).Value = SID;
            cmdsave.Parameters.Add("@PMID", SqlDbType.NVarChar, 50).Value = Code;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;
            // cmdsave.Parameters.Add("@ID", SqlDbType.NVarChar, 50).Value = NID;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);

            adp.Fill(DsRech);

            // output = Convert.ToString(results.remark);


            //  }
            return DsRech;




        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {

            Connection.Close();
        }
    }

    public DataSet GetVariables(string UID, int opt, int ID, string VarValue)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            string query = "";
            SqlCommand cmdsave = new SqlCommand("mgrVariables", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@uid", SqlDbType.NVarChar, 255).Value = UID;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;
            cmdsave.Parameters.Add("@VarValue", SqlDbType.NVarChar, 255).Value = VarValue;
            cmdsave.Parameters.Add("@id", SqlDbType.Int).Value = ID;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(dsClient);
            return dsClient;
        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {

            Connection.Close();
        }
    }
    public DataSet UpdateVariables(string UID, int opt, int ID, string VarValue)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            string query = "UPDATE dbo.URLVariables SET VariableValue = '" + VarValue + "' where id=" + ID;
            SqlCommand cmdsave = new SqlCommand(query, Connection);
            cmdsave.CommandType = CommandType.Text;
            //cmdsave.Parameters.Add("@uid", SqlDbType.NVarChar, 255).Value = UID;
            //cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;
            //cmdsave.Parameters.Add("@VarValue", SqlDbType.NVarChar, 255).Value = VarValue;
            //cmdsave.Parameters.Add("@id", SqlDbType.Int).Value = ID;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(DsRech);
            return DsRech;
        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {

            Connection.Close();
        }
    }
    public DataSet GetFractionComplete(string projectid, int opt)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("getCompleteFraction", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@projectid", SqlDbType.Int).Value = Convert.ToInt32(projectid);
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(dsClient);
            return dsClient;
        }
        catch (SystemException ex)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }

    public string ChangePassword(string UID, string Password, string NewPassword)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("Usp_ChangePassword", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            SqlParameter paramOut = new SqlParameter("@Result", SqlDbType.VarChar, 25);
            paramOut.Direction = ParameterDirection.Output;
            cmdsave.Parameters.Add("@uid", SqlDbType.VarChar, 50).Value = UID;
            cmdsave.Parameters.Add("@OldPwd", SqlDbType.NVarChar, 50).Value = Password;
            cmdsave.Parameters.Add("@NewPwd", SqlDbType.NVarChar, 50).Value = NewPassword;
            //cmdsave.Parameters.Add("@Url", SqlDbType.NVarChar, 4000).Value = Url;
            //cmdsave.Parameters.Add("@Opt", SqlDbType.Int).Value = OPT;
            cmdsave.Parameters.Add(paramOut);
            drUser = cmdsave.ExecuteReader();
            output = Convert.ToString(paramOut.Value);
            return output;
        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");
            return "Trandaction failed";
        }
        finally
        {
            drUser.Close();
            Connection.Close();
        }
    }
    public DataSet getmanager()
    {
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlDataAdapter adp = new SqlDataAdapter("GetManager", Connection);
            adp.Fill(dsClient);
            return dsClient;
        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");
        }
        finally
        {
            Connection.Close();
        }
    }
    public DataSet GetDeshBoard(string id)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("usp_GetDashboardDetails", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;

            cmdsave.Parameters.Add("@manager", SqlDbType.Int).Value = Convert.ToInt32(id);

            //SqlDataAdapter adp = new SqlDataAdapter("usp_GetDashboardDetails", Connection);
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(dsClient);

            // output = Convert.ToString(results.remark);


            //  }
            return dsClient;




        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {

            Connection.Close();
        }
    }

    public int CheckForPixelTracking(string ID, int opt, int PID)
    {
        try
        {
            DataSet ds = new DataSet();
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("CheckPixelTracking", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@ID", SqlDbType.NVarChar, 50).Value = ID;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;
            cmdsave.Parameters.Add("@pid", SqlDbType.Int).Value = PID;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(ds);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                }
            }
            return -1;
        }
        catch (Exception e)
        {
            return -1;
        }
    }

    public DataSet GetRequestUrl(string ID, int opt, int PID)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("usp_ShareStatusOnRequest", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@ID", SqlDbType.NVarChar, 50).Value = ID;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;
            cmdsave.Parameters.Add("@pid", SqlDbType.Int).Value = PID;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(dsClient);
            // output = Convert.ToString(results.remark);
            //  }
            return dsClient;
        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {

            Connection.Close();
        }
    }

    public DataSet GetRequestUrlImmidiate(string ID, int opt, string Status)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("usp_StatusImmidiateOError", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;

            cmdsave.Parameters.Add("@ID", SqlDbType.NVarChar, 50).Value = ID;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;
            cmdsave.Parameters.Add("@Status", SqlDbType.VarChar, 50).Value = Status;

            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);

            adp.Fill(dsClient);

            // output = Convert.ToString(results.remark);


            //  }
            return dsClient;




        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {

            Connection.Close();
        }
    }
    public string UpdateRequestStatus(string ID)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("update [dbo].[SupplierProjects] set IsSent=1 where [UID]='" + ID + "'", Connection);
            cmdsave.CommandType = CommandType.Text;
            // cmdsave.Parameters.Add("@ID", SqlDbType.NVarChar, 50).Value = ID;
            cmdsave.ExecuteNonQuery();
            return "1";
        }
        catch (SystemException ex)
        {
            return "0";
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {

            Connection.Close();
        }
    }

    public DataSet GetSupplierStatusReport(int CID, string FDate, string TDate, int Status)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("Usp_SupplierReport", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;

            cmdsave.Parameters.Add("@FDate", SqlDbType.NVarChar, 50).Value = FDate;
            cmdsave.Parameters.Add("@TDate", SqlDbType.NVarChar, 50).Value = TDate;
            cmdsave.Parameters.Add("@CountryID", SqlDbType.Int).Value = CID;
            cmdsave.Parameters.Add("@Status", SqlDbType.Int).Value = Status;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);

            adp.Fill(dsClient);

            // output = Convert.ToString(results.remark);


            //  }
            return dsClient;




        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {

            Connection.Close();
        }
    }
    public string GetProjectId()
    {
        DataTable dt = new DataTable();
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("GetMaxPID", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;

            //cmdsave.Parameters.Add("@FDate", SqlDbType.NVarChar, 50).Value = FDate;
            //cmdsave.Parameters.Add("@TDate", SqlDbType.NVarChar, 50).Value = TDate;
            //cmdsave.Parameters.Add("@CountryID", SqlDbType.Int).Value = CID;
            //cmdsave.Parameters.Add("@Status", SqlDbType.Int).Value = Status;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);

            adp.Fill(dt);
            string pid = "";
            if (dt.Rows.Count > 0)
            {
                pid = dt.Rows[0][0].ToString();
            }

            // output = Convert.ToString(results.remark);


            //  }
            return pid;




        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {

            Connection.Close();
        }
    }

    public static void WriteErrorLog(string strError)
    {
        try
        {
            //string strLogPath= Server.MapPath("~/ErrorLogs/");
            string strLogPath = System.Web.Hosting.HostingEnvironment.MapPath("~/ErrorLogs/");



            string strFileNameSuffix = DateTime.Today.Date.Day.ToString() + "_" + DateTime.Today.Date.Month.ToString() + "_" + DateTime.Today.Date.Year.ToString() + ".log";
            string strLine = "";

            string strFileName = strLogPath + strFileNameSuffix;

            //DirectoryInfo logdir = new DirectoryInfo(strFileName);
            //if (!logdir.Exists)
            //    logdir.Create();
            strLine = DateTime.Now.ToString() + "|" + strError;


            if (!File.Exists(strFileName))
            {
                FileStream f = File.Create(strFileName);
                f.Close();
            }
            using (StreamWriter sw = File.AppendText(strFileName))
            {
                sw.WriteLine(strLine);
                // tw.Close();
                //tw.Dispose();
            }

        }
        catch (Exception ex)
        {

            // CreateLogFiles Err = new CreateLogFiles();
            // ErrorLog(Server.MapPath("ErrorLog"), ex.Message);
            // ClsCommon.WriteErrorLog("Error occured in Add User page on GetUserDetails()event : " + ex.Message);
            //string ErrMsg = ex.Message ;
        }
    }
    //profile details
    public DataTable getuserprofiledetails(string ID)
    {
        DataTable dt = new DataTable();
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("usp_getuserprofile", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;

            cmdsave.Parameters.Add("@ID", SqlDbType.NVarChar, 50).Value = ID;

            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);

            adp.Fill(dt);

            // output = Convert.ToString(results.remark);


            //  }
            return dt;




        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {
            Connection.Close();
        }
    }

    public DataSet ManageCompleteRedirects(string Code, int opt, int Id)
    {
        DataTable dt = new DataTable();
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("ManageCompleteRedirects", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@Code", SqlDbType.NVarChar, 500).Value = Code;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;
            cmdsave.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(DsRech);
            return DsRech;
        }
        catch (SystemException ex)
        {
            return null;
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");

        }
        finally
        {
            Connection.Close();
        }
    }

    public DataSet InsertRecontact(string recontactname, string CPI, string Notes, string recon_description, int Status, float LOI, float IR, int RCQ, string SID, string MURL, string PID, int opt)
    {
        DataTable dt = new DataTable();
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("Recontactmgr", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@recontactname", SqlDbType.VarChar, 200).Value = recontactname;
            cmdsave.Parameters.Add("@CPI", SqlDbType.Int).Value = float.Parse(CPI);
            cmdsave.Parameters.Add("@SID", SqlDbType.VarChar, 255).Value = SID;
            cmdsave.Parameters.Add("@Notes", SqlDbType.VarChar, 255).Value = Notes;
            cmdsave.Parameters.Add("@MURL", SqlDbType.VarChar, 255).Value = MURL;
            cmdsave.Parameters.Add("@PID", SqlDbType.VarChar, 15).Value = PID;
            cmdsave.Parameters.Add("@recontactdescription", SqlDbType.VarChar, 100).Value = recon_description;
            cmdsave.Parameters.Add("@status", SqlDbType.Int).Value = Status;
            cmdsave.Parameters.Add("@loi", SqlDbType.Float).Value = LOI;
            cmdsave.Parameters.Add("@IR", SqlDbType.Float).Value = IR;
            cmdsave.Parameters.Add("@rcq", SqlDbType.Float).Value = RCQ;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(DsRech);
            return DsRech;
        }
        catch (SystemException ex)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }
    public DataSet InsertVariables(string VariableName, int id, int opt)
    {
        DataTable dt = new DataTable();
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("mgrVariables", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@variablename", SqlDbType.VarChar, 200).Value = VariableName;
            cmdsave.Parameters.Add("@rcid", SqlDbType.Int).Value = id;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(DsRech);
            return DsRech;
        }
        catch (SystemException ex)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }
    public DataSet ValidateUIDforRecontact(string UID, int opt, string SID)
    {
        DataTable dt = new DataTable();
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("validateUID", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@uid", SqlDbType.NVarChar, 255).Value = UID;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;
            cmdsave.Parameters.Add("@SID", SqlDbType.NVarChar, 50).Value = SID;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(DsRech);
            return DsRech;
        }
        catch (SystemException ex)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }
    public bool CheckUIDForRecontact(string UID, string SID)
    {
        DataTable dt = new DataTable();
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            string query = "SELECT count(*) FROM dbo.RecontactProjects rp INNER JOIN dbo.RecontactProjectsIds rp2  ON rp.id=rp2.recontactprojectsid INNER JOIN dbo.ProjectMapping pm ON pm.id = rp.ProMapId INNER JOIN dbo.SupplierProjects sp ON pm.sid = sp.sid WHERE sp.uid = '" + UID + "' AND pm.SID = '" + SID + "' AND rp2.uid = '" + UID + "'";
            SqlCommand cmdsave = new SqlCommand(query, Connection);
            cmdsave.CommandType = CommandType.Text;
            //cmdsave.Parameters.Add("@projectid", SqlDbType.Int).Value = Convert.ToInt32(PID);
            //cmdsave.Parameters.Add("@countryid", SqlDbType.Int).Value = Convert.ToInt32(CID);
            //cmdsave.Parameters.Add("@supplierid", SqlDbType.Int).Value = Convert.ToInt32(SID);
            //cmdsave.Parameters.Add("@RCQ", SqlDbType.Int).Value = Convert.ToInt32(RCQ);
            //cmdsave.Parameters.Add("@CPI", SqlDbType.Int).Value = float.Parse(CPI);
            //cmdsave.Parameters.Add("@uids", SqlDbType.VarChar, 255).Value = ids;
            //cmdsave.Parameters.Add("@Notes", SqlDbType.VarChar, 255).Value = Notes;
            //cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(DsRech);
            if (DsRech.Tables.Count > 0)
            {
                if (DsRech.Tables[0].Rows.Count > 0)
                {
                    return Convert.ToInt32(DsRech.Tables[0].Rows[0][0]) > 0 ? true : false;
                }
                else return false;
            }
            else return false;


        }
        catch (SystemException ex)
        {
            return false;
        }
        finally
        {
            Connection.Close();
        }
    }

    public DataSet LoadRecontactProjects(string PID, string CID, string SID, int opt)
    {
        DataTable dt = new DataTable();
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("FetchRecontact", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@projectid", SqlDbType.Int).Value = Convert.ToInt32(PID);
            cmdsave.Parameters.Add("@countryid", SqlDbType.Int).Value = Convert.ToInt32(CID);
            cmdsave.Parameters.Add("@supplierid", SqlDbType.Int).Value = Convert.ToInt32(SID);
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(DsRech);
            return DsRech;
        }
        catch (SystemException ex)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }
    public DataSet DeleteRecontact(string uid, int ID, int opt)
    {
        DataTable dt = new DataTable();
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("DELETERecontactprojects", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@id", SqlDbType.Int).Value = Convert.ToInt32(ID);
            cmdsave.Parameters.Add("@uid", SqlDbType.VarChar, 255).Value = uid;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(DsRech);
            return DsRech;
        }
        catch (SystemException ex)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }

    public DataSet UpdateRecontactProjectIds(int opt, string PMID, string UIDnew, string UID)
    {
        DataTable dt = new DataTable();
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("updateRecontactProjectsIDs", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = Convert.ToInt32(opt);
            cmdsave.Parameters.Add("@PMID", SqlDbType.VarChar, 255).Value = PMID;
            cmdsave.Parameters.Add("@UIDnew", SqlDbType.VarChar, 255).Value = UIDnew;
            cmdsave.Parameters.Add("@UID", SqlDbType.VarChar, 255).Value = UID;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(DsRech);
            return DsRech;
        }
        catch (SystemException ex)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }
    public DataSet UpdateRecontactRespondantstbl(string UID, string RespondantLink, string Var1, string Var2, string Var3, string Var4, string Var5, string PMID, string MURL, string id, string SID, int projectmappingid, int projectid, int countryid, int supplierid, int opt)
    {
        DataTable dt = new DataTable();
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("RecontactRespondanttbl", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;
            cmdsave.Parameters.Add("@UID", SqlDbType.VarChar, 255).Value = UID;
            cmdsave.Parameters.Add("@RedirectLink", SqlDbType.VarChar, 255).Value = RespondantLink;
            cmdsave.Parameters.Add("@Var1", SqlDbType.VarChar, 255).Value = Var1;
            cmdsave.Parameters.Add("@Var2", SqlDbType.VarChar, 255).Value = Var2;
            cmdsave.Parameters.Add("@Var3", SqlDbType.VarChar, 255).Value = Var3;
            cmdsave.Parameters.Add("@Var4", SqlDbType.VarChar, 255).Value = Var4;
            cmdsave.Parameters.Add("@Var5", SqlDbType.VarChar, 255).Value = Var5;
            cmdsave.Parameters.Add("@PMID", SqlDbType.VarChar, 255).Value = PMID;
            cmdsave.Parameters.Add("@MURL", SqlDbType.VarChar, 255).Value = MURL;
            cmdsave.Parameters.Add("@id", SqlDbType.VarChar, 10).Value = id;
            cmdsave.Parameters.Add("@SID", SqlDbType.VarChar, 10).Value = SID;
            cmdsave.Parameters.Add("@projectmappingid", SqlDbType.Int).Value = projectmappingid;
            cmdsave.Parameters.Add("@projectid", SqlDbType.Int).Value = projectid;
            cmdsave.Parameters.Add("@countryid", SqlDbType.Int).Value = countryid;
            cmdsave.Parameters.Add("@supplierid", SqlDbType.Int).Value = supplierid;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(DsRech);
            return DsRech;
        }
        catch (SystemException ex)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }

    public DataSet UpdateRecontactProjects(string UID, string RespondantLink, string MURL, int id, int opt)
    {
        DataTable dt = new DataTable();
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            dsClient.Clear();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("UpdateRecontctRespondants", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            cmdsave.Parameters.Add("@opt", SqlDbType.Int).Value = opt;
            cmdsave.Parameters.Add("@uid", SqlDbType.VarChar, 255).Value = UID;
            cmdsave.Parameters.Add("@clientURL", SqlDbType.VarChar, 255).Value = RespondantLink;
            cmdsave.Parameters.Add("@maskingURL", SqlDbType.VarChar, 255).Value = MURL;
            cmdsave.Parameters.Add("@rpid", SqlDbType.Int).Value = id;
            SqlDataAdapter adp = new SqlDataAdapter(cmdsave);
            adp.Fill(DsRech);
            return DsRech;
        }
        catch (SystemException ex)
        {
            return null;
        }
        finally
        {
            Connection.Close();
        }
    }


    public SqlDataReader ValidateUser(string uname, string pwd)
    {
        // SqlConnection con = null;
        SqlCommand cmd = null;
        SqlDataReader dr = null;
        try
        {

            // con = new SqlConnection(ConfigurationManager.AppSettings["Con"].ToString());
            cmd = new SqlCommand("loginUser", Connection);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add("@UserName", SqlDbType.VarChar).Value = uname;
            cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = pwd;

            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }
            dr = cmd.ExecuteReader();
            //   dr.Read();

            return dr;
        }
        catch (Exception ex)
        {
            return null;
        }
        finally
        {
            // dr.Close();
            //if (con.State == ConnectionState.Open)
            //    con.Close();
            cmd = null;
            // dr = null;
        }
    }
    /////////////////////////////////////////////////////////////////////////// --------Pre Screening-----------------/////////////////////////////////////////////////////////////////////
    public string MgrQuestionsOptions(int ID, int QID, string OLabel, string OCode, int OPT)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("MgrQuestionOptions", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            SqlParameter paramOut = new SqlParameter("@Result", SqlDbType.VarChar, 25);
            paramOut.Direction = ParameterDirection.Output;
            cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            cmdsave.Parameters.Add("@QID", SqlDbType.Int).Value = QID;
            cmdsave.Parameters.Add("@QLabel", SqlDbType.NVarChar, 500).Value = OLabel;
            cmdsave.Parameters.Add("@QCode", SqlDbType.NVarChar, 500).Value = OCode;
            cmdsave.Parameters.Add("@Opt", SqlDbType.Int).Value = OPT;
            cmdsave.Parameters.Add(paramOut);
            drUser = cmdsave.ExecuteReader();
            output = Convert.ToString(paramOut.Value);
            return output;
        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");
            return "Trandaction failed";
        }
        finally
        {
            drUser.Close();
            Connection.Close();
        }
    }
    public string PreScreening(int ID, string QID, string Qlabel, int QType, int OPT)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("uspMgrScreening", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            SqlParameter paramOut = new SqlParameter("@Result", SqlDbType.VarChar, 25);
            paramOut.Direction = ParameterDirection.Output;
            cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            cmdsave.Parameters.Add("@QID", SqlDbType.NVarChar, 50).Value = QID;
            cmdsave.Parameters.Add("@QLabel", SqlDbType.NVarChar, 500).Value = Qlabel;
            cmdsave.Parameters.Add("@QType", SqlDbType.Int).Value = QType;
            //cmdsave.Parameters.Add("@OptionNo", SqlDbType.Int).Value = Option;
            //cmdsave.Parameters.Add("@OptionLabel", SqlDbType.NVarChar, 500).Value = OLabel;
            //cmdsave.Parameters.Add("@OptionCode", SqlDbType.NVarChar, 50).Value = OCode;
            cmdsave.Parameters.Add("@Opt", SqlDbType.Int).Value = OPT;
            cmdsave.Parameters.Add(paramOut);
            drUser = cmdsave.ExecuteReader();
            output = Convert.ToString(paramOut.Value);
            return output;
        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");
            return "Trandaction failed";
        }
        finally
        {
            drUser.Close();
            Connection.Close();
        }
    }
    public DataSet GetOptions(int QID)
    {
        try
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdusers = new SqlCommand();
            cmdusers.CommandText = "GetQuestionOptions";
            cmdusers.CommandType = CommandType.StoredProcedure;
            //cmdusers.Parameters.Add("@uid", SqlDbType.BigInt).Value = uid;
            cmdusers.Parameters.Add("@QID", SqlDbType.VarChar, 100).Value = QID;
            cmdusers.Connection = Connection;
            //cmdclient.CommandTimeout =0;
            SqlDataAdapter adap = new SqlDataAdapter(cmdusers);
            DataSet ds = new DataSet();
            adap.Fill(ds);
            return ds;
        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in GetDataMap()");
            return null;
        }
        finally
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
        }
    }
    public DataSet GetQuestions(int ID)
    {
        try
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }

            SqlCommand cmdusers = new SqlCommand();


            // cmdusers.CommandText = "Select * from dbo.[user] where adminid!=u_code and adminid =" + uid;
            cmdusers.CommandText = "usp_GetQuestions";
            cmdusers.CommandType = CommandType.StoredProcedure;
            //cmdusers.Parameters.Add("@uid", SqlDbType.BigInt).Value = uid;
            cmdusers.Parameters.Add("@QID", SqlDbType.VarChar, 100).Value = "";
            cmdusers.Parameters.Add("@ID", SqlDbType.VarChar, 100).Value = ID;



            cmdusers.Connection = Connection;
            //cmdclient.CommandTimeout =0;
            SqlDataAdapter adap = new SqlDataAdapter(cmdusers);
            DataSet ds = new DataSet();
            adap.Fill(ds);
            return ds;
        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in GetDataMap()");
            return null;
        }
        finally
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
        }
    }
    public DataSet GetSurveyQuestions(int QID, int PID, int Action, int CID)
    {
        try
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }

            SqlCommand cmdusers = new SqlCommand();


            // cmdusers.CommandText = "Select * from dbo.[user] where adminid!=u_code and adminid =" + uid;
            cmdusers.CommandText = "usp_GetSurveyProjectDetails";
            cmdusers.CommandType = CommandType.StoredProcedure;
            //cmdusers.Parameters.Add("@uid", SqlDbType.BigInt).Value = uid;
            cmdusers.Parameters.Add("@PID", SqlDbType.Int).Value = PID;
            cmdusers.Parameters.Add("@QstID", SqlDbType.Int).Value = QID;
            cmdusers.Parameters.Add("@Act", SqlDbType.Int).Value = Action;
            cmdusers.Parameters.Add("@CountryID", SqlDbType.Int).Value = CID;



            cmdusers.Connection = Connection;
            //cmdclient.CommandTimeout =0;
            SqlDataAdapter adap = new SqlDataAdapter(cmdusers);
            DataSet ds = new DataSet();
            adap.Fill(ds);
            return ds;
        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in GetDataMap()");
            return null;
        }
        finally
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
        }
    }
    public string MgrQuestionSelectedOptions(int ID, int QID, string QOptions, int PID, string UID)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {


            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("usp_MgrQuestionOptions", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            SqlParameter paramOut = new SqlParameter("@Result", SqlDbType.VarChar, 25);
            paramOut.Direction = ParameterDirection.Output;
            cmdsave.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            cmdsave.Parameters.Add("@QID", SqlDbType.Int).Value = QID;
            cmdsave.Parameters.Add("@QOptions", SqlDbType.NVarChar, 500).Value = QOptions;
            cmdsave.Parameters.Add("@PID", SqlDbType.Int).Value = PID;
            cmdsave.Parameters.Add("@UID", SqlDbType.NVarChar, 255).Value = UID;
            cmdsave.Parameters.Add(paramOut);
            drUser = cmdsave.ExecuteReader();
            output = Convert.ToString(paramOut.Value);
            // output = Convert.ToString(results.remark);


            //  }
            return output;




        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");
            return "Trandaction failed";
        }
        finally
        {
            drUser.Close();
            Connection.Close();
        }
    }
    public string CheckForTerminate(int PID, int CID, string Options, int QuestionId)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdsave = new SqlCommand("usp_CheckForTerminate", Connection);
            cmdsave.CommandType = CommandType.StoredProcedure;
            SqlParameter paramOut = new SqlParameter("@Result", SqlDbType.VarChar, 25);
            paramOut.Direction = ParameterDirection.Output;
            cmdsave.Parameters.Add("@PID", SqlDbType.Int).Value = PID;
            cmdsave.Parameters.Add("@CID", SqlDbType.Int).Value = CID;
            cmdsave.Parameters.Add("@Options", SqlDbType.NVarChar, 500).Value = Options;
            cmdsave.Parameters.Add("@QuestionId", SqlDbType.Int).Value = QuestionId;
            cmdsave.Parameters.Add(paramOut);
            drUser = cmdsave.ExecuteReader();
            output = Convert.ToString(paramOut.Value);
            return output;
        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");
            return "Trandaction failed";
        }
        finally
        {
            drUser.Close();
            Connection.Close();
        }
    }
    public DataSet GetQuestionSelectedOptions(int QID, int PID, string UID)
    {
        try
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }

            SqlCommand cmdusers = new SqlCommand();


            // cmdusers.CommandText = "Select * from dbo.[user] where adminid!=u_code and adminid =" + uid;
            cmdusers.CommandText = "usp_GetSelectedOptions";
            cmdusers.CommandType = CommandType.StoredProcedure;
            //cmdusers.Parameters.Add("@uid", SqlDbType.BigInt).Value = uid;
            cmdusers.Parameters.Add("@PID", SqlDbType.Int).Value = PID;
            cmdusers.Parameters.Add("@QID", SqlDbType.Int).Value = QID;
            cmdusers.Parameters.Add("@UID", SqlDbType.NVarChar, 255).Value = UID;
            cmdusers.Connection = Connection;
            //cmdclient.CommandTimeout =0;
            SqlDataAdapter adap = new SqlDataAdapter(cmdusers);
            DataSet ds = new DataSet();
            adap.Fill(ds);
            return ds;
        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in GetDataMap()");
            return null;
        }
        finally
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
        }
    }
    public DataSet GetQuestions(ClsPreScreening formData, int opt)
    {
        try
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdusers = new SqlCommand();
            cmdusers.CommandText = "GetQuestions";
            cmdusers.CommandType = CommandType.StoredProcedure;
            //cmdusers.Parameters.Add("@uid", SqlDbType.BigInt).Value = uid;
            cmdusers.Parameters.Add("@ProjectID", SqlDbType.Decimal).Value = formData.ProjectID;
            cmdusers.Parameters.Add("@CountryID", SqlDbType.Decimal).Value = formData.CountryID;
            cmdusers.Parameters.Add("@opt", SqlDbType.Int).Value = opt;
            cmdusers.Connection = Connection;
            SqlDataAdapter adap = new SqlDataAdapter(cmdusers);
            DataSet ds = new DataSet();
            adap.Fill(ds);
            return ds;
        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in GetDataMap()");
            return null;
        }
        finally
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
        }
    }
    public DataSet MapQuestions(ClsPreScreening formData)
    {
        try
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdusers = new SqlCommand();
            cmdusers.CommandText = "PreScreeningMapQuestions";
            cmdusers.CommandType = CommandType.StoredProcedure;
            //cmdusers.Parameters.Add("@uid", SqlDbType.BigInt).Value = uid;
            cmdusers.Parameters.Add("@ProjectID", SqlDbType.Decimal).Value = formData.ProjectID;
            cmdusers.Parameters.Add("@QuestionIDS", SqlDbType.NVarChar, 255).Value = formData.QuestionIDS;
            cmdusers.Parameters.Add("@CountryID", SqlDbType.Decimal).Value = formData.CountryID;
            cmdusers.Parameters.Add("@PreviousButton", SqlDbType.Int).Value = formData.PreviousButton;
            cmdusers.Parameters.Add("@QuestionQID", SqlDbType.Int).Value = formData.QuestionQID;
            cmdusers.Parameters.Add("@Logo", SqlDbType.Int).Value = formData.Logo;
            cmdusers.Parameters.Add("@opt", SqlDbType.Int).Value = formData.opt;
            cmdusers.Connection = Connection;
            SqlDataAdapter adap = new SqlDataAdapter(cmdusers);
            DataSet ds = new DataSet();
            adap.Fill(ds);
            return ds;
        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in GetDataMap()");
            return null;
        }
        finally
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
        }
    }

    public DataSet MapOptions(ClsPreScreening formData)
    {
        try
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdusers = new SqlCommand();
            cmdusers.CommandText = "PreScreeningMapQuestions";
            cmdusers.CommandType = CommandType.StoredProcedure;
            //cmdusers.Parameters.Add("@uid", SqlDbType.BigInt).Value = uid;
            cmdusers.Parameters.Add("@ProjectID", SqlDbType.Decimal).Value = formData.ProjectID;
            cmdusers.Parameters.Add("@QuestionIDS", SqlDbType.NVarChar, 255).Value = formData.QuestionIDS;
            cmdusers.Parameters.Add("@CountryID", SqlDbType.Decimal).Value = formData.CountryID;
            //cmdusers.Parameters.Add("@QuestionQID", SqlDbType.Int).Value = formData.QuestionQID;
            cmdusers.Parameters.Add("@opt", SqlDbType.Int).Value = formData.opt;
            cmdusers.Connection = Connection;
            SqlDataAdapter adap = new SqlDataAdapter(cmdusers);
            DataSet ds = new DataSet();
            adap.Fill(ds);
            return ds;
        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in GetDataMap()");
            return null;
        }
        finally
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
        }
    }

    public int MapOptions1(OptionMapping optionMapping)
    {
        try
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdusers = new SqlCommand();
            cmdusers.CommandText = "PreScreeningMapQuestions";
            cmdusers.CommandType = CommandType.StoredProcedure;

            cmdusers.Parameters.Add("@ProjectID", SqlDbType.Decimal).Value = optionMapping.PID;
            cmdusers.Parameters.Add("@CountryID", SqlDbType.Decimal).Value = optionMapping.CID;
            cmdusers.Parameters.Add("@Logic", SqlDbType.Int).Value = optionMapping.Logic;
            cmdusers.Parameters.Add("@Quota", SqlDbType.Int).Value = optionMapping.Quota;
            cmdusers.Parameters.Add("@OptionId", SqlDbType.Int).Value = optionMapping.OptionId;
            cmdusers.Parameters.Add("@opt", SqlDbType.Int).Value = optionMapping.opt;

            cmdusers.Connection = Connection;
            SqlDataAdapter adap = new SqlDataAdapter(cmdusers);
            DataSet ds = new DataSet();
            adap.Fill(ds);
            return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in GetDataMap()");
            return -1;
        }
        finally
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
        }
    }

    public DataSet FetchOptions(QuestionOptions formData)
    {
        try
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdusers = new SqlCommand();
            cmdusers.CommandText = "FetchQuestions";
            cmdusers.CommandType = CommandType.StoredProcedure;
            cmdusers.Parameters.Add("@opt", SqlDbType.Int).Value = formData.opt;
            cmdusers.Parameters.Add("@QuestionsIds", SqlDbType.NVarChar, 255).Value = formData.QuestionsIds;
            cmdusers.Parameters.Add("@ProjectId", SqlDbType.NVarChar, 255).Value = formData.ProjectID;
            cmdusers.Parameters.Add("@CountryId", SqlDbType.NVarChar, 255).Value = formData.CountryID;
            cmdusers.Connection = Connection;
            SqlDataAdapter adap = new SqlDataAdapter(cmdusers);
            DataSet ds = new DataSet();
            adap.Fill(ds);
            return ds;
        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in GetDataMap()");
            return null;
        }
        finally
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////// --------Pre Screening END----------/////////////////////////////////////////////////////////////////////

    #region VendorsHH
    public DataSet GetSuppliers(VendorsHH vendorsHH)
    {
        try
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            SqlCommand cmdusers = new SqlCommand();
            cmdusers.CommandText = "GetSuppliersForHashing";
            cmdusers.CommandType = CommandType.StoredProcedure;
            cmdusers.Parameters.Add("@opt", SqlDbType.Int).Value = vendorsHH.opt;
            cmdusers.Connection = Connection;
            SqlDataAdapter adap = new SqlDataAdapter(cmdusers);
            DataSet ds = new DataSet();
            adap.Fill(ds);
            return ds;
        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in GetDataMap()");
            return null;
        }
        finally
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
        }
    }

    #endregion


}
public class SurvayLinks
{
    public int ID { get; set; }
    public string Country { get; set; }
    public string SupplierName { get; set; }
    public string ClientSurveyLink { get; set; }
    public string SupplierLink { get; set; }
}
public class FractionComplete
{
    public int Complete { get; set; }
    public int Total { get; set; }
    //IR Part
    public string ExpectedIR { get; set; }
    public string ActualIR { get; set; }
    public string Incomplete { get; set; }
    public string IRPercent { get; set; }
    public string ExpectedLOI { get; set; }
    public string ActualLOI { get; set; }
}
public class OverallReportData
{
    public DateTime? sdate { get; set; }
    public DateTime? edate { get; set; }
    public int countryid { get; set; }
    public int clientid { get; set; }
    public int supplierid { get; set; }
}
public class ClsProject
{
    public int ID { get; set; }
    public string PName { get; set; }
    public string ProjectNumber { get; set; }
    public string PID { get; set; }
    public string Descriptions { get; set; }
    public int? ClientID { get; set; }
    public string PManager { get; set; }
    public string LOI { get; set; }
    public string IRate { get; set; }
    public double CPI { get; set; }

    public string SampleSize { get; set; }
    public string Quota { get; set; }
    public string SDate { get; set; }
    public string EDate { get; set; }
    public string Country { get; set; }
    public int LType { get; set; }
    public string Notes { get; set; }
    public string ClientName { get; set; }
    public int Status { get; set; }
    public float Actual { get; set; }
    public float Expected { get; set; }
    public string Sdate { get; set; }
    public string Edate { get; set; }
    public int total { get; set; }
    public int completed { get; set; }
    public int incomplete { get; set; }
    public int overquota { get; set; }
    public int quotafull { get; set; }
    public int terminate { get; set; }
    public int securityterm { get; set; }
    public int frauderror { get; set; }
    public int screened { get; set; }
    public int Active { get; set; }
    public string DeviceBlock { get; set; }
    public string ActIR { get; set; }
    public string ActLOI { get; set; }
    public string Flag { get; set; }
    public int Cancelled { get; set; }
}


public class ClsProjectPageDetails
{
    public int ID { get; set; }
    public string PName { get; set; }
    public string Descriptions { get; set; }
    public int ClientID { get; set; }


}

public class Recontact
{
    public int ID { get; set; }
    public int ProMapID { get; set; }
    public string MURL { get; set; }
    public int RCcnt { get; set; }
    public string RecontactName { get; set; }
    public string RecontactDescription { get; set; }
    public string PID { get; set; }
    public int pidd { get; set; }
    public string PName { get; set; }
    public int CID { get; set; }
    public string Country { get; set; }
    public string SID { get; set; }
    public string Supplier { get; set; }
    public int RCQ { get; set; }
    public float CPI { get; set; }
    public int LOI { get; set; }
    public int IR { get; set; }
    public int Statusint { get; set; }
    public string StatusStr { get; set; }
    public string Notes { get; set; }
    public int Total { get; set; }
    public int Complete { get; set; }
    public int Terminate { get; set; }
    public int Overquota { get; set; }
    public int S_term { get; set; }
    public int F_error { get; set; }
    public int Incomplete { get; set; }
    public int Cancelled { get; set; }
    public string ErrorReturnDBUID { get; set; }
    public string ErrorReturnDBMessage { get; set; }
    public string ErrorReturnDBStatus { get; set; }
    //public int Total { get; set; }
    //public int Total { get; set; }
    //public int Total { get; set; }
    //public int Total { get; set; }
    //public int Total { get; set; }
}
public class ClsRedirects
{
    public int ID { get; set; }
    public string Complete { get; set; }
    public string Terminate { get; set; }
    public string Overquota { get; set; }
    public string S_Term { get; set; }
    public string F_Error { get; set; }
    public string Var1 { get; set; }
    public string Var2 { get; set; }
}
public class ClsClients
{
    public int Id { get; set; }
    public string ClientName { get; set; }
    public string CCPerson { get; set; }
    public int Contactno { get; set; }
    public string Email { get; set; }
    public string complete { get; set; }
    public string percentage { get; set; }

}
public class ClsManager
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string CCPerson { get; set; }
    public int Contactno { get; set; }
    public string Email { get; set; }
    public string complete { get; set; }
    public string Completepercentage { get; set; }
    public string Total { get; set; }
    public string Totalpercentage { get; set; }
    public string Revenue { get; set; }
    public string Revenuepercentage { get; set; }
}
public class ClsSuppliers2
{
    public int Id { get; set; }
    public string SupplierName { get; set; }
    public int Contactno { get; set; }
    public string Email { get; set; }
    public string complete { get; set; }
    public string percentage { get; set; }

}
public class ClsCountry
{
    public int Id { get; set; }
    public string Country { get; set; }
    public string NumericCode { get; set; }
    public string Total { get; set; }
    public string Percentage { get; set; }

}
public class ClsIRate
{
    public int MaxIRate { get; set; }
    public int MinIRate { get; set; }
    public int AvgIRate { get; set; }

}
public class ClsLOI
{
    public int MaxLOI { get; set; }
    public int MinLOI { get; set; }
    public int AvgLOI { get; set; }

}
public class ClsActualLOI
{
    public int MaxLOI { get; set; }
    public int MinLOI { get; set; }
    public int AvgLOI { get; set; }

}
public class CLSGetValue
{
    public int Summ { get; set; }
}
public class CLSGetValueSupp
{
    public string SupplierName { get; set; }
    public float suppcost { get; set; }
    public float SuppPercent { get; set; }
}
public class CLSGetValueClient
{
    public string ClientName { get; set; }
    public float ClientValue { get; set; }
    public float ClientPercent { get; set; }
}
public class CLSGetValueCountry
{
    public string CountryName { get; set; }
    public float ContValue { get; set; }
    public float ContPercent { get; set; }
}
public class ClsCPI
{
    public int MaxCPI { get; set; }
    public int MinCPI { get; set; }
    public int AvgCPI { get; set; }
}
public class CLsRespondants
{
    public int id { get; set; }
    public int Total { get; set; }
    public int Complete { get; set; }
    public int Incomplete { get; set; }
    public int Screened { get; set; }
    public int Quotafull { get; set; }
    public int Terminate { get; set; }
}
public class CLsSuccessRate
{
    public int id { get; set; }
    public int ProjectSuccessRate { get; set; }
    public int IRSuccessRate { get; set; }
}
public class CLSRespondentsCom
{
    public string SupplierName { get; set; }
    public string SID { get; set; }
    public string UID { get; set; }
    public string Country { get; set; }
    public string SupplierId { get; set; }
    public string Status { get; set; }
    public string Sdate { get; set; }
    public string Edate { get; set; }
    public string Duration { get; set; }
    public string ClientBrowser { get; set; }
    public string ClientIP { get; set; }
    public string Device { get; set; }
    public string Error { get; set; }
    public string PName { get; set; }
    public string PID { get; set; }
}
public class CLSPieChart
{
    public int Min { get; set; }
    public int Max { get; set; }
    public int Mean { get; set; }
}
public class CLSBarChart
{
    public string Months { get; set; }
    public int Completes { get; set; }
    //public int Mean { get; set; }
}
public class CLSProjectmapping
{
    //public int ID { get; set; }
    public string PNumber { get; set; }
    public string ProjectName { get; set; }
    public string Country { get; set; }
    public string Supplier { get; set; }
    public int RespondantQuota { get; set; }
    //public int CPI { get; set; }
    //public string Notes { get; set; }
    //public int MyProperty { get; set; }

    //new properties
    public int ID { get; set; }
    public int ProjectID { get; set; }
    public int CountryID { get; set; }
    public int SUpplierID { get; set; }
    public string OLink { get; set; }
    public float CPI { get; set; }
    public string MLink { get; set; }
    public string SID { get; set; }
    public string Code { get; set; }
    public string SCode { get; set; }
    public string NID { get; set; }
    public int IsUsed { get; set; }
    public string Status { get; set; }
    public string ClientIP { get; set; }
    public string ClientBrowser { get; set; }
    public DateTime CreationDate { get; set; }
    public int Respondants { get; set; }
    public int IsSent { get; set; }
    public string Notes { get; set; }
    public int IsChecked { get; set; }
    public string Completes { get; set; }
    public string Terminate { get; set; }
    public string Quotafull { get; set; }
    public string Screened { get; set; }
    public string Overquota { get; set; }
    public string Incomplete { get; set; }
    public string Security { get; set; }
    public string Fraud { get; set; }
    public string SUCCESS { get; set; }
    public string DEFAULT { get; set; }
    public string FAILURE { get; set; }
    public string QUALITY_TERMINATION { get; set; }
    public string OVER_QUOTA { get; set; }
    public int IsActive { get; set; }
    public int Block { get; set; }
    public int TrackingType { get; set; }
    public int Prescreening { get; set; }
    public int opt { get; set; }
    public int RetVal { get; set; }
    public string RetMessage { get; set; }
    //public int Prescreening { get; set; }
    public int AddHashing { get; set; }
    public string ParameterName { get; set; }
    public string HashingType { get; set; }
}
public class Suppliers
{
    public int ID { get; set; }
    public string Name { get; set; }
    public float CPI { get; set; }
    public float IR { get; set; }
    public string Description { get; set; }
    public string Number { get; set; }
    public string Email { get; set; }
    public string PSize { get; set; }
    public string Country { get; set; }
    public string Notes { get; set; }
    public int Total { get; set; }
    public string Completes { get; set; }
    public string Terminate { get; set; }
    public string Quotafull { get; set; }
    public string Screened { get; set; }
    public string Overquota { get; set; }
    public string Incomplete { get; set; }
    public string Security { get; set; }
    public string Fraud { get; set; }
    public DateTime CreationDate { get; set; }
    public int SStatus { get; set; }
    public int Block { get; set; }
    public int TrackingType { get; set; }
    public int Quota { get; set; }
    public string ActLOI { get; set; }
    public int PreScreening { get; set; }
}

public class SurveySpecs
{
    public int id { get; set; }
    public string country { get; set; }
    public int Survey_Quota { get; set; }
    public int loi { get; set; }
    public float cpi { get; set; }
    public int irate { get; set; }
    public int countryid { get; set; }
    public int total { get; set; }
    public int complete { get; set; }
    public float completepercent { get; set; }
    public int Status { get; set; }
    public int IRPercent { get; set; }
    public int ActLOI { get; set; }
    //public int ID { get; set; }
    //public int ID { get; set; }
    //public int ID { get; set; }
    //public int ID { get; set; }
    //public int ID { get; set; }
    //public int ID { get; set; }
    public string Notes { get; set; }
}
public class CLSManager
{
    public int Id { get; set; }
    public string Manager { get; set; }
}
public class IPSecuritymgr
{
    public int Id { get; set; }
    public string Message { get; set; }
}
public class ClsToken
{
    public int ID { get; set; }
    public string Token { get; set; }
    public string Message { get; set; }

}
public class ClsProjectWiseReport
{
    public string PNumber { get; set; }
    public string PStatus { get; set; }
    public string Company { get; set; }
    public string Manager { get; set; }
    public string Country { get; set; }
    public float Complete { get; set; }
    public float CPC { get; set; }
    public float RevenueFromCompletes { get; set; }
    public int CountryID { get; set; }
    public float TotalRevenue { get; set; }
    public int SupplierID { get; set; }
    public string Supplier { get; set; }
    public float Cost { get; set; }
    public float TotalCost { get; set; }
    public float GrossProfit { get; set; }
    public float Margin { get; set; }
}
public class ClsPreScreening
{
    public int ID { get; set; }
    public string QuestionID { get; set; }
    public string QuestionLabel { get; set; }
    public int QuestionType { get; set; }
    public double ProjectID { get; set; }
    public double CountryID { get; set; }
    public int RetVal { get; set; }
    public string RetStat { get; set; }
    public int opt { get; set; }
    public string QuestionIDS { get; set; }
    public int PreviousButton { get; set; }
    public int QuestionQID { get; set; }
    public int Logo { get; set; }

}
public enum Operation
{
    nothing = 0,
    select = 1,
    insert = 2
}
public enum report
{
    client = 1,
    supplier = 2,
    report = 3
}