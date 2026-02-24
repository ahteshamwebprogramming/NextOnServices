using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

public partial class VT_HashingTest : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        TestNewHashing2();
    }
    public async void testHashing1()
    {
        try
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("PUT"), "https://callback.samplicio.us/callback/v1/status/5e9f3949-03e4-042d-9329-f614373ec35d"))
                {

                    request.Headers.TryAddWithoutValidation("Authorization", "7EF141BB-C7DB-45E6-9EB5-4A21FAF91758");
                    //request.Headers.TryAddWithoutValidation("Authorization", "7EF141BB-C7DB-45E6-9EB5-4A21FAF91758");

                    request.Content = new StringContent("{“status_id”:10}");
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");


                    //var gizmo = new Product() { Name = "Gizmo", Price = 100, Category = "Widget" };
                    //response = await client.PostAsJsonAsync("api/products", gizmo);
                    //if (response.IsSuccessStatusCode)
                    //{
                    //    Uri gizmoUrl = response.Headers.Location;

                    //    // HTTP PUT
                    //    gizmo.Price = 80;   // Update price
                    //    response = await client.PutAsJsonAsync(gizmoUrl, gizmo);

                    //    // HTTP DELETE
                    //    response = await client.DeleteAsync(gizmoUrl);
                    //}


                    var response = httpClient.SendAsync(request);
                    //DateTime now = DateTime.Now;
                    // ClsDAL.WriteErrorLog("Hashing URL at time " + now + " is : " + HashingURL + ". || Response Status : " + response.Status + " || ");
                    //ClsDAL.WriteErrorLog("Hashing URL is : "+HashingURL);
                }
            }
        }
        catch (Exception e)
        {
            ClsDAL.WriteErrorLog("Exception in Hashing URL at time " + DateTime.Now + " is : ");
        }


    }
    public void TestNewHashing2()
    {
        WebRequest request = WebRequest.Create("https://callback.samplicio.us/callback/v1/status/5e9f3949-03e4-042d-9329-f614373ec35d");
        string args = @"{
                  ""status_id"":""10""
                }";
        request.Method = "PUT";
        request.ContentType = "application/json";
        request.Headers.Add("Authorization", "7EF141BB-C7DB-45E6-9EB5-4A21FAF91758");
        using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
        {
            streamWriter.Write(args);
            streamWriter.Flush();
            streamWriter.Close();
        }
        WebResponse response = request.GetResponse();

        
    }

}
public static class HashingTest1
{
    public static async void testHashing()
    {
        using (var httpClient = new HttpClient())
        {
            using (var request = new HttpRequestMessage(new HttpMethod("PUT"), "https://callback.samplicio.us/callback/v1/status/test_99014956"))
            {
                request.Headers.TryAddWithoutValidation("Authorization", "7EF141BB-C7DB-45E6-9EB5-4A21FAF91758");

                request.Content = new StringContent("{\"status_id\":10}");
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                var response = await httpClient.SendAsync(request);
            }
        }

    }
}