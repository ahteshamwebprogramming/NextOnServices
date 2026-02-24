using System;
using System.Data;
using System.Linq;
using System.Web.UI.HtmlControls;

public partial class ProjectStatus : System.Web.UI.Page
{
    NextOnServicesEntities3 db = new NextOnServicesEntities3();
    ClsDAL objDAL = new ClsDAL();
    string SID = "", Status = "", Resp = "", ReqUrl = "", ClientID = ""; int RC = 0;
    int cnt = 0, PID = 0, PX = 0;
    protected void Page_Load(object sender, EventArgs e)
    {



        //CheckComplete("COMPLETE");
        //return;
        //while (5 >cnt)
        //{
        //    RequestUrl();
        //    cnt++;
        //}
        //char Seperator = '?';
        //string EncryptedCompleteURL = HttpContext.Current.Request.Url.AbsoluteUri;
        //string URLFORDECRYPTION = EncryptedCompleteURL.Split(Seperator)[1];
        //string DecryptedURL = Encryption.Decrypt(URLFORDECRYPTION, "sblw-3hn8-sqoy25");
        //string DecryptedCompletedURL = EncryptedCompleteURL.Split(Seperator)[0] + "?" + DecryptedURL;
        //Uri myUri = new Uri(DecryptedCompletedURL);
        //if (HttpUtility.ParseQueryString(myUri.Query).Count > 1)
        //{
        //    SID = HttpUtility.ParseQueryString(myUri.Query).Get("ID");
        //    Status = HttpUtility.ParseQueryString(myUri.Query).Get("Status");
        //    RC = Convert.ToInt32(HttpUtility.ParseQueryString(myUri.Query).Get("RC"));
        //    SID = SID.Replace("[", "").Replace("]", "");
        //    if (RC == 0)
        //    {
        //        Resp = objDAL.UpdateProjectDetails("", "", Status, "", SID.Trim(), 1, 2);
        //    }
        //    else if (RC == 1)
        //    {
        //        PID = Convert.ToInt32(Request.QueryString["PID"]);
        //        Resp = objDAL.UpdateProjectDetails("", "", Status, PID.ToString(), SID.Trim(), 1, 3);
        //    }
        //}


        if (Request.QueryString.Count > 1)
        {
            SID = Convert.ToString(Request.QueryString["ID"]);
            string TempStat = Convert.ToString(Request.QueryString["Status"]);
            Status = ReformStatus(Convert.ToString(Request.QueryString["Status"]));
            RC = Convert.ToInt32(Request.QueryString["RC"]);
            SID = SID.Replace("[", "").Replace("]", "");
            if (RC == 0)
            {
                Resp = objDAL.UpdateProjectDetails("", TempStat, Status, "", SID.Trim(), 1, 2);
            }
            else if (RC == 1)
            {
                PID = Convert.ToInt32(Request.QueryString["PID"]);
                Resp = objDAL.UpdateProjectDetails("", TempStat, Status, PID.ToString(), SID.Trim(), 1, 3);
            }
            //if (Request.QueryString.Count == 3)
            //{
            //    RC = Convert.ToInt32(Request.QueryString["RC"]);
            //    Resp = objDAL.UpdateProjectDetails("", "", Status, "", SID.Trim(), 1, 3);
            //}
            //else
            //{
            //    Resp = objDAL.UpdateProjectDetails("", "", Status, "", SID.Trim(), 1, 2);
            //}
            if (Request.QueryString.AllKeys.Contains("PX"))
            {
                PX = 1;
            }
            if (Resp == "1")
            {
                if (Status.ToUpper() == "COMPLETE")
                {
                    lblmsg.Text = "Congratulations! You have successfully completed the survey.";
                }
                else if (Status.ToUpper() == "TERMINATE")
                {
                    lblmsg.Text = "Thank you very much for your participation. <br> Unfortunately, at the moment we are looking for a different profile to match survey's conditions.";
                    // Response.Write("Thank you for your interest in this survey. <br> Unfortunately, you have not completed the survey.");
                }
                else if (Status.ToUpper() == "QUOTAFULL")
                {
                    lblmsg.Text = "Thank you very much for your participation, but at this time we have received specific numbers of completes.";
                    // Response.Write("Thank you for your interest in this survey. <br> Unfortunately, you did not match the criteria required for participation.");
                }
                else if (Status.ToUpper() == "SCREENED")
                {
                    lblmsg.Text = "Thank you for your interest in this survey. <br> Unfortunately, you did not match the criteria required for participation.";
                    // Response.Write("Thank you for your interest in this survey. <br> Unfortunately, you did not match the criteria required for participation.");
                }
                else if (Status.ToUpper() == "OVERQUOTA")
                {
                    lblmsg.Text = "Thank you for your interest in this survey. <br> Unfortunately, the quota was reached for your demographic group.";
                    // Response.Write("Thank you for your interest in this survey. <br> Unfortunately, the quota was reached for your demographic group.");
                }
                else if (Status.ToUpper() == "SEC_TERM")
                {
                    lblmsg.Text = "Thank you very much for your participation. <br> Unfortunately, at the moment we are lookng for a different profile to match survey's conditions.";
                    // Response.Write("Thank you for your interest in this survey. <br> Unfortunately, the quota was reached for your demographic group.");
                }
                else if (Status.ToUpper() == "F_ERROR")
                {
                    lblmsg.Text = "Thank you very much for your participation. <br> Unfortunately, at the moment we are lookng for a different profile to match survey's conditions.";
                    // Response.Write("Thank you for your interest in this survey. <br> Unfortunately, the quota was reached for your demographic group.");
                }
                else
                {
                    // Response.Write("Thank you for your interest in this survey. <br> Unfortunately, you have not completed the survey.");
                }
                // Response.Write("Your status has been punched as " + Status);
                //  System.Threading.Thread.Sleep(20*1000);
                // Label1.Text = "You will now be redirected in 5 seconds";
                int St = RequestUrl(SID.Trim(), RC, PID, PX);
                //if (St > 0)
                //{
                //    if (Status.ToUpper() == "COMPLETE")
                //    {
                //        Response.Write("Congratulations, you have completed the survey.");
                //    }
                //    else if (Status.ToUpper() == "TERMINATE")
                //    {
                //        Response.Write("Thank you for your interest in this survey. <br> Unfortunately, you have not completed the survey.");
                //    }
                //    else if (Status.ToUpper() == "QUOTA FULL")
                //    {
                //        Response.Write("Thank you for your interest in this survey. <br> Unfortunately, you did not match the criteria required for participation.");
                //    }
                //    else if (Status.ToUpper() == "SCREENED")
                //    {
                //        Response.Write("Thank you for your interest in this survey. <br> Unfortunately, you did not match the criteria required for participation.");
                //    }
                //    else if (Status.ToUpper() == "OVERQUOTA")
                //    {
                //        Response.Write("Thank you for your interest in this survey. <br> Unfortunately, the quota was reached for your demographic group.");
                //    }
                //    else
                //    {
                //        Response.Write("Thank you for your interest in this survey. <br> Unfortunately, you have not completed the survey.");
                //    }
                //   // Response.Write("Your status has been punched as " + Status);
                //}
                //else
                //{
                //    Response.Write("Thank you for your interest in this survey. <br> Unfortunately, a survey related to this has already been attempted by you.");
                //}
            }
            else
            {
                lblmsg.Text = "Thank you for your interest in this survey.";
                // Response.Write("Thank you for your interest in this survey.");
            }

        }
        else
        {
            lblmsg.Text = "Required Parameters";
        }
    }

