using System.Collections.Generic;

namespace BibaViewEngine.Models
{
    public class RouterData
    {
        private IDictionary<string, object> _data;

        public RouterData()
        {
            _data = new Dictionary<string, object>();
        }

        public string GetValue(string name)
        {
            if (_data.TryGetValue(name, out object value))
            {
                return value.ToString();
            }

            return string.Empty;
        }

        public void SetRouterData(IDictionary<string, object> data)
        {
            _data = data;
        }
    }
}