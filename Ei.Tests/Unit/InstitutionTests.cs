using System;

namespace Ei.Tests.Unit.Ontology
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Security.Authentication;

    using Ei.Data.Yaml;
    using Ei.Ontology;
    using Ei.Ontology.Activities;
    using Ei.Persistence;

    using NUnit.Framework;

    [TestFixture]
    class InstitutionTests
    {
 
        [Test]
        public void Instituion_WhenLoadedFromCorrectYaml_ReturnsNewInstance()
        {
            // load dao 
            var institutionDao = YamlInstitutionFactory.Instance.LoadFromFile("Institutions/AllComponents.yaml");

            var manager = InstitutionManager.Launch(institutionDao);
            var institution = manager.Ei;
            // load institution from dao

            this.CheckParametricEntities(institutionDao.Types, institution.Types);
//           this.CheckParametricEntities(institutionDao.Actions, institution.Actions);

            // check workflow

            for (var i = 0; i < institutionDao.Workflows.Count; i++)
            {
                var dao = institutionDao.Workflows[i];
                var entity = institution.CreateWorkflow(dao.Id, null);

                Assert.AreEqual(dao.Stateless, entity.Stateless);
                Assert.AreEqual(dao.Static, entity.Static);

                if (dao.Actions != null)
                {
                    for (var j = 0; j < dao.Actions.Length; j++)
                    {
                        var daoActivity = dao.Actions[j];
                        var entityActivity = entity.Actions[j];

                        // check basic properties

                        Assert.AreEqual(daoActivity.Id, entityActivity.Id);
//                        Assert.AreEqual(daoActivity.Name??entityActivity.GetType().Name , entityActivity.Name);
                        Assert.AreEqual(daoActivity.Description, entityActivity.Description);
                        
                    }
                } 

                // check all connections
                if (dao.Connections != null)
                {
                    foreach (var connection in dao.Connections)
                    {
                        var activityFromId = connection.Join[0];
                        var activityToId = connection.Join[1];

                        var positionFrom = entity.FindPosition(activityFromId);
                        var positionTo = entity.FindPosition(activityToId);

                        var conn = positionFrom.Outs.First(w => w.To == positionTo);

                        // check preconditions
                        if (connection.Allow != null)
                        {
                            for (var k = 0; k < connection.Allow.Length; k++)
                            {
                                this.CheckAccessCondition(connection.Allow, conn.Preconditions.Allow);
                            }
                        }

                        // check postconditions
                        if (connection.Postconditions != null)
                        {
                            for (var k = 0; k < connection.Postconditions.Length; k++)
                            {
                                this.CheckAccessCondition(connection.Postconditions, conn.Postconditions);
                            }
                        }                        
                    }
                }
            }

            // check the initial workflow
            Assert.AreEqual(institution.MainWorkflowId, institutionDao.InitialWorkflow);
        }

        [Test]
        public void Institution_AuthorisationValid_ReturnsTrue()
        {
            var institutionDao = YamlInstitutionFactory.Instance.LoadFromFile("Institutions/AllComponents.yaml");
            var ei = new Institution(institutionDao);

            for (var i = 0; i < institutionDao.Authorisation.Count; i++)
            {
                var dao = institutionDao.Authorisation[i];
                var user = ei.CreatePermissions.Authenticate(dao.Organisation, dao.User, dao.Password);
                var allowRoles = dao.Groups.Select(role => new Group(ei, role)).ToList();

                Assert.IsTrue(Security.Authorise(user, allowRoles));
            }
        }

        [Test]
        public void Institution_AuthorisationNotValid_ReturnsFalse()
        {
            var institutionDao = YamlInstitutionFactory.Instance.LoadFromFile("Institutions/AllComponents.yaml");
            var institution = new Institution(institutionDao);
             
            Assert.IsTrue(institution.CreatePermissions.Authenticate(null, "badUser", "").IsEmpty);
            Assert.IsTrue(institution.CreatePermissions.Authenticate(null, "user", "").IsEmpty);

            var auth = institution.CreatePermissions.Authenticate("bad_org", "user2", "pass");

            Assert.IsTrue(institution.CreatePermissions.Authenticate("bad_org", "user2", "pass").IsEmpty);

            var user = institution.CreatePermissions.Authenticate(null, "user", "pass");
            Assert.IsFalse(Security.Authorise(user,  new List<Group> { new Group(institution, "7", "1")}));

            var user2 = institution.CreatePermissions.Authenticate("org", "user_limited", "pass");
            Assert.IsFalse(Security.Authorise(user2, new List<Group> { new Group(institution, "1", "2") }));

        }

        // helper methods

        private void CheckParametricEntities(IReadOnlyList<ParametricEntityDao> daos, IReadOnlyList<ParametricEntity> entities)
        {
            if (daos == null)
            { 
                Assert.AreEqual(entities.Count, 0);
                return;
            }

            Assert.AreEqual(daos.Count, entities.Count);
            for (var i = 0; i < daos.Count; i++)
            {
                this.CheckParametricEntity(daos[i], entities[i]);
            }
        }

        private void CheckParametricEntity(ParametricEntityDao dao, ParametricEntity entity)
        {
            Assert.AreEqual(dao.Name, entity.Name);
            Assert.AreEqual(dao.Description, entity.Description);
            this.CheckParameters(dao.Properties, entity.ParameterDefinitions);          
        }

        private void CheckParameters(List<ParameterDao> dao, List<Parameter> param)
        {
            for (int i = 0; i < dao.Count; i++)
            {
                Assert.AreEqual(dao[i].Name, param[i].Name);
                Assert.AreEqual(dao[i].Optional, param[i].Optional);
                Assert.AreEqual(dao[i].Type, param[i].Type);
                Assert.AreEqual(param[i].Parse(dao[i].DefaultValue), param[i].DefaultValue);

                this.CheckAccess(dao[i].AllowAccess, dao[i].DenyAccess, param[i]);
            }
        }

        private void CheckAccess(AccessConditionDao[] allow, AccessConditionDao[] deny, Parameter param)
        {
            if (allow != null)
            {
                foreach (var accessConditionDao in allow)
                {
                    var allowRoles = accessConditionDao.Groups.Select(role => new Group(new Organisation(role.OrganisationId), new Role(role.RoleId))).ToList();
                    Assert.IsTrue(param.HasAccess(allowRoles, null));
                }
            } 

            if (deny != null)
            {
                foreach (var accessConditionDao in deny)
                {
                    var denyRoles = accessConditionDao.Groups.Select(role => new Group(new Organisation(role.OrganisationId), new Role(role.RoleId))).ToList();
                    Assert.IsFalse(param.HasAccess(denyRoles, null));
                }
            }
        }

        private void CheckAccessCondition(AccessConditionDao[] dao, IList<AccessCondition> entity)
        { 
            for (var i = 0; i < dao.Length; i++)
            {
                var daoCondition = dao[i];
                var entityCondition = entity[i];

                for (var k = 0; k < daoCondition.Groups.Length; k++)
                {
                    Assert.AreEqual(
                        daoCondition.Groups[k].OrganisationId, 
                        entityCondition.Groups[k].Organisation.Id);
                    Assert.AreEqual(
                        daoCondition.Groups[k].RoleId, 
                        entityCondition.Groups[k].Role.Id);
                }

                if (daoCondition.Conditions != null)
                {
                    for (var k = 0; k < daoCondition.Conditions.Length; k++)
                    {
                        Assert.AreEqual(
                            daoCondition.Conditions[k],
                            entityCondition.Conditions[k]);
                    }
                }
            }
        }



    }
}
