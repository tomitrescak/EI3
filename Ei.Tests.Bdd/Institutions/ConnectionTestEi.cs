using Ei.Ontology;
using Ei.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ei.Tests.Bdd.Institutions
{
    public class ConnectionTestEi : Institution
    {
        public ConnectionTestEi(): base("ConnectionTest") {

            // init basic properties
            this.Name = "Connection Test";
            this.Description = "Connection Test Description";

            // init components
            this.InitRoles();
        }

        // init institutional parts

        private void InitRoles() {
            var citizenRole = new Role("1");
        }

        // abstract implementation 

        public override Institution Instance {
            get {
                return new ConnectionTestEi();
            }
        }
    }

    public class CitizenRole: Role
    {
        public CitizenRole(): base("1") {
            this.Name = "Citizen";
            this.Description = null;
        }

        public class Properties: VariableState
        {
            // individual descriptors
            static VariableDefinition<int, Properties> parentParameterDefinition = new VariableDefinition<int, Properties>("ParentParameter");

            // all descriptors
            static IVariableDefinition[] variableDefinitions = new IVariableDefinition[] {
                parentParameterDefinition
            };

            // fields
            public int ParentParameter { get; set; }

            // ctors
            public Properties(): base(variableDefinitions) {
                this.ParentParameter = parentParameterDefinition.TypedDefaultValue;
            }

            public Properties(IVariableDefinition[] childDefinitions) : base(variableDefinitions.Concat(childDefinitions).ToArray()) {
                this.ParentParameter = parentParameterDefinition.TypedDefaultValue;
            }
        }
    }
}
