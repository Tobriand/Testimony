using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testimony.Attributes;
using Testimony.Interfaces;

namespace Testimony.Samples.PostDesignTime
{
    /// <summary>
    /// Pr
    /// </summary>
    public interface IDesignTimeTests : IProvidesCoverage<IDesignTimePostHocTestReqs>
    {
        [Covers(typeof(IDesignTimePostHocTestReqs), nameof(IDesignTimePostHocTestReqs.Method1), "RaisesEventOnCall")]
        void TestMethodOne_RaisesEventOnCall();
    }
}
