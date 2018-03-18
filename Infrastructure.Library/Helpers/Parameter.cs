namespace Infrastructure.Library.Helpers
{
    public class Parameter
    {
        private string name;

        private string value;

        private ParameterType type;

        public Parameter(string name, string value, ParameterType type)
        {
            this.name = name;
            this.value = value;
            this.type = type;
        }

        public Parameter()
        {
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public ParameterType Type
        {
            get { return this.type; }
            set { this.type = value; }
        }
    }
}
