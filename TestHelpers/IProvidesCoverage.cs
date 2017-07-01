﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestHelpers
{
    /// <summary>
    /// Derived interface indicating that a class provides test coverage for a specific type.
    /// Appends the appropriate extension methods to actually validate that coverage.
    /// </summary>
    /// <typeparam name="TTested"></typeparam>
    public interface IProvidesCoverage<TTested> : IProvidesCoverage
    {
    }

    public interface IProvidesCoverage
    {
        [TestMethod]
        void ValidateCoverageTest();
    }

    public static class ICoverageExtensions
    {
        /// <summary>
        ///     Obtains the properties and methods of TTested (public and private) and confirms that for each, there exists
        ///     a corresponding test decorated with 
        /// </summary>
        /// <typeparam name="TTested"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool ValidateCoverage<TTested>(this IProvidesCoverage<TTested> target)
        {
            var testedType = typeof(TTested);
            var testerType = target.GetType();

            // For coverage to be valid, it should 
            // - For each tested member that requests coverage, there exists a tester member
            //      that corresponds to the member decorated for the type

            // Get anything explicit from the type...
            var coverage = GetCoverageFromType(testerType);
            var expected = GetTestRequirementsFromType(testedType);

            var coverageHs = new HashSet<Tuple<string, string>>(coverage);
            var failed = new List<Tuple<string, string>>();
            foreach (var testDef in expected)
            {
                if (!coverage.Contains(testDef))
                {
                    failed.Add(testDef);
                }
            }
            if (failed.Count > 0)
            {
                var msg = string.Join("\n\t", failed.Select(i => i.ToString()));
                Assert.Fail($"Coverage did not meet the specified requirements. The following items were missing: \n\t{msg}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Obtains the coverage specified by a type, on the assumption that each such member is a public 
        /// property or method, and is in turn decorated with the TestMethod decorator.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        /// <remarks>Inheritance amongst test classes has not, for now, been considered.</remarks>
        private static IEnumerable<Tuple<string, string>> GetCoverageFromType(Type target)
        {
            return target.GetMembers()
                .Select(m => new { Member = m, Attribute = m.GetCustomAttribute<CoversAttribute>() })
                .Where(mi => mi.Attribute != null && mi.Member.GetCustomAttribute<TestMethodAttribute>() != null)
                .Select(mi => new { MemberName = mi.Attribute.MemberName, Info = (mi.Attribute.ProvidesTotalCoverage ? new string[] { null } : mi.Attribute.TestItemPerformed) })
                .SelectMany(
                    mi => mi.Info.Select(testName => Tuple.Create(mi.MemberName, testName)));
        }

        private static IEnumerable<Tuple<string, string>> GetTestRequirementsFromType(Type target)
        {
            var baseMembers = target.GetMembers().Where(m => m.GetCustomAttribute<TestRequirementAttribute>() != null);
            var interfaceMembers = target.GetInterfaces().SelectMany(i => i.GetMembers().Where(m => m.GetCustomAttribute<TestRequirementAttribute>() != null));
            var members = baseMembers.Union(interfaceMembers).Distinct();

            return members
                .Select(m => new { Member = m, Attribute = m.GetCustomAttribute<TestRequirementAttribute>() })
                .Select(mi => new { MemberName = mi.Member.Name, Tests = mi.Attribute.Tests ?? new string[] { null } })
                .SelectMany(
                    mi => mi.Tests.Select(testName => Tuple.Create(mi.MemberName, testName))
                )
                .Distinct();
        }

        private const string TestException_failMessage = 
            "Did not receive an expected exception after performing the testMethod().";
        private const string TestException_failMessageUnexpected = 
            "Received an unexpected exception of type {0}";


        public static void TestException<TException>(
            this IProvidesCoverage target, Action testMethod,
            Func<object, string> failMessage = null,
            Func<Exception, string> failMessageUnexpectedException = null
            )
            where TException : Exception
        {
            
            target.TestException<TException, object>(new Func<object>(() => { testMethod(); return null; }), failMessage, failMessageUnexpectedException);
        }

        public static TResult TestException<TException, TResult>(
            this IProvidesCoverage target, Func<TResult> testMethod,
            Func<TResult, string> failMessage = null,
            Func<Exception, string> failMessageUnexpectedException = null
            )
            where TException : Exception
        {
            failMessage = failMessage ?? ((e) => TestException_failMessage);
            failMessageUnexpectedException = failMessageUnexpectedException ?? (e => string.Format(TestException_failMessageUnexpected, e.GetType().Name));
            TResult res = default(TResult);
            try
            {
                res = testMethod();
                var msg = failMessage != null ? failMessage(res) : TestException_failMessage;
                Assert.Fail(msg);
            }
            catch (TException ex)
            {
                ex.GetHashCode();
                // Test passed...
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var msg = failMessageUnexpectedException != null ?
                    failMessageUnexpectedException(ex) :
                    TestException_failMessageUnexpected;
                Assert.Fail(msg);
            }
            return res;
        }


    }

    
}
