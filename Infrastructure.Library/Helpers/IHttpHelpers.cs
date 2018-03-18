using System.Net.Http;
using System.Threading.Tasks;

namespace Infrastructure.Library.Helpers
{
    public interface IHttpHelpers
    {
        Task<HttpResponseMessage> DoApiPost<TRequest>(string url, string path, TRequest requestObject);

        Task<Response<T>> DoApiGet<T>(string url, string path, params Parameter[] parameters);
    }
}
