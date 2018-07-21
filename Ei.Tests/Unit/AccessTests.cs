namespace Ei.Tests.Unit.Ontology
{
    using System.Collections.Generic;

    using Ei.Ontology;
    using Ei.Persistence;

    using NUnit.Framework;

    [TestFixture]
    class AccessTests
    {
        private static Institution ei;

        private static Institution Ei
        {
            get
            {
                if (ei == null)
                {
                    var dao = new InstitutionDao();
                    dao.Roles.Add(new RoleDao { Id = "0" });
                    dao.Roles.Add(new RoleDao { Id = "1" });
                    dao.Roles.Add(new RoleDao { Id = "2" });
                    dao.Roles.Add(new RoleDao { Id = "3" });
                    dao.Roles.Add(new RoleDao { Id = "4" });
                    dao.Roles.Add(new RoleDao { Id = "5" });
                    dao.Roles.Add(new RoleDao { Id = "6" });
                    dao.Roles.Add(new RoleDao { Id = "7" });
                    dao.Roles.Add(new RoleDao { Id = "8" });
                    dao.Roles.Add(new RoleDao { Id = "9" });

                    dao.Organisations.Add(new OrganisationDao { Id = "0" });
                    dao.Organisations.Add(new OrganisationDao { Id = "1" });
                    dao.Organisations.Add(new OrganisationDao { Id = "2" });
                    dao.Organisations.Add(new OrganisationDao { Id = "3" });
                    dao.Organisations.Add(new OrganisationDao { Id = "4" });
                    dao.Organisations.Add(new OrganisationDao { Id = "5" });
                    dao.Organisations.Add(new OrganisationDao { Id = "6" });

                    ei = new Institution(dao);
                    
                }
                return ei;
            }
        }


        [TestCase("1,2;mama|2,3;baba")]
        [TestCase("|2,3;baba")]
        [TestCase("2,3;baba|")]
        public void Access(string dao)
        {
            // parse string
            // format is: ALLOW|DENY and ALLOW: roles;conditions_roles;conditions

            var splitStr = dao.Split('|');
            var allow = ParseConditions(splitStr[0]);
            var deny = ParseConditions(splitStr[1]);

            var access = new Access(Ei, allow, deny);

            if (access.Allow != null)
            {
                Assert.AreEqual(access.Allow.Count, allow.Length);
                Assert.AreEqual(access.Allow[0].Conditions.Length, allow[0].Conditions.Length);
                Assert.AreEqual(access.Allow[0].Groups.Count, allow[0].Groups.Length);
            }
              
            if (access.deny != null)
            {
                Assert.AreEqual(access.deny.Count, deny.Length);
            }
        }

        //        // Allow and Deny have following structure
        //        // Allow: [] -> Array
        //
        [TestCase("1,2;|", "1,2", "3,4")]
        [TestCase("1,2;|", "6,7;1,2", "3,4")]
        [TestCase("0,2;|", "1,2", "")]
        [TestCase("1,0;|", "1,2", "2,1")]
        public void Parameter_AllowedRoledListed_AccessEvaluated(string accessString, string shouldAllowRoles, string shouldDenyRoles)
        {
            var access = ParseAccess(accessString);

            var allowRoleList = ParseOrganisationRoles(Ei, shouldAllowRoles);
            var denyRoleList = ParseOrganisationRoles(Ei, shouldDenyRoles);
        
            Assert.IsTrue(access.CanAccess(allowRoleList, null));
            Assert.IsFalse(access.CanAccess(denyRoleList, null));
        
        }




        [TestCase("|1,0;", "3,2", "1,2")]
        [TestCase("|0,2;", "0,1", "1,2")]
        [TestCase("|1,1;", "0,2", "1,1")]
        [TestCase("|1,0;", "4,3;4,1", "1,8")]
        public void Parameter_DenyRoledListed_AccessEvaluated(string accessString, string shouldAllowRoles, string shouldDenyRoles)
        {
            var access = ParseAccess(accessString);

            var allowRoleList = ParseOrganisationRoles(Ei, shouldAllowRoles);
            var denyRoleList = ParseOrganisationRoles(Ei, shouldDenyRoles);

            Assert.IsTrue(access.CanAccess(allowRoleList, null));
            Assert.IsFalse(access.CanAccess(denyRoleList, null));

        }

        // helpers


        private static List<Group> ParseOrganisationRoles(Institution ei, string val)
        {
            // parse organisation roles
        
            var organisationRolesList = new List<Group>();
        
            if (string.IsNullOrEmpty(val))
            {
                return organisationRolesList;
            }
        
            var pairs = val.Split(';');
            foreach (var pair in pairs)
            {
                var ids = pair.Split(',');
                organisationRolesList.Add(new Group(ei, ids[0], ids[1]));
            }
            return organisationRolesList;
        }

        private static Access ParseAccess(string dao)
        {
            var splitStr = dao.Split('|');
            var allow = ParseConditions(splitStr[0]);
            var deny = ParseConditions(splitStr[1]);

            return new Access(Ei, allow,  deny);
        }

        private static AccessConditionDao[] ParseConditions(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            var result = new List<AccessConditionDao>();

            // split access groups separated by _
            var accond = str.Split('_');

            foreach (var acc in accond)
            {
                var rolestr = acc.Split(';')[0].Split(',');
                var conditions = acc.Split(';')[1].Split(',');

                // parse organisation roles
                var roles = new List<GroupDao>();
                for (var i = 0; i < rolestr.Length / 2; i++)
                {
                    roles.Add(new GroupDao(rolestr[i*2], rolestr[i*2+1]));
                }

                result.Add(new AccessConditionDao { Groups = roles.ToArray(), Conditions = conditions});
            }

            return result.ToArray();
        }
    }



}
