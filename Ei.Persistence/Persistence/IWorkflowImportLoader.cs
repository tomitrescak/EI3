using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ei.Persistence
{
    public interface IWorkflowImportLoader
    {
        string LoadImport(string name);
    }
}
