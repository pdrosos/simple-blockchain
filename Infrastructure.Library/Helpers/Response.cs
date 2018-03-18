using System.Net;

namespace Infrastructure.Library.Helpers
{
    public class Response<T>
    {
        public HttpStatusCode StatusCode { get; set; }

        public string StatusMessage { get; set; }

        public T Data { get; set; }
    }
}
