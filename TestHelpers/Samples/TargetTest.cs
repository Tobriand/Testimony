using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHelpers.Samples
{
    /// <summary>
    /// The test defined in this class will pass. Note the attribute that targets a specific 
    /// member and test category for coverage. To make these tests fail, comment out either the 
    /// test method attribute, or the ProvieesCoverageFor one.
    /// </summary>
    [TestClass]
    public class ExampleTester : IProvidesCoverage<TargetImplementation>
    {
        [TestMethod]
        public void ValidateCoverageTest()
        {
            this.ValidateCoverage();
        }

        [TestMethod]
        [Covers(typeof(TargetImplementation), nameof(ITarget.Example2), "Getter")]
        public void TestExample2_Getter()
        {
        }

        [TestMethod]
        [Covers(typeof(TargetImplementation), nameof(ITarget.Example))]
        public void TestExample()
        {
        }
    }
}
