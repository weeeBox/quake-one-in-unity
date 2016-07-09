using NUnit.Framework;
using System;
using System.IO;

namespace Test
{
    [TestFixture]
    public class BSPTest
    {
        [Test]
        public void TestFoo()
        {
            using (FileStream stream = File.OpenRead("e1m1.bsp"))
            {
                DataStream data = new DataStream(stream);
                new BSP(data);
            }
        }
    }
}

