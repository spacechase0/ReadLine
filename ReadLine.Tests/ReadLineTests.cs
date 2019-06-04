using System.Linq;
using static ReadLine.ReadLine;
using NUnit.Framework;


namespace ReadLine.Tests
{
    [TestFixture]
    internal sealed class ReadLineTests
    {
        [SetUp]
        public void TestInitialize()
        {
            string[] history = { "ls -a", "dotnet run", "git init" };
            AddHistory(history);
        }

        [TearDown]
        public void TestDispose()
        {
            // If all above tests pass clear history works.
            ClearHistory();
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
    }
}
