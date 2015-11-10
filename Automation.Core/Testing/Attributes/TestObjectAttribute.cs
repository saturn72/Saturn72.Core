using System;

namespace Automation.Core.Testing.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method)]
    public class TestObjectAttribute : Attribute
    {
        protected TestObject testObject;

        public TestObjectAttribute(string id, TestObjectType testObjectType)
        {
            testObject = new TestObject
            {
                Id = Guid.Parse(id),
                TestObjectType = testObjectType
            };
        }

        public TestObject TestObject { get { return testObject; } }
    }
}