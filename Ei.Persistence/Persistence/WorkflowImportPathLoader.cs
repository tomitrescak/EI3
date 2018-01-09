using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ei.Persistence
{
    public class WorkflowImportPathLoader : IWorkflowImportLoader
    {
        private string directory;

        public WorkflowImportPathLoader(string directory)
        {
            this.directory = directory;
        }

        public string LoadImport(string name)
        {
            var importPath = Path.Combine(this.directory, Path.Combine("Workflows", name + ".yaml"));
            return File.ReadAllText(importPath);
        }
    }
}

