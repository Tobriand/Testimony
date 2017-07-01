using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelpers;
using System.Linq;
using System.Reflection;
using TestHelpers.Samples;

namespace TestHelpers
{
    [TestClass]
    public class TesterTests : ProvidesCoverageDefaultTestsBase
    {
        [TestMethod]
        public override void DecorateMeWithTestMethod()
        {
            
        }

        [TestMethod]
        public void Tester_InterfacesCanProvideAttributes()
        {
            var testType = typeof(TargetImplementation);

            // Expect to be able to retrieve the TestRequirement decorators from the interface...
            string[] expectedNames = new string[] { null, "Getter" };
            
            var interfaces = testType.GetInterfaces();
            var attributesPresentFromInterface = interfaces
                .SelectMany(
                    i => i.GetMembers()
                        .SelectMany(m => m.GetCustomAttributes(true).OfType<TestRequirementAttribute>())
                )
                .SelectMany(a => a.Tests ?? new string[] { null })
                .ToArray();
            Assert.IsTrue(expectedNames.SequenceEqual(attributesPresentFromInterface));
        }

        

        
    }
}
