using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestHelpers
{
    /// <summary>
    /// Confirms that all implementations of <see cref="IProvidesCoverage{TTested}"/> actually
    /// cover what they purport to do. However, in order for this method to provide intended
    /// coverage, it must execute from the context of your test assembly. This can be
    /// accomplished by creating a class therein that inherits from this, and overriding the 
    /// method.
    /// </summary>
    /// 
    [TestClass]
    public abstract class ProvidesCoverageDefaultTestsBase
    {
        /// <summary>
        /// Guarantee that all assemblies are loaded, such that 
        /// </summary>
        public ProvidesCoverageDefaultTestsBase()
        {
            var assemblies = this.GetType().Assembly.GetReferencedAssemblies();
            foreach (var aname in assemblies)
                Assembly.Load(aname);
        }

        /// <summary>
        ///     Helper method providing an implementation of a test that will guarantee
        ///     that every implementation of <see cref="IProvidesCoverage{TTested}"/> actually
        ///     tests every member that it is supposed to. This is accomplished by jumping on 
        ///     the front of the first test that enters here to call 
        ///     <see cref="TestAllCoverageInReferencedAssemblies_TestPart"/>. Once that's
        ///     happened once, it skips through on all subsequent tests.
        /// </summary>
        [TestInitialize]
        public void TestAllCoverageInReferencedAssemblies()
        {
            lock (TestAllCoverageInReferencedAssemblies_Lock)
            {
                if (this.AllCoverageTestCalled)
                    return;
                this.TestAllCoverageInReferencedAssemblies_TestPart();
            }

        }
        private object TestAllCoverageInReferencedAssemblies_Lock = new object();

        /// <summary>
        ///     Tests that, for all identifiable implementations of 
        ///     <see cref="IProvidesCoverage{TTested}"/>, <see cref="IProvidesCoverage{TTested}.ValidateCoverageTest"/>
        ///     has a <see cref="TestMethodAttribute"/> appended. Does not actually
        ///     perform those tests, since that would be very prohibitive in terms of 
        ///     causing all tests to fail unexpectedly.
        /// </summary>
        [TestMethod]
        public void TestAllCoverageInReferencedAssemblies_TestPart()
        {
            // For this to work, all referenced assemblies must have been loaded, along with
            // dependencies.
            this.AllCoverageTestCalled = true;
            var coverage = typeof(IProvidesCoverage);
            var coverageProviders = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.DefinedTypes)
                .Where(ti => coverage.IsAssignableFrom(ti) && ti != coverage && !ti.IsInterface)
                .ToList();

            var invalidProviders = coverageProviders
                .SelectMany(ti => ti.GetMember(nameof(IProvidesCoverage.ValidateCoverageTest)))
                .Where(mi => mi.GetCustomAttribute<TestMethodAttribute>() == null || mi.DeclaringType.GetCustomAttribute<TestClassAttribute>() == null)
                .ToList();
            Assert.IsTrue(invalidProviders.Count == 0, $"Some implementations of {nameof(IProvidesCoverage)} do not test the coverage they provide: \n\t{string.Join("\n\t", invalidProviders.Select(mi => mi.DeclaringType.Name))}");
        }

        protected bool AllCoverageTestCalled
        {
            get; set;
        }

        public abstract void DecorateMeWithTestMethod();
    }
}
