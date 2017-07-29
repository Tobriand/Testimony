using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testimony.Samples
{
    [TestClass]
    [SkipTest(nameof(ClassLevelExample.DoSomething))]
    [SkipTest(nameof(ClassLevelExample.ExampleProp))]
    [SkipTest(nameof(ClassLevelExample.Example))]
    [SkipTest(nameof(ClassLevelExample.ExampleProp), "NullChecK")]
    [SkipTest(nameof(ClassLevelExample.SomeMethod), "ArgNullException")]
    [SkipTest(nameof(ClassLevelExample.Example2), "Getter")]
    public class ClassLevelTest : ProvidesCoverage<ClassLevelExample>
    {
    }
}
