using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testimony.Attributes
{
    /// <summary>
    /// Attribute that can be appended to a test-providing class to indicate that
    /// that test providing class does not (and does not intend to) implement a 
    /// test that is specified by its target. For instance, an implementation of the
    /// interface might not be complete because it does not need to be, and testing
    /// a lot of NotImplementedExceptions would therefore be excessive.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SkipTestAttribute : Attribute
    {
        public SkipTestAttribute(string member) : 
            this(member, null)
        {
        }

        public SkipTestAttribute(string member, string test)
        {
            this.Member = member;
            this.Test = test;
        }

        public string Member { get; private set; }
        public string Test { get; private set; }

        public Tuple<string, string> GetTestDef(bool log = false)
        {
            if (this.Member == null)
                throw new ArgumentNullException($"It is not possible for {nameof(Member)} to be null in a test def.");
            var res = Tuple.Create(this.Member, this.Test);
            if (log)
                Console.WriteLine($"Created skip member: {res.ToString()}");
            return res;
        }
    }
}
