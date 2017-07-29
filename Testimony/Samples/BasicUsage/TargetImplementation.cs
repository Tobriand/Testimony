using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testimony.Samples
{


    public class TargetImplementation : ITarget
    {
        public string Example
        {
            get { return "Hello"; }
        }

        public string Example2
        {
            get
            {
                return "World";
            }
        }
    }
}
