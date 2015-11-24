using System.Collections.Generic;
using Moq;
using Saturn72.Core.Data;
using Saturn72.Core.Domain.Common;
using Saturn72.Core.Domain.Logging;
using Saturn72.Core.Services.Logging;
using Saturn72.TestSdk.UnitTesting;
using Xunit;

namespace Saturn72.Core.Services.Tests.Logging
{
    public class DatabaseLoggerTests
    {
        [Fact]
        public void DbLogger_WriteToDatabase()
        {
            var logRecordList = new List<LogRecord>();
            var repositoryMock = new Mock<IRepository<LogRecord>>();
            repositoryMock
                .Setup(r => r.Insert(It.IsAny<LogRecord>()))
                .Callback<LogRecord>(lr => logRecordList.Add(lr));

            var fakeCommonSettings = new FakeCommonSettings();

            var dbLogger = new DbLogger(repositoryMock.Object, fakeCommonSettings);
            var expected = new LogRecord
            {
                LogLevel = LogLevel.Debug,
                ShortMessage = "shortMessage",
                FullMessage = "fullMessage"
            };

            dbLogger.InsertLogRecord(expected.LogLevel, expected.ShortMessage, expected.FullMessage);

            Assert.Equal(1, logRecordList.Count);

            var actual = logRecordList[0];
            expected.FullMessage.ShouldEqual(actual.FullMessage);
            expected.ShortMessage.ShouldEqual(actual.ShortMessage);
            expected.LogLevel.ShouldEqual(actual.LogLevel);

        }
    }

    public class FakeCommonSettings:CommonSettings
    {
        public FakeCommonSettings():base()
        {
            IgnoreLogWordlist = new List<string>();
        }
        public new List<string> IgnoreLogWordlist { get; set; }


    }
}
