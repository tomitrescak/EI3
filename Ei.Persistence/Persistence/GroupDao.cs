
namespace Ei.Persistence {
    /**
     * Defines relation of role belonging to an organisation
     */
    public struct GroupDao {
        
        public GroupDao(string organisationId, string roleId) 
        {
            this.OrganisationId = organisationId;
            this.RoleId = roleId;
        }

        /**
         * Id of the organsiation
         */
        public string OrganisationId { get; set; }

        /**
         * Id of the role
         */
        public string RoleId { get; set; }
    }
}