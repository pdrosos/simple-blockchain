using System.Threading.Tasks;

namespace Node.Api.Helpers
{
    public interface IHttpHelpers
    {
        Task DoApiPost<TRequest>(string url, string path, TRequest requestObject);
    }
}
