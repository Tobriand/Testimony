﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testimony.Attributes;
using Testimony.Data;

namespace Testimony.Samples
{
    [ClassTestRequirements]
    [ClassTestRequirements(MemberTargetType.Properties, "NullCheck")]
    public interface IClassLevelExample
    {
        string ExampleProp { get; set; }

        void DoSomething();
    }
}
