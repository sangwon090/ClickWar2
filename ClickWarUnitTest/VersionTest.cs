using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClickWar2;

namespace ClickWarUnitTest
{
    [TestClass]
    public class VersionTest
    {
        [TestMethod]
        public void TestLeftIsBig1()
        {
            ClickWar2.Utility.Version left = new ClickWar2.Utility.Version(1, 2, 3, 4);
            ClickWar2.Utility.Version right = new ClickWar2.Utility.Version(1, 2, 3, 3);

            Assert.IsTrue(left > right);
        }

        [TestMethod]
        public void TestLeftIsBig2()
        {
            ClickWar2.Utility.Version left = new ClickWar2.Utility.Version(1, 2, 3, 4);
            ClickWar2.Utility.Version right = new ClickWar2.Utility.Version(1, 2, 2, 5);

            Assert.IsTrue(left > right);
        }

        [TestMethod]
        public void TestLeftIsBig3()
        {
            ClickWar2.Utility.Version left = new ClickWar2.Utility.Version(2, 2, 3, 4);
            ClickWar2.Utility.Version right = new ClickWar2.Utility.Version(1, 9, 4, 5);

            Assert.IsTrue(left > right);
        }

        [TestMethod]
        public void TestLeftIsSmall1()
        {
            ClickWar2.Utility.Version left = new ClickWar2.Utility.Version(0, 0, 2, 7);
            ClickWar2.Utility.Version right = new ClickWar2.Utility.Version(0, 0, 3, 0);

            Assert.IsTrue(left < right);
        }
    }
}
