namespace PICS
{
    public class PropertyValuePair
    {
        public string Property { get; set; }
        public string Value { get; set; }

        public PropertyValuePair() : this("N/A", "N/A") { }

        public PropertyValuePair(string property, string value)
        {
            Property = property;
            Value = value;
        }
    }
}
