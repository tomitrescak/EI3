using System.Linq;
using Ei.Ontology;
using Ei.Persistence;
using Ei.Runtime;
using NUnit.Framework;

namespace Ei.Tests.Unit
{
    [TestFixture]
    class OrganisationRoleTests
    {
        [TestCase("1","2")]
        public void OrganisationRole_Init_ReturnsInstance(string orgId, string roleId)
        {
            //var dao = new OrganisationRoleDao { OrganisationId = orgId, RoleId = roleId };

            var orgRole = new Group(new Organisation(orgId), new Role(roleId));

            Assert.AreEqual(orgRole.Organisation.Id, orgId);
            Assert.AreEqual(orgRole.Role.Id, roleId);
        }

        [Test]
        public void Role_CyclicParentDepndency_ThrowsError()
        {
            var role1 = new Role("1");
            var role2 = new Role("2");
            var role3 = new Role("3");

            role1.Parents = new[] { role2 };
            role2.Parents = new[] { role3 };

            Assert.Throws<InstitutionException>(() => role3.Parents = new[] {role1});
        }

        [Test]
        public void Role_InheritsParameters_ThrowsError()
        {
            var role1 = new Role("1");
            role1.ParameterDefinitions.Add(new Parameter(null, "Parent", "int", false, "0"));

            var role2 = new Role("2");
            role2.ParameterDefinitions.Add(new Parameter(null, "Child", "int", false, "0"));
            role2.Parents = new[] { role1 };

            Assert.IsTrue(role2.AllParameters.Any(w => w.Name == "Parent"));
        }
    }
}
