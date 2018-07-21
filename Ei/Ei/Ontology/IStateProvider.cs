﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ei.Ontology
{
    public interface IStateProvider
    {
        string Name { get; }
        void NotifyParameterChanged(string ownerString, object paramValue);
    }
}
