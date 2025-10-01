using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.Helper
{
    public class HashingTest
    {
        
        public async Task TestHashingAsync(string hashingUrl, string status, string authorizationKey, DateTime now)
        {
            try
            {
                using (var httpClient = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Put, hashingUrl))
                {
                    request.Headers.TryAddWithoutValidation("Authorization", authorizationKey);

                    var json = "{\"status_id\":10}";
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await httpClient.SendAsync(request);
                    var statusCode = (int)response.StatusCode;

                    //_logger.LogInformation("Hashing URL at time {Time} is: {URL} || Response Status: {Status}",
                    //    now, hashingUrl, statusCode);
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Exception in Hashing URL at time {Time} is: {URL}", DateTime.Now, hashingUrl);
            }
        }

        public void CallSSCB(string hashingUrl, string status, string authorizationKey, DateTime now)
        {
            try
            {
                var request = WebRequest.Create(hashingUrl);
                string args = "{\"status_id\":\"10\"}";

                request.Method = "PUT";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", authorizationKey);

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(args);
                    streamWriter.Flush();
                }

                using (var response = request.GetResponse())
                {
                    var statusCode = ((HttpWebResponse)response).StatusCode;
                    //_logger.LogInformation("Hashing URL at time {Time} is: {URL} || Response Status: {Status}",
                    //    now, hashingUrl, statusCode);
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Exception in Hashing URL at time {Time} is: {URL}", DateTime.Now, hashingUrl);
            }
        }
    }

}
