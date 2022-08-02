using CmdArgs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            var x = Assert.ThrowsException<ArgumentException>(() =>
            {
                MyArgs args = new();
                Arguments.Parse(mdArgs, args);
            });
            Assert.AreEqual("Argument -i already used", x.Message);
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

            var argEx = Assert.ThrowsException<ArgumentException>(() =>
            {
                MyArgs args = new();
                Arguments.Parse(mdArgs, args);
            });
            Assert.AreEqual("Argument \"-f\" missing or mispositioned", argEx.Message);
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

        [TestMethod]
        public void EnforcesArgSetOrder()
        {
            string[] mdArgs =
            {
                "-t", "some text",
                "-to", "other text",
                "-f", "3.1416",
                "-fo", "2.1828"
            };

            MyArgs args = new();
            OtherArgs other = new();
            var argEx = Assert.ThrowsException<ArgumentException>(() =>
            {
                Arguments.Parse(mdArgs, args, other);
            });
            Assert.AreEqual("Argument \"-f\" missing or misplaced", argEx.Message);
        }

        [TestMethod]
        public void EnforcesArgSetOrderForOptionalArguments()
        {
            string[] mdArgs =
            {
                "-t", "some text",
                "-f", "3.1416",
                "-to", "other text",
                "-i", "1443",
                "-fo", "2.1828"
            };

            MyArgs args = new();
            OtherArgs other = new();
            var argEx = Assert.ThrowsException<ArgumentException>(() =>
            {
                Arguments.Parse(mdArgs, args, other);
            });
            Assert.AreEqual("Argument \"-fo\" missing or mispositioned", argEx.Message);
        }

        [TestMethod]
        public void CanBuildDescription()
        {
            MyArgs args = new();
            OtherArgs other = new();
            string description = Arguments.Describe(args, other);
            Assert.IsTrue(description.Length > 0);
        }
    }
}
