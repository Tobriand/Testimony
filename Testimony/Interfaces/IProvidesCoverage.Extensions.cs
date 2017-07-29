using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testimony;
using Testimony.Attributes;
using System.IO;
using System.Reflection;


namespace Testimony.Interfaces
{
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
            var skips = GetSkipTestsFromType(testerType);
            coverage = coverage.Union(skips).ToList();

            var adoptions = GetAdoptionsForTested(testerType, testedType);
            var expected = GetTestRequirementsFromType(testedType);
            expected = expected.Union(adoptions);

            var coverageHs = new HashSet<Tuple<string, string>>(coverage, new InvariantCultureIgnoreCaseTupleComparer());
            var failed = new List<Tuple<string, string>>();
            foreach (var testDef in expected)
            {
                if (!coverageHs.Contains(testDef))
                {
                    failed.Add(testDef);
                }
            }
            if (failed.Count > 0)
            {
                var msg = string.Join($"{Environment.NewLine}\t", failed.Select(i => i.ToString()));
                msg = $"Coverage for {testedType.Name} provided by {testerType.Name} did not meet the specified requirements. The following items were missing: \n\t{msg}";
                Console.WriteLine(msg);
                Assert.Fail($"Coverage did not meet the specified requirements. See test output for details.");
                return false;
            }

            return true;
        }

        private class InvariantCultureIgnoreCaseTupleComparer : IEqualityComparer<Tuple<string, string>>
        {
            public bool Equals(Tuple<string, string> x, Tuple<string, string> y)
            {
                return
                    StringComparer.InvariantCultureIgnoreCase.Equals(x.Item1, y.Item1) &&
                    StringComparer.InvariantCultureIgnoreCase.Equals(x.Item2, y.Item2);
            }

            public int GetHashCode(Tuple<string, string> obj)
            {
                int res;
                unchecked
                {
                    res =
                        (obj.Item1 != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(obj.Item1) : 0) +
                        (obj.Item2 != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(obj.Item2) : 0);
                }
                return res;
            }
        }

        private static IEnumerable<Tuple<string, string>> GetAdoptionsForTested(Type testerType, Type testedType)
        {
            var attribs = testerType
                .GetCustomAttributes<AdoptRequirementsFromAttribute>();
            var validAdoptionSources = attribs
                .Where(arfa => arfa.IsValid())
                .Where(arfa => arfa.Original.IsAssignableFrom(testedType))
                .ToList();
            foreach (var adoptionSource in validAdoptionSources)
                Console.WriteLine($"Adopting tests from {adoptionSource.TestSource.Name} by virtue of {adoptionSource.Original.Name}");
            return validAdoptionSources
                .SelectMany(arfa => GetTestRequirementsFromType(arfa.TestSource))
                .Distinct()
                .ToList();
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
                    mi => mi.Info.Select(testName => Tuple.Create(mi.MemberName, testName))
                )
                ;
        }

        private static List<Tuple<string, string>> GetSkipTestsFromType(Type target)
        {
            return target
                    .GetCustomAttributes<SkipTestAttribute>()
                    .Where(sta => sta.Member != null)
                    .Select(sta => sta.GetTestDef(true))
                    .ToList();
        }

        private static IEnumerable<Tuple<string, string>> GetTestRequirementsFromType(Type target)
        {
            var baseMembers = target.GetMembers().Where(m => m.GetCustomAttribute<TestRequirementAttribute>() != null);
            var interfaceMembers = target.GetInterfaces().SelectMany(i => i.GetMembers().Where(m => m.GetCustomAttribute<TestRequirementAttribute>() != null));
            var members = baseMembers.Union(interfaceMembers).Distinct();
            var classLevelDeclarations = target.GetCustomAttributes<ClassTestRequirementsAttribute>();

            var baseClassMembers = classLevelDeclarations
                .Select(cld => new { CLD = cld, Members = cld.GetRelevantMembers(target) });
            var interfaceClassMembers = target.GetInterfaces()
                .SelectMany(i => i.GetCustomAttributes<ClassTestRequirementsAttribute>().Select(cld => new { Interface = i, CLD = cld }))
                .Where(cld => cld.CLD != null)
                .Select(icld => new { CLD = icld.CLD, Members = icld.CLD.GetRelevantMembers(icld.Interface) });
            var classMembers = baseClassMembers.Union(interfaceClassMembers)
                .SelectMany(cldi => cldi.CLD.GlobalTests.Select(tn => new { TestName = tn, Members = cldi.Members }))
                .SelectMany(nm => nm.Members.Select(m => Tuple.Create(m.Name, nm.TestName)))
                .Distinct();

            var memberItems = members
                .Select(m => new { Member = m, Attribute = m.GetCustomAttribute<TestRequirementAttribute>() })
                .Select(mi => new { MemberName = mi.Member.Name, Tests = mi.Attribute.Tests ?? new string[] { null } })
                .SelectMany(
                    mi => mi.Tests.Select(testName => Tuple.Create(mi.MemberName, testName))
                );
            return classMembers
                .Union(memberItems)
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
