using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Node.Api.Extensions;

namespace Node.Api.Helpers
{
    public class HttpHelpers : IHttpHelpers
    {
        public async Task<HttpResponseMessage> DoApiPost<TRequest>(string domainUrl, string path, TRequest requestObject)
        {
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Accept.Clear();

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string requestUrl = $"{domainUrl}/{path}";

            HttpResponseMessage response = await httpClient.PostAsync(requestUrl, new JsonContent(requestObject));

            return response;
        }
    }
}
