using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testimony.Attributes;

namespace Testimony.Samples
{

    public class ClassLevelExample : IClassLevelExample, ITarget
    {
        public string Example
        {
            get; set;
        }

        public string Example2
        {
            get;set;
        }

        public string ExampleProp
        {
            get;set;
        }

        public void DoSomething()
        {
            Console.WriteLine("I did something!");
        }

        [TestRequirement("ArgNullException")]
        public void SomeMethod(object arg) { }
    }
}
