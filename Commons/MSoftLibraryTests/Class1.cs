using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MSoftLibraryTests
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void EncodeYesString()
        {
            var yes = Encoding.ASCII.GetBytes("Yes");
            Console.Out.WriteLine(Convert.ToBase64String(yes));
            Assert.AreEqual("WWVz", Convert.ToBase64String(yes));
            var no = Encoding.ASCII.GetBytes("No");
            Console.Out.WriteLine(Convert.ToBase64String(no));
            Assert.AreEqual("Tm8=", Convert.ToBase64String(no));
        }
    }
}