    int RequestUrl(string ID, int RC, int PID, int PX)
    {
        SID = ""; Status = ""; ClientID = ""; int opt;
        // string ReqUrl = "http://localhost:2359/RequestUrl.aspx?survey_id=12999&SID=XXXXXXX&redirected_from=survey&status=Screened";
        // string createdURL = ReqUrl +
        // "&identifier=344555" +
        // "&Status=InComplete" ;
        try
        {
            if (RC == 0)
            {
                opt = 0;
            }
            else
            {
                opt = 1;
            }

            ClsDAL.WriteErrorLog("Reached");
            int trackingtype = objDAL.CheckForPixelTracking(ID, opt, PID);
            DataSet DS = objDAL.GetRequestUrl(ID, opt, PID);
            if (DS.Tables[0].Rows.Count > 0)
            {
                SID = Convert.ToString(DS.Tables[0].Rows[0]["SuplierID"]);
                Status = Convert.ToString(DS.Tables[0].Rows[0]["PStatus"]);
                ReqUrl = Convert.ToString(DS.Tables[0].Rows[0]["RequestUrl"]);
                ClientID = Convert.ToString(DS.Tables[0].Rows[0]["ClientID"]);
                //int applyToSupplier = Convert.ToInt32(DS.Tables[0].Rows[0]["ApplyToSupplier"]);
                //int applyToSupplier1 = Convert.ToInt32(DS.Tables[0].Rows[0]["ApplyToSupplier1"]);
                //string parameterName = Convert.ToString(DS.Tables[0].Rows[0]["ParameterName"]);

                var projectMapping = db.ProjectMappinggs.Where(x => x.SID == SID).FirstOrDefault();
                int addHashing = projectMapping.AddHashing ?? default(int);
                string parameterName = projectMapping.ParameterName;
                string hashingType = projectMapping.HashingType;

                string hashcode = "";

                //ReqUrl = ReqUrl + ((applyToSupplier == 1 && applyToSupplier1 == 1) ? "&" + parameterName + "=" + parameterValue : "");


                if (ReqUrl.IndexOf("[respondentID]") > 0 || ReqUrl.IndexOf("[RespondentID]") > 0 || ReqUrl.IndexOf("[RESPONDENTID]") > 0)
                {
                    ReqUrl = ReqUrl.Replace("[respondentID]", ClientID);
                    ReqUrl = ReqUrl.Replace("[RespondentID]", ClientID);
                    ReqUrl = ReqUrl.Replace("[RESPONDENTID]", ClientID);

                    if (addHashing == 1 && hashingType.ToUpper().Trim() == "SHA3")
                    {
                        hashcode = Encryption.HashSHA3(ReqUrl, "9c1ea7525fdb73c4498bbc55b961b381c1054d6b");
                        ReqUrl = ReqUrl + "&" + parameterName + "=" + hashcode;
                        ClsDAL.WriteErrorLog("Hashcode : " + hashcode + "///// ReqURL : " + ReqUrl);
                    }
                    else if (addHashing == 1 && hashingType.ToUpper().Trim() == "SHA1")
                    {
                        hashcode = Encryption.HashSHA1_C(ReqUrl, "50e8a826d47d488cbf7a036b38fab5");
                        ReqUrl = ReqUrl + "&" + parameterName + "=" + hashcode;
                        ClsDAL.WriteErrorLog("Hashcode : " + hashcode + "///// ReqURL : " + ReqUrl);
                    }

                    if (trackingtype == 1)
                    {
                        Response.Write("<iframe src=\"" + ReqUrl + "\"\" scrolling = \"no\" frameborder = \"0\" width = \"1\" height = \"1\" ></ iframe > ");
                    }
                    else if (PX == 1)
                    {
                        Response.Write("<script>top.window.location.href = '" + ReqUrl + "';</script>");
                    }
                    else
                    {
                        DateTime now = DateTime.Now;
                        DataSet ds = objDAL.GetRequestUrl(ID, 2, 0);
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                if (Convert.ToInt32(ds.Tables[0].Rows[0]["AllowHashing"]) == 1)
                                {
                                    //////////////////////////New Code Start///////////////////////////////////
                                    string HashingURL = Convert.ToString(ds.Tables[0].Rows[0]["HashingURL"]);
                                    string CompleteStatus = "10";//Convert.ToString(ds.Tables[0].Rows[0]["CompleteStatus"]);
                                    string AuthorizationKey = Convert.ToString(ds.Tables[0].Rows[0]["AuthorizationKey"]);
                                    if (Status.ToUpper() == "COMPLETE")
                                    {
                                        if (HashingURL.IndexOf("[respondentID]") > 0 || HashingURL.IndexOf("[RespondentID]") > 0 || HashingURL.IndexOf("[RESPONDENTID]") > 0)
                                        {
                                            HashingURL = HashingURL.Replace("[respondentID]", ClientID);
                                            HashingURL = HashingURL.Replace("[RespondentID]", ClientID);
                                            HashingURL = HashingURL.Replace("[RESPONDENTID]", ClientID);
                                        }
                                        if (HashingURL.IndexOf("[StatusCode]") > 0 || HashingURL.IndexOf("[StatusCODE]") > 0 || HashingURL.IndexOf("[STATUSCODE]") > 0)
                                        {
                                            HashingURL = HashingURL.Replace("[StatusCode]", CompleteStatus);
                                            HashingURL = HashingURL.Replace("[StatusCODE]", CompleteStatus);
                                            HashingURL = HashingURL.Replace("[STATUSCODE]", CompleteStatus);
                                        }
                                        HashingTest hashingTest = new HashingTest();
                                        //string stt = Status.ToUpper() == "Complete" ? "10" : "70";
                                        hashingTest.CallSSCB(HashingURL, CompleteStatus, AuthorizationKey, now);
                                    }


                                    //////////////////////////New Code End///////////////////////////////////
                                }
                            }
                        }

                        ClsDAL.WriteErrorLog("ReqURL URL at time " + now + " is : " + ReqUrl);
                        HtmlMeta meta = new HtmlMeta();
                        meta.HttpEquiv = "Refresh";
                        meta.Content = "2;url=" + ReqUrl;
                        this.Page.Controls.Add(meta);

                        // Response.Redirect(ReqUrl, false);
                        //////////////////////////Old Code Start///////////////////////////////////
                        //HtmlMeta meta = new HtmlMeta();
                        //meta.HttpEquiv = "Refresh";
                        //meta.Content = "2;url=" + ReqUrl;
                        //this.Page.Controls.Add(meta);
                        //////////////////////////Old Code End///////////////////////////////////
                        //HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(ReqUrl);
                        ////Get response from Ozeki NG SMS Gateway Server and read the answer
                        //HttpWebResponse myResp = (HttpWebResponse)myReq.GetResponse();
                        //System.IO.StreamReader respStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
                        //string responseString = respStreamReader.ReadToEnd();
                        //respStreamReader.Close();
                        //myResp.Close();

                    }
                    objDAL.UpdateRequestStatus(ID);

                    var sp = db.SupplierProjects.Where(x => x.UID == ID).FirstOrDefault();
                    sp.ENC = hashcode;
                    db.SaveChanges();
                    return 1;
                }
                else
                {
                    // Response.Write("[respondentID] identifier is not found");
                    return 1;
                }
            }
            else
            {
                return 0;
            }
        }
        catch (SystemException ex)
        {
            ClsDAL.WriteErrorLog("Exception in ProjectStatus at " + DateTime.Now + " : " + ex.Message);
            return 0;

        }
    }

    public string Encode(string text)
    {
        byte[] mybyte = System.Text.Encoding.UTF8.GetBytes(text);
        string returntext = System.Convert.ToBase64String(mybyte);

        return returntext;

    }

    public string Decode(string text)
    {
        byte[] mybyte = System.Convert.FromBase64String(text);
        string returntext = System.Text.Encoding.UTF8.GetString(mybyte);
        return returntext;
    }

    public bool CheckComplete(string Status)
    {
        ClsDAL clsDAL = new ClsDAL();
        DataSet ds = clsDAL.ManageCompleteRedirects("", 2, 0);
        string[] Codes = new string[ds.Tables[0].Rows.Count];
        if (ds.Tables.Count > 0)
        {
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Codes[i] = ds.Tables[0].Rows[i]["Code"].ToString();
                }
            }
        }

        if (Array.IndexOf(Codes, Status) >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public string ReformStatus(string Status)
    {
        ClsDAL clsDAL = new ClsDAL();
        DataSet ds = clsDAL.ManageCompleteRedirects("", 2, 0);
        string[] Codes = new string[ds.Tables[0].Rows.Count];
        if (ds.Tables.Count > 0)
        {
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Codes[i] = ds.Tables[0].Rows[i]["Code"].ToString();
                }
            }
        }

        if (Array.IndexOf(Codes, Status) >= 0)
        {
            return "COMPLETE";
        }
        else if (Status.ToUpper() == "TERMINATE")
        {
            return "TERMINATE";
        }
        else if (Status.ToUpper() == "QUOTAFULL")
        {
            return "QUOTAFULL";
        }
        else if (Status.ToUpper() == "SCREENED")
        {
            return "SCREENED";
        }
        else if (Status.ToUpper() == "OVERQUOTA")
        {
            return "OVERQUOTA";
        }
        else if (Status.ToUpper() == "SEC_TERM")
        {
            return "SEC_TERM";
        }
        else if (Status.ToUpper() == "F_ERROR")
        {
            return "F_ERROR";
        }
        else if (Status.ToUpper() == "INCOMPLETE")
        {
            return "INCOMPLETE";
        }
        else
        {
            return "SEC_TERM";
        }
    }

    //protected void btnCode_Click(object sender, EventArgs e)
    //{
    //    // Response.Write(Encode(txtCode.Text));
    //    lblResult.Text = Encode(txtCode.Text);
    //}

    //protected void btnDecode_Click(object sender, EventArgs e)
    //{
    //    lblResult.Text = Decode(txtCode.Text);
    //}
}