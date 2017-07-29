using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testimony;

namespace Testimony.Samples.PostDesignTime
{
    /// <summary>
    /// Demonstrates that where no tests were defined at design time, but some should have 
    /// been, they chan be defined in a test-interface and adopted from there.
    /// </summary>
    [TestClass]
    [SkipTest(nameof(IDesignTime.Method1))]
    [SkipTest(nameof(IDesignTime.Method1), "RaisesEventOnCall")]
    [SkipTest(nameof(IDesignTime.Method2))]
    [SkipTest(nameof(IDesignTime.OmgItsAnEvent))]
    [SkipTest(nameof(IDesignTime.Foo))]
    [SkipTest(nameof(IDesignTime.DeepMethod))]
    [AdoptRequirementsFrom(typeof(IDesignTimeTest), typeof(IDesignTime))]
    public class DesignTimeImplTester : ProvidesCoverage<DesignTimeImplementation>
    {
    }
}
