using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClickWar2.Utility;

namespace ClickWarUnitTest
{
    [TestClass]
    public class EncryptionTest
    {
        [TestMethod]
        public void TestShortEncryption()
        {
            string data = "Hello, 안녕!! ㅎ";
            string key;

            string encoded = Security.EncodeEx(data, out key);

            string decoded = Security.DecodeEx(encoded, key);

            Assert.AreEqual(data, decoded);
        }

        [TestMethod]
        public void TestLongEncryption()
        {
            string data = string.Concat(Enumerable.Repeat("Hello, 안녕!! ㅎ", 4096));
            string key;

            string encoded = Security.EncodeEx(data, out key);

            string decoded = Security.DecodeEx(encoded, key);

            Assert.AreEqual(data, decoded);
        }
    }
}
