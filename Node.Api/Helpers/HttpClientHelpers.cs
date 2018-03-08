using Microsoft.AspNetCore.Http;

namespace Node.Api.Helpers
{
    public class HttpContextHelpers : IHttpContextHelpers
    {
        public string GetApplicationUrl(HttpContext context)
        {
            string hostValue = context.Request.Host.Value.ToString();

            string requestScheme = context.Request.Scheme;

            string applicationUrl = string.Format("{0}://{1}", requestScheme, hostValue);

            return applicationUrl;
        }
    }
}
