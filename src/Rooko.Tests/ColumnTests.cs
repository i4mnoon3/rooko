using System;
using NUnit.Framework;
using Rooko.Core;

namespace Rooko.Tests
{
    [TestFixture]
    public class ColumnTests
    {
        [Test]
        public void TestProperties()
        {
            var c = new Column("name");
            Assert.AreEqual("name", c.Name);
            Assert.AreEqual("varchar(255)", c.Type);
            Assert.AreEqual("name", c.ToString());
        }
    }
}
