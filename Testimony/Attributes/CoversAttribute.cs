using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testimony.Attributes
{
    /// <summary>
    /// Attribute that specifies that a method provides some level of coverage for a method or property.
    /// </summary>
    public class CoversAttribute : Attribute
    {
        public CoversAttribute(Type type, string memberName)
            : this(type, memberName, default(IEnumerable<string>))
        {
        }

        public CoversAttribute(Type type, string memberName, params string[] tests)
            : this(type, memberName, tests as IEnumerable<string>)
        {
        }

        private CoversAttribute(Type type, string memberName, IEnumerable<string> tests)
        {
            if (type == null)
                throw new ArgumentNullException($"Expected the name of a type for which the test is valid.");
            this.Type = type;
            this.MemberName = memberName;
            this.TestItemPerformed = tests as string[] ?? tests?.ToArray();
        }

        public Type Type { get; private set; }
        public string[] TestItemPerformed { get; private set; }
        public bool ProvidesTotalCoverage { get { return this.TestItemPerformed == null; } }
        public string MemberName { get; private set; }
    }
}
