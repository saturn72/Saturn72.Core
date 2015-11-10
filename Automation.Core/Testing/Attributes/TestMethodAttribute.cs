namespace Automation.Core.Testing.Attributes
{
    public class TestMethodAttribute : TestObjectAttribute
    {
        public TestMethodAttribute(string id)
            : base(id, TestObjectType.Method)
        {
        }
    }
}