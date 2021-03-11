using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Genetec_Project.Services
{
    class Problem5
    {
        public static async Task<string> MakeRequest(String url) {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "bbfd6981289248588bcf2fd35d998400");
            

            // Request parameters
            queryString["mode"] = "Printed";
            var uri = "https://eastus.api.cognitive.microsoft.com/vision/v2.0/recognizeText?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{\"url\":\""+url+"\"}");

            using (var content = new ByteArrayContent(byteData)) {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
                HttpHeaders headers = response.Headers;
                IEnumerable<string> values;
                if (headers.TryGetValues("Operation-Location", out values)) {
                    string location = values.First();
                    Console.WriteLine("Operation Location : " + location);
                    return location;
                }
                
                return null;
            }

        }


        public static async Task<string> MakeGetRequest(string operationUrl) {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "bbfd6981289248588bcf2fd35d998400");

            var uri = operationUrl + "?" + queryString;

            var response = await client.GetAsync(uri);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
