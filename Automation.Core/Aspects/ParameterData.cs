namespace Automation.Core.Aspects
{
    public class ParameterData
    {
        private readonly string _name;
        private readonly object _value;

        public ParameterData(string name, object value)
        {
            _name = name;
            _value = value;
        }

        public string Name
        {
            get { return _name; }
        }

        public object Value
        {
            get { return _value; }
        }

        public override string ToString()
        {
            return string.Format("Name: {0} <==> Value: {1}", Name, Value);
        }
    }
}