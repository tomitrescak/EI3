using System;
using System.Collections.Generic;
using Ei.Data.Yaml;
using Ei.Persistence;
using Ei.Persistence.Actions;
using NUnit.Framework;

namespace Ei.Tests.Unit.Data
{
    [TestFixture]
    public class YamlInstitutionFactoryTests
    {

        [TestCase]
        public void Load_EmptyJson_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => YamlInstitutionFactory.Instance.Load(null, null));
        }

        [TestCase]
        public void Load_OkJsonNoComponents_CreatesInstance()
        {
            var source = @"Name: Ins
Description: Des
Types: []
Roles: []
RoleRelations: []
Organisations: [] 
OrganisationRelations: []
Workflows: []
";
            var institution = YamlInstitutionFactory.Instance.Load(source, null);
            Assert.IsNotNull(institution);
            Assert.AreEqual(institution.Name, "Ins");
            Assert.AreEqual(institution.Description, "Des");
            Assert.AreEqual(institution.Types.Count, 0);
            Assert.AreEqual(institution.Roles.Count, 0);
            //           Assert.AreEqual(institution.Actions.Count, 0);
            Assert.AreEqual(institution.Workflows.Count, 0);
            Assert.AreEqual(institution.InitialWorkflow, null);
        }

        [Test]
        public void LoadAccessDao_GoodJson_ReturnsInstance()
        {
            var yaml = @"
  - Groups:
    - OrganisationId: 1
      RoleId: 2
    - OrganisationId: 2
      RoleId: 3
    Conditions: 
      - cond1
      - cond2
  - Groups:
    - OrganisationId: 4
      RoleId: 5
  - Conditions:
    - a";

            AccessConditionDao[] p = YamlInstitutionFactory.Instance.LoadAccess(yaml);

            Assert.AreEqual(3, p.Length);
            Assert.AreEqual(2, p[0].Groups.Length);
            Assert.AreEqual("1", p[0].Groups[0].OrganisationId);
            Assert.AreEqual("2", p[0].Groups[0].RoleId);
            Assert.AreEqual(2, p[0].Conditions.Length);
            Assert.IsNull(p[1].Conditions);
            Assert.AreEqual(1, p[1].Groups.Length);
            Assert.IsNull(p[2].Groups);
            Assert.AreEqual(1, p[2].Conditions.Length);


            var list = new List<ActionDao>();
            list.Add(new ActionDao() {Id = "1"});
            list.Add(new ActionJoinWorkflowDao {WorkflowId = "2"});
            //list.Add(new ActionMessageDao {ActionId = "4"});

            //var m = YamlInstitutionFactory.Instance.SaveArcs(list);
        }
    }
}
