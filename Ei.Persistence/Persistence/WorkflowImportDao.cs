using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Ei.Persistence;

namespace Ei.Persistence
{
    public class WorkflowImportDao
    {
        public WorkflowDao Workflow { get; set; }

 //       public List<ActionDao> Actions { get; set; }
        public List<ClassDao> Types { get; set; }
        public List<OrganisationDao> Organisations { get; set; }
        public List<RoleDao> Roles { get; set; }
    }
}
