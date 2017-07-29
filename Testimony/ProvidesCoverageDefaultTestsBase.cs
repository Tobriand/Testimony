using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Testimony
{
    /// <summary>
    ///     Confirms that all implementations of <see cref="IProvidesCoverage{TTested}"/> actually
    ///     cover what they purport to do. However, in order for this method to provide intended
    ///     coverage, it must execute from the context of your test assembly. As such, for each 
    ///     test assembly there should be at least one class decorated with <see cref="TestClassAttribute"/> 
    ///     inheriting from <see cref="ProvidesCoverageDefaultTestsBase"/>.
    /// </summary>
    public abstract class ProvidesCoverageDefaultTestsBase
    {
        /// <summary>
        /// Guarantee that all assemblies are loaded (along with dependencies)
        /// at the point any test in this class is called.
        /// </summary>
        public ProvidesCoverageDefaultTestsBase()
        {
            var assemblies = this.GetType().Assembly.GetReferencedAssemblies();
            foreach (var aname in assemblies)
                Assembly.Load(aname);
        }

        /// <summary>
        ///     Tests that, for all identifiable implementations of 
        ///     <see cref="IProvidesCoverage{TTested}"/>, <see cref="IProvidesCoverage{TTested}.ValidateCoverageTest"/>
        ///     has a <see cref="TestMethodAttribute"/> appended. Does not actually
        ///     perform those tests, since that would be very prohibitive in terms of 
        ///     causing all tests to fail unexpectedly.
        /// </summary>
        [TestMethod]
        public void TestAllCoverageInReferencedAssemblies()
        {
            var coverage = typeof(IProvidesCoverage);
            var coverageProviders = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.DefinedTypes)
                .Where(ti => !ti.IsAbstract && !ti.IsInterface && coverage.IsAssignableFrom(ti) && ti != coverage)
                .ToList();

            var invalidProviders = coverageProviders
                .SelectMany(ti => ti.GetMember(nameof(IProvidesCoverage.ValidateCoverageTest)))
                .Where(mi => mi.GetCustomAttribute<TestMethodAttribute>() == null || mi.ReflectedType.GetCustomAttribute<TestClassAttribute>() == null)
                .ToList();
            Console.WriteLine($"Some implementations of {nameof(IProvidesCoverage)} do not test the coverage they provide: \n\t{string.Join("\n\t", invalidProviders.Select(mi => mi.DeclaringType.Name))}");
            Assert.IsTrue(invalidProviders.Count == 0, $"Some implementations of {nameof(IProvidesCoverage)} do not test the coverage they provide. Please see output for details.");
        }
    }
}
