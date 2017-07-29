using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testimony.Samples.PostDesignTime
{
    public class DesignTimeImplementation : IDesignTime
    {
        public int Foo
        {
            get; set;
        }

#pragma warning disable
        // No need to warn about an unused event, because it's there for member testing, not for use.
        public event EventHandler OmgItsAnEvent;
#pragma warning restore

        public void DeepMethod()
        {
            throw new NotImplementedException();
        }

        public void Method1()
        {
            
        }

        public string Method2()
        {
            return "Bar";
        }
    }
}
