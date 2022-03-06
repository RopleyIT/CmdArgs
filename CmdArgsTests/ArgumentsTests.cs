using Microsoft.VisualStudio.TestTools.UnitTesting;
using CmdArgs;
using System.Linq;
using System.Reflection;
using System;

namespace CmdArgsTests
{
    [TestClass]
    public class ArgumentsTests
    {
        [TestMethod]
        public void HandlesCollisionsCorrectly()
        {
            string[] mdArgs =
            {
                "-t", "some text",
                "--integer", "1443",
                "-i", "42",
                "-f", "3.1416"
            };

            Assert.ThrowsException<ArgumentException>(() =>
            {
                MyArgs args = new();
                Arguments.Parse(mdArgs, args);
            });
        }

        [TestMethod]
        public void HandlesArgsCorrectly()
        {
            string[] mdArgs =
            {
                "-t", "some text",
                "--integer", "1443",
                "-f", "3.1416"
            };

            MyArgs args = new();
            Arguments.Parse(mdArgs, args);
            Assert.AreEqual("some text", args.TArg);
            Assert.AreEqual(1443, args.MyNumber);
            Assert.AreEqual(3.1416, args.MyDouble);
        }

        [TestMethod]
        public void HandlesRequiredArgsCorrectly()
        {
            string[] mdArgs =
            {
                "-t", "some text",
                "-i", "1443",
            };

            Assert.ThrowsException<ArgumentException>(() =>
            {
                MyArgs args = new();
                Arguments.Parse(mdArgs, args);
            });
        }

        [TestMethod]
        public void HandlesOptionalArgsCorrectly()
        {
            string[] mdArgs =
            {
                "-t", "some text",
                "-f", "3.1416"
            };

            MyArgs args = new();
            Arguments.Parse(mdArgs, args);
            Assert.AreEqual("some text", args.TArg);
            Assert.AreEqual(null, args.MyNumber);
            Assert.AreEqual(3.1416, args.MyDouble);
        }

        [TestMethod]
        public void PopulatesMultipleArgSets()
        {
            string[] mdArgs =
            {
                "-t", "some text",
                "-f", "3.1416",
                "-to", "other text",
                "-fo", "2.1828"
            };

            MyArgs args = new();
            OtherArgs other = new();
            Arguments.Parse(mdArgs, args, other);
            Assert.AreEqual("some text", args.TArg);
            Assert.AreEqual(3.1416, args.MyDouble);
            Assert.AreEqual("other text", other.TArgOther);
            Assert.AreEqual(2.1828f, other.MyFloat);
        }
    }
}
