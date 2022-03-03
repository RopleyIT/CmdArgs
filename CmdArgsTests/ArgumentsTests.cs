using Microsoft.VisualStudio.TestTools.UnitTesting;
using CmdArgs;
using System.Linq;
using System.Reflection;

namespace CmdArgsTests
{
    [TestClass]
    public class ArgumentsTests
    {
        [TestMethod]
        public void BuildsCorrectly()
        {
            string[] mdArgs =
            {
                "-t", "some text",
                "--integer", "1443",
                "-i", "42",
                "-f", "3.1416"
            };

            MyArgs args = Arguments<MyArgs>.Parse(mdArgs);
            Assert.AreEqual("some text", args.TArg);
            Assert.AreEqual(42, args.MyNumber);
            Assert.AreEqual(3.1416, args.MyDouble);
        }
    }
}
