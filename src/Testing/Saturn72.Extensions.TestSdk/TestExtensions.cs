using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace Saturn72.Extensions.TestSdk
{
    public static class TestExtensions
    {
        public static void ShouldNotNull<T>(this T obj)
        {
            Assert.Null(obj);
        }

        public static void ShouldNotNull<T>(this T obj, string message)
        {
            var condition = obj == null;
            Assert.True(condition, message);
        }

        public static void ShouldNotBeNull<T>(this T obj)
        {
            Assert.NotNull(obj);
        }

        public static void ShouldNotBeNull<T>(this T obj, string message)
        {
            var condition = obj != null;
            Assert.True(condition, message);
        }

        public static void ShouldNotEqual<T>(this T actual, object expected)
        {
            Assert.NotEqual(expected, actual);
        }

        public static void ShouldNotEqual<T>(this T actual, object expected, string message)
        {
            var result = expected.Equals(actual);
            Assert.True(result, message);
        }

        public static void ShouldEqual<T>(this T actual, object expected)
        {
            Assert.Equal(expected, actual);
        }

        /// <summary>
        ///     Asserts that two objects are equal.
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <param name="message"></param>
        public static void ShouldEqual(this object actual, object expected, string message)
        {
            Assert.Equal(expected, actual);
        }

        public static Exception ShouldBeThrownBy(this Type exceptionType, Action testCode)
        {
            return Assert.Throws(exceptionType, testCode);
        }

        public static Exception ShouldBeThrownBy(this Type exceptionType, Action testCode, string message)
        {
            var t = new Task(() => { });
            var exception = Assert.Throws(exceptionType, testCode);
            Assert.Equal(message, exception.Message);

            return exception;
        }

        public static void ShouldBe<T>(this object actual)
        {
            Assert.Equal(typeof (T), actual.GetType());
        }

        public static void ShouldBeNull(this object actual)
        {
            Assert.Null(actual);
        }

        public static void ShouldBeTheSameAs(this object actual, object expected)
        {
            Assert.Same(expected, actual);
        }

        public static void ShouldBeNotBeTheSameAs(this object actual, object expected)
        {
            Assert.NotSame(expected, actual);
        }

        public static T CastTo<T>(this object source)
        {
            return (T) source;
        }

        public static void ShouldBeTrue(this bool source)
        {
            Assert.True(source);
        }

        public static void ShouldBeTrue(this bool source, string message)
        {
            Assert.True(source, message);
        }

        public static void ShouldBeFalse(this bool source)
        {
            Assert.False(source);
        }

        /// <summary>
        ///     Compares the two strings (case-insensitive).
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        public static void AssertSameStringAs(this string actual, string expected)
        {
            if (!string.Equals(actual, expected, StringComparison.InvariantCultureIgnoreCase))
            {
                var message = string.Format("Expected {0} but was {1}", expected, actual);
                throw new XunitException(message);
            }
        }
    }
}