using System.Net.Http;
using System.Threading.Tasks;

namespace Node.Api.Helpers
{
    public interface IHttpHelpers
    {
        Task<HttpResponseMessage> DoApiPost<TRequest>(string url, string path, TRequest requestObject);
    }
}
