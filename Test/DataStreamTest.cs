using NUnit.Framework;
using System;

namespace Test
{
    [TestFixture]
    public class DataStreamTest
    {
        [Test]
        public void TestFoo()
        {
            DataStream stream = new DataStream();
            VECTOR3_T v = stream.readStruct<VECTOR3_T>();

        }
    }
}

