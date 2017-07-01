using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHelpers.Samples
{
    internal interface ITarget
    {
        [TestRequirement()]
        string Example { get; }

        [TestRequirement("Getter")]
        string Example2 { get; }
    }
}
