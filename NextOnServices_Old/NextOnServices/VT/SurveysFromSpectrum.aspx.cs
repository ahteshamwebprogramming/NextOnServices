using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class VT_SurveysFromSpectrum : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }


    [WebMethod]
    public static string GetSurveys()
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        var client = new HttpClient();
        //client.DefaultRequestHeaders.Add("access-token", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjY1MDMyZTdjZjM5MDcxNDZkMjBjNmIyOCIsInVzcl9pZCI6IjM0MzciLCJpYXQiOjE2OTQ3MDczMjR9.92Ewak2OcZTbuFS5REq_Nwcw5iB6Psv43ZMEEBCGHGo");
        client.DefaultRequestHeaders.Add("access-token", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjY1NmY4YmY4YzIwZjM0NGM3YjYyZGI4ZCIsInVzcl9pZCI6IjE5NzAxIiwiaWF0IjoxNzAxODA5MTQ0fQ.1rzvWDMOvFBqzw8U2k8jFpOFOglgNWIXTrWykub-USc");
        //client.DefaultRequestHeaders.Add("access-token", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJjbXAiOjU2MywiaWF0IjoxNzY4NDk4MDM1fQ.1S71kJc5BRxPcJnacK3Wg2kysswyaRcLdxwHiKTMDBg");
        client.DefaultRequestHeaders.CacheControl = CacheControlHeaderValue.Parse("no-cache");
        var uri = "https://api.spectrumsurveys.com/suppliers/v2/surveys";
        //var uri = "http://staging.spectrumsurveys.com/suppliers/v2/surveys";
        //var response = await client.GetAsync(uri);
        SpectrumAPISurveys spectrumAPISurveys = new SpectrumAPISurveys();
        var response = client.GetAsync(uri).Result;
        if (response.IsSuccessStatusCode)
        {
            string stringData = response.Content.ReadAsStringAsync().Result;
            return stringData;
        }
        else
        {
            return "Error";
        }
    }



    [WebMethod]
    public static string TestAPI()
    {
        //string pi = ProjectId;

        //HttpClient hc = new HttpClient();
        //hc.BaseAddress = new Uri("https://api.sample-cube.com/api/");
        //var consumeAPI = hc.GetAsync("Survey/GetSupplierAllocatedSurveys/1595/084853e8-1b98-4828-9af8-15332e5fe165");
        //consumeAPI.Wait();

        //var readd = consumeAPI.Result;

        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("access-token", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjY1MDMyZTdjZjM5MDcxNDZkMjBjNmIyOCIsInVzcl9pZCI6IjM0MzciLCJpYXQiOjE2OTQ3MDczMjR9.92Ewak2OcZTbuFS5REq_Nwcw5iB6Psv43ZMEEBCGHGo");
        client.DefaultRequestHeaders.CacheControl = CacheControlHeaderValue.Parse("no-cache");
        var uri = "http://staging.spectrumsurveys.com/suppliers/v2/surveys";
        //var response = await client.GetAsync(uri);
        SpectrumAPISurveys spectrumAPISurveys = new SpectrumAPISurveys();
        var response = client.GetAsync(uri).Result;
        if (response.IsSuccessStatusCode)
        {
            string stringData = response.Content.ReadAsStringAsync().Result;
            return stringData;
        }
        else
        {
            return "Error";
        }

        //var res = await response.Content.ReadAsAsync<object>();
        //return spectrumAPISurveys;
        //return product;

        //var dataObjects = response.rea  .ReadAsAsync<IEnumerable<DataObject>>().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll
        //foreach (var d in dataObjects)
        //{
        //    Console.WriteLine("{0}", d.Name);
        //}


        //return response;



    }

}