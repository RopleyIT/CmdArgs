using Microsoft.VisualStudio.TestTools.UnitTesting;
using CmdArgs;
using System.Linq;
using System.Reflection;

namespace CmdArgsTests
{
    [TestClass]
    public class ArgAttributes
    {
        [TestMethod]
        public void CanRecordProperties()
        {
            var myAttributes = System.Attribute.GetCustomAttributes(typeof(MyArgs), false);
            Assert.IsInstanceOfType(myAttributes, typeof(System.Attribute[]));
            Assert.IsTrue(myAttributes.Any(a => a is CmdArgs.ArgSetAttribute));

            var properties = typeof(MyArgs).GetProperties();
            CheckProperty(properties, "TArg", "-t");
            CheckProperty(properties, "MyNumber", "-i", "--integer");
            CheckProperty(properties, "MyDouble", "-f");
        }

        private static void CheckProperty(PropertyInfo[]? properties, string propertyName,
            string shortTag, string longTag = "")
        {
            Assert.IsNotNull(properties);
            var property = properties.FirstOrDefault(p => p.Name == propertyName);
            Assert.IsNotNull(property);
            object[]? attribute = property.GetCustomAttributes(typeof(ArgAttribute), false);
            Assert.IsNotNull(attribute);
            Assert.IsTrue(attribute.Length > 0);
            var myArg = attribute[0] as ArgAttribute;
            Assert.AreEqual(shortTag, myArg?.Tag);
            if (attribute.Length > 1)
            {
                myArg = attribute[1] as ArgAttribute;
                Assert.AreEqual(longTag, myArg?.Tag);
            }

        }
    }

    [ArgSet]
    public class MyArgs
    {
        [Arg("-t")]
        public string? TArg { get; set; } 

        [Arg("-i")]
        [Arg("--integer")]
        public int? MyNumber { get; set; } 

        [Arg("-f")]
        public double? MyDouble { get; set; } 
    }
}