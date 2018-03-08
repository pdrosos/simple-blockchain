using Microsoft.AspNetCore.Http;

namespace Node.Api.Helpers
{
    public interface IHttpContextHelpers
    {
        string GetApplicationUrl(HttpContext context);
    }
}
