using NUnit.Framework;
using System;
using System.IO;

namespace Test
{
    [TestFixture]
    public class MDLTest
    {
        [Test]
        public void TestFoo()
        {
            using (FileStream stream = File.OpenRead("Data/shambler.mdl"))
            {
                DataStream data = new DataStream(stream);
                new MDL(data, "shambler.mdl");
            }
        }
    }
}