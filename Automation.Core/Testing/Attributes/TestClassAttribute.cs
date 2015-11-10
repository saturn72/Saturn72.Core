namespace Automation.Core.Testing.Attributes
{
    public class TestClassAttribute : TestObjectAttribute
    {
        public TestClassAttribute(string id)
            : base(id, TestObjectType.Class)
        {
        }
    }
}