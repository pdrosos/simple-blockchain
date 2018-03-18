using System.Net.Http;
using System.Text;

using Newtonsoft.Json;

namespace Infrastructure.Library.Extensions
{
    public class JsonContent : StringContent
    {
        public JsonContent(object obj) : base(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
        {
        }
    }
}
