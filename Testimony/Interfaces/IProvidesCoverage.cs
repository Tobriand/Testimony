using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Testimony.Interfaces
{
    /// <summary>
    /// Derived interface indicating that a class provides test coverage for a specific type.
    /// Appends the appropriate extension methods to actually validate that coverage.
    /// </summary>
    /// <typeparam name="TTested"></typeparam>
    public interface IProvidesCoverage<TTested> : IProvidesCoverage
    {
    }

    public interface IProvidesCoverage
    {
        [TestMethod]
        void ValidateCoverageTest();
    }

    

    

    
}
