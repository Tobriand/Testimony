using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testimony.Attributes;
using Testimony.Data;

namespace Testimony.Samples.PostDesignTime
{
    [ClassTestRequirements(
        MemberTargetType.All, 
        MemberTargetAccessibility.InheritedExceptObject | MemberTargetAccessibility.Public
        )]
    public interface IDesignTimeTest : IDesignTime
    {
        [TestRequirement("RaisesEventOnCall")]
        new void Method1();
    }
}
