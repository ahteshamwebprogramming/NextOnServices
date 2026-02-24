using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

/// <summary>
/// Summary description for HashingTest
/// </summary>
public class HashingTest
{
    public HashingTest()
    {
        //
        // TO DO: Add constructor logic here
        //
    }
    public void testHashing(string HashingURL, string Status, string AuthorizationKey, DateTime now)
    {
        try
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("PUT"), HashingURL))
                {
                    request.Headers.TryAddWithoutValidation("Authorization", AuthorizationKey);

                    request.Content = new StringContent("{“status_id”:10}");
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                    var response = httpClient.SendAsync(request);
                    //DateTime now = DateTime.Now;
                    ClsDAL.WriteErrorLog("Hashing URL at time " + now + " is : " + HashingURL + ". || Response Status : " + response.Status + " || ");
                    //ClsDAL.WriteErrorLog("Hashing URL is : "+HashingURL);
                }
            }
        }
        catch (Exception e)
        {
            ClsDAL.WriteErrorLog("Exception in Hashing URL at time " + DateTime.Now + " is : " + HashingURL);
        }
    }

    public void CallSSCB(string HashingURL, string Status, string AuthorizationKey, DateTime now)
    {
        try
        {
            WebRequest request = WebRequest.Create(HashingURL);
            string args = @"{
                  ""status_id"":""10""
                }";
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", AuthorizationKey);
            using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(args);
                streamWriter.Flush();
                streamWriter.Close();
            }
            WebResponse response = request.GetResponse();
            ClsDAL.WriteErrorLog("Hashing URL at time " + now + " is : " + HashingURL + ". || Response Status : " + ((System.Net.HttpWebResponse)response).StatusCode + " || ");
        }
        catch (Exception e)
        {
            ClsDAL.WriteErrorLog("Exception in Hashing URL at time " + DateTime.Now + " is : " + HashingURL);
        }
    }


}