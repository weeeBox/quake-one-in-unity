using NUnit.Framework;
using System;
using System.IO;

namespace Test
{
    [TestFixture]
    public class DynamicArrayTest
    {
        [Test]
        public void TestAdd()
        {
            var array = new DynamicArray<int>();
            array[1] = 1;
            array[0] = 0;
            array[4] = 4;
            array[2] = 2;

            Assert.AreEqual(5, array.length);
            Assert.AreEqual(0, array[0]);
            Assert.AreEqual(1, array[1]);
            Assert.AreEqual(2, array[2]);
            Assert.AreEqual(0, array[3]);
            Assert.AreEqual(4, array[4]);
        }
    }
}

