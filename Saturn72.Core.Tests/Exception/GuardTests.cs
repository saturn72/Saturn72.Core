using System;
using Automation.Extensions;
using Automation.Extensions.UnitTesting;
using Xunit;
using Guard = Automation.Extensions.Guard;

namespace Automation.Core.Tests.Exception
{
    public class GuardTests
    {
        [Fact]
        public void MustFollows_ThrowsCustomException_OnBooleanCondition()
        {
            typeof (Saturn72Exception).ShouldBeThrownBy(
                () => Guard.MustFollow("".HaveValue(), () => { throw new Saturn72Exception(); }));
        }

        [Fact]
        public void MustFollows_ThrowsCustomException_OnDelegate()
        {
            Assert.Throws<Saturn72Exception>(
                () => Guard.MustFollow(() => "".HaveValue(), () => { throw new Saturn72Exception(); }));
        }

        [Fact]
        public void NotEmpty_ThrowsCustomExceptio()
        {
            Assert.Throws<Saturn72Exception>(
                () => Guard.NotEmpty(((string) null), () => { throw new Saturn72Exception(); }));
            Assert.Throws<Saturn72Exception>(
                () => Guard.NotEmpty("", () => { throw new Saturn72Exception(); }));
        }

        [Fact]
        public void NotEmpty_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => Guard.NotEmpty(((string) null)));
            Assert.Throws<InvalidOperationException>(() => Guard.NotEmpty(""));
        }

        [Fact]
        public void NotNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Guard.NotNull(null));
        }

        [Fact]
        public void NotNull_ThrowsArgumentNullExceptionWithMessage()
        {
            typeof(ArgumentNullException).ShouldBeThrownBy(() => Guard.NotNull(null, "message"), "message");
        }

        [Fact]
        public void NotNull_ThrowsCustomException()
        {
            Assert.Throws<Saturn72Exception>(() => Guard.NotNull(null, () => { throw new Saturn72Exception(); }));
        }
    }
}