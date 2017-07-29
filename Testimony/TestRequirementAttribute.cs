using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testimony
{
    /// <summary>
    /// Decorator that specifies a test that should be implemented in order for a corresponding <see cref="IProvidesCoverage{TTested}"/>
    /// bearer to pass validation.
    /// </summary>
    public class TestRequirementAttribute : Attribute
    {
        /// <summary>
        /// Specifies that a test must exist for the method/property. Depending on the implementation of <see cref="IProvidesCoverage{TTested}"/>,
        /// this may or may not be required, since all public members may be tested by default.
        /// </summary>
        public TestRequirementAttribute()
        {
        }

        /// <summary>
        /// Specifies that a test must exist with a specific name for for validation of test coverage to pass.
        /// </summary>
        /// <param name="test"></param>
        public TestRequirementAttribute(params string[] test)
        {
            this.Tests = test;
        }

        /// <summary>
        /// The name of the test to perform for the specified member.
        /// </summary>
        public string[] Tests { get; private set; }
    }
}
