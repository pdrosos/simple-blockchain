using System.Collections.Generic;

namespace Node.Api.Configuration
{
    public class ApplicationSettings
    {
        public string About { get; set; }

        public int Difficulty { get; set; }

        public List<string> AllNodes { get; set; }
    }
}
