using System;
using System.Collections.Generic;
using System.IO;
using Saturn72.Core.Plugins;
using Saturn72.TestSdk.UnitTesting;
using Xunit;

namespace Saturn72.Core.Tests.Plugins
{
    public class PluginFileParserTests
    {
        [Fact]
        public void ParseInstalledPluginsFile_ReturnsCorrectList()
        {
            var saveToPath = Path.GetTempFileName();
            var lines = new List<string> {"line1", "line2", "line3", "line4"};
            File.WriteAllLines(saveToPath, lines);

            var actual = PluginFileParser.ParseInstalledPluginsFile(saveToPath);
            lines.ShouldEqual(actual);
            File.Delete(saveToPath);
        }

        [Fact]
        public void ParseInstalledPluginsFile_ReturnsCorrectList_EmptyLines()
        {
            var saveToPath = Path.GetTempFileName();
            var lines = new List<string> {"line1", "    ", null, "", "line4"};
            var afterProcess = new List<string> {"line1", "line4"};
            File.WriteAllLines(saveToPath, lines);

            var actual = PluginFileParser.ParseInstalledPluginsFile(saveToPath);
            afterProcess.ShouldEqual(actual);
            File.Delete(saveToPath);
        }

        [Fact]
        public void ParseInstalledPluginsFile_ReturnsEmptyList_OnEmptyFileName()
        {
            0.ShouldEqual(
                PluginFileParser.ParseInstalledPluginsFile(string.Empty).Count);
        }

        [Fact]
        public void ParseInstalledPluginsFile_ReturnsEmptyList_OnEmptyTextFile()
        {
            var saveToPath = Path.GetTempFileName();
            using (var sw = File.CreateText(saveToPath))
            {
                sw.Close();
            }

            var actual = PluginFileParser.ParseInstalledPluginsFile(saveToPath);
            new List<string>().ShouldEqual(actual);
            File.Delete(saveToPath);
        }

        [Fact]
        public void ParseInstalledPluginsFile_ReturnsEmptyList_OnNonExistsFile()
        {
            0.ShouldEqual(
                PluginFileParser.ParseInstalledPluginsFile(@"C:\3FE59448-ABBC-4629-B918-7C7C6AC42131.log").Count);
        }

        [Fact]
        public void ParseInstalledPluginsFile_ReturnsEmptyList_OnNullFileNAme()
        {
            0.ShouldEqual(
                PluginFileParser.ParseInstalledPluginsFile(null).Count);
        }

        [Fact]
        public void ParsePluginDescriptionFile_ReturnsDefaultPluginDescriptor_OnEmptyDescriptionFile()
        {
            const string description = "";
            var saveToPath = Path.GetTempFileName();
            File.WriteAllText(saveToPath, description);

            PluginFileParser.ParsePluginDescriptionFile(saveToPath).ShouldEqual(new PluginDescriptor());

            File.Delete(saveToPath);
        }

        [Fact]
        public void ParsePluginDescriptionFile_ReturnsDefaultPluginDescriptor_OnWhiteSpaceDescriptionFile()
        {
            var description = "                                                               ";
            var saveToPath = Path.GetTempFileName();
            File.WriteAllText(saveToPath, description);

            PluginFileParser.ParsePluginDescriptionFile(saveToPath).ShouldEqual(new PluginDescriptor());

            File.Delete(saveToPath);
        }


        [Fact]
        public void ParsePluginDescriptionFile_ThrowsFileNotFoundException()
        {
            typeof (ArgumentException).ShouldBeThrownBy(() => PluginFileParser.ParsePluginDescriptionFile(""));
        }

        [Fact]
        public void SaveInstalledPluginsFile_SaveAllLines()
        {
            var lines = new[] {"lines1", "line2", "line3"};

            var saveToPath = Path.GetTempFileName();
            PluginFileParser.SaveInstalledPluginsFile(lines, saveToPath);

            lines.ShouldEqual(File.ReadAllLines(saveToPath));

            File.Delete(saveToPath);
        }
    }
}