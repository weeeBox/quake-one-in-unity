using NUnit.Framework;
using System;
using System.IO;

namespace Test
{
    [TestFixture]
    public class DataStreamTest
    {
        [Test]
        public void TestFoo()
        {
            using (FileStream stream = File.OpenRead("e1m1.bsp"))
            {
                DataStream data = new DataStream(stream);
                var h = data.readStruct<HEADER_T>();
                int v = h.version;
            }

        }
    }
}

