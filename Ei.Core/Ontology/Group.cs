using System.Linq;
using System.Net;
using Ei.Core.Runtime;

namespace Ei.Core.Ontology{

    /**
     * This class pairs Organisation with a role
     */
    public class Group
    {
//        private Organisation organisation;
//        private Role role;
//
//
//        //public Organisation Organisation { get; private set; }
//        public string OrganisationId { get; }
//
//        //public Role Role { get; private set; }
//        public string RoleId { get; }
//
//        public Group(OrganisationRoleDao organisationRole)
//        {           
//            this.OrganisationId = organisationRole.OrganisationId;
//            this.RoleId = organisationRole.RoleId;
//        }
//
//        public Organisation GetOrganisation(Institution ei)
//        {
//            if (this.organisation == null)
//            {
//                this.organisation = ei.OrganisationById(this.OrganisationId);
//            }
//            return this.organisation;
//        }
//
//        public Role GetRole(Institution ei)
//        {
//            if (this.role == null)
//            {
//                this.role = ei.RoleById(this.RoleId);
//            }
//            return this.role;
//        }
//
//        public Group(string organisationId, string roleId)
//        {
//            this.OrganisationId = organisationId;
//            this.RoleId = roleId;
//        }

        public Organisation Organisation { get;  }
        public Role Role { get;  }

        public Group(Organisation organisation, Role role)
        {
            this.Organisation = organisation;
            this.Role = role;

            if (this.Organisation == null && this.Role == null)
            {
                throw new InstitutionException("Organisation or role needs to be specified");
            }
        }

        public Group(Institution ei, string orgId, string roleId)
        {
             
            this.Organisation = (orgId == "0" || string.IsNullOrEmpty(orgId)) ? null : ei.Organisations.First(w => w.Id == orgId);
            this.Role = (roleId == "0" || string.IsNullOrEmpty(roleId)) ? null : ei.Roles.First(w => w.Id == roleId);
        }

        public override string ToString()
        {
            if (this.Organisation == null)
            {
                return this.Role.Id;
            }
            if (this.Role == null)
            {
                return this.Organisation.Id;
            }
            return this.Organisation.Id + ", " + this.Role.Id;
        }
    }
}