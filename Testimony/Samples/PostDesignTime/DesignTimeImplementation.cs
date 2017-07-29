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

        public event EventHandler OmgItsAnEvent;

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
