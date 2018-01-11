using System;

namespace BibaViewEngine.Exceptions
{
    public class EvaluatorPropertyNotExistsException : Exception
    {
        string _propName;

        public EvaluatorPropertyNotExistsException(string propertyName, object context)
        {
            _propName = propertyName;
            Data.Add("Context", context);
            Data.Add("PropertyName", propertyName);
        }

        public override string Message => $"Property \"{_propName}\" does not exists in current context";
    }
}
