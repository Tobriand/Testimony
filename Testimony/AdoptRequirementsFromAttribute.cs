using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testimony
{
    /// <summary>
    ///     Includes TestRequirements defined in <paramref name="testSource"/> when evaluating 
    ///     <paramref name="original"/> when applied to an <see cref="IProvidesCoverage"/> 
    ///     instance. Should only be applied if the target of any coverage validation
    ///     is assignable from original.
    /// </summary>
    public class AdoptRequirementsFromAttribute : Attribute
    {
        public AdoptRequirementsFromAttribute(Type testSource, Type original)
        {
            this.TestSource = testSource;
            this.Original = original;
        }

        public Type Original { get; private set; }
        public Type TestSource { get; private set; }

        public bool IsValid()
        {
            return this.Original != null && this.TestSource != null && this.Original.IsAssignableFrom(this.TestSource);
        }
    }
}
