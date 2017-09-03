using System;
using System.Linq;
using System.Collections;

namespace BibaViewEngine.Compiler
{
    public class Evaluator : IDisposable
    {
        private bool disposed = false;
        private Queue queue;

        private Evaluator()
        {
            queue = new Queue();
        }

        public static Evaluator Create()
        {
            return new Evaluator();
        }

        public string Evaluate(string expression, object context)
        {
            var index = expression.IndexOf(".");
            var propName = string.Empty;

            if (index < 1)
            {
                propName = expression;
            }
            else
            {
                propName = expression.Substring(0, index);
            }

            var prop = context.GetType().GetProperties().FirstOrDefault(x => x.Name == propName);

            if (prop == null)
            {
                throw new Exception($"Property {propName} in object {context.GetType().Name} does not exists");
            }

            var value = prop.GetValue(context);

            if (expression.Contains(".") && value is object)
            {
                return Evaluate(expression.Substring(index + 1, expression.Length - ++index), value);
            }

            return value?.ToString() ?? string.Empty;
        }

        public void Dispose()
        {
            if (!disposed)
            {
                GC.SuppressFinalize(this);

                disposed = true;
            }
        }
    }
}