namespace EtAlii.UniCon.Editor
{
    public class FilterRule
    {
        public FilterType FilterType { get; set; }
        public string Property { get; }
        public object Value { get; }

        public FilterRule(string property, object value, FilterType filterType)
        {
            FilterType = filterType;
            Property = property;
            Value = value;
        }
    }
}
