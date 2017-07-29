using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testimony.Samples.PostDesignTime
{
    /// <summary>
    /// Specifies an interface that does NOT request any tests be performed on it.
    /// We'll change this later...
    /// </summary>
    public interface IDesignTime : IDeeperDesignTime
    {
        void Method1();
        string Method2();
        int Foo { get; set; }
        event EventHandler OmgItsAnEvent;
    }

    public interface IDeeperDesignTime
    {
        void DeepMethod();
    }
}
