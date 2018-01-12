using BibaViewEngine.Models;
using System;

namespace BibaViewEngine.Exceptions
{
    public class IndexHtmlNotExistsException : Exception
    {
        private readonly BibaViewEngineProperties _props;

        public IndexHtmlNotExistsException(BibaViewEngineProperties props)
        {
            _props = props;
        }

        public override string Message => $"File {_props.IndexHtml} not exists";
    }
}
