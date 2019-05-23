namespace System.Reflection.Emit
{
    internal class ProxyFieldMapping
    {
        public ProxyFieldMapping(FieldBuilder fieldBuilder)
        {
            FieldBuilder = fieldBuilder;
        }

        public string Name {
            get => FieldBuilder.Name;
        }

        public FieldBuilder FieldBuilder {
            get;
        }
    }
}
