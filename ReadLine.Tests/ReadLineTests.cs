using System;
using System.Linq;
using static ReadLine.ReadLine;
using NUnit.Framework;


namespace ReadLine.Tests
{
    internal sealed class ReadLineTests : IDisposable
    {
        public ReadLineTests()
        {
            string[] history = { "ls -a", "dotnet run", "git init" };
            AddHistory(history);
        }

        [Test]
        public void TestNoInitialHistory() 
        {
            Assert.AreEqual(3, GetHistory().Count);
        }

        [Test]
        public void TestUpdatesHistory() 
        {
            AddHistory("mkdir");
            Assert.AreEqual(4, GetHistory().Count);
            Assert.AreEqual("mkdir", GetHistory().Last());
        }

        [Test]
        public void TestGetCorrectHistory() 
        {
            Assert.AreEqual("ls -a", GetHistory()[0]);
            Assert.AreEqual("dotnet run", GetHistory()[1]);
            Assert.AreEqual("git init", GetHistory()[2]);
        }

        public void Dispose()
        {
            // If all above tests pass
            // clear history works
            ClearHistory();
        }
    }
}
