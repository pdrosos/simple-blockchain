using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Infrastructure.Library.Extensions;

namespace Infrastructure.Library.Helpers
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

        public async Task<Response<T>> DoApiGet<T>(string url, string path, params Parameter[] parameters)
        {
            var httpClient = new HttpClient();

            string fullUrl = $"{url}/{path}".ToLower();

            foreach (var parameter in parameters)
            {
                if (parameter.Type == ParameterType.UrlSegment)
                {
                    string parameterPlaceholder = "{" + parameter.Name.ToLower() + "}";

                    fullUrl = fullUrl.Replace(parameterPlaceholder, parameter.Value.ToLower());
                }
            }

            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(fullUrl);

            var response = new Response<T>()
            {
                StatusCode = httpResponseMessage.StatusCode,
                StatusMessage = httpResponseMessage.ReasonPhrase
            };

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string responseDataStr = await httpResponseMessage.Content.ReadAsStringAsync();

                response.Data = JsonConvert.DeserializeObject<T>(responseDataStr);
            }

            return response;
        }
    }
}
