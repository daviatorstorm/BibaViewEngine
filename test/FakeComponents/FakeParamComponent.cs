using BibaViewEngine;
using BibaViewEngine.Models;

namespace test.FakeComponents
{
    public class FakeParamComponent : Component
    {
        public FakeParamComponent(RouterData data)
        {
            var id = data.GetValue("id");
            if (!id.Equals("10"))
            {
                throw new System.ArgumentException("Id cannot be null or empty", "Id");
            }
        }
    }
}
