using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace ChatWebClient.Helpers
{
    public static class HttpHelper
    {
        public static async Task<HttpResponseMessage> HttpPost(String url, Object httpBody, String jwtBearer = null)
        {
            using var httpClient = new HttpClient();

            if(jwtBearer != null)
            {
                var contentType = new MediaTypeWithQualityHeaderValue("application/json");
                httpClient.DefaultRequestHeaders.Accept.Add(contentType);
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwtBearer);
            }
            
            var jsonBody = JsonConvert.SerializeObject(httpBody);
            StringContent data = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(url, data);

            return response;
        }

        public static async Task<HttpResponseMessage> HttpGet(String url, String jwtBearer = null)
        {
            using var httpClient = new HttpClient();

            if (jwtBearer != null)
            {
                var contentType = new MediaTypeWithQualityHeaderValue("application/json");
                httpClient.DefaultRequestHeaders.Accept.Add(contentType);
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwtBearer);
            }

            var response = await httpClient.GetAsync(url);

            return response;
        }

    }
}
