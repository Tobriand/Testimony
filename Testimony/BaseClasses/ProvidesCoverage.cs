using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testimony.Interfaces;

namespace Testimony.BaseClasses
{
    public abstract class ProvidesCoverage<TCovered> : IProvidesCoverage<TCovered>
    {
        [TestMethod]
        public void ValidateCoverageTest()
        {
            this.ValidateCoverage();
        }
    }
}
