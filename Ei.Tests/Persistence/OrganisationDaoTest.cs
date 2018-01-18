using System;
using System.Collections.Generic;
using Ei.Compilation;
using Ei.Ontology;
using Ei.Persistence;
using Xunit;

namespace Ei.Tests.Persistence
{
    public class OrganisationDaoTest
    {
        [Fact]
        public void GenerateWithNoProperties() {
            var dao = new OrganisationDao {
                Description = "Description",
                Id = "default",
                Name = "Default"
            };

            var actual = dao.GenerateCode();

            Assert.Null(Compiler.Compile(actual));
        }


        [Fact]
        public void GenerateWithProperties() {
            var dao = new OrganisationDao {
                Description = "Description",
                Id = "default",
                Name = "Default",
                Properties = new List<ParameterDao> {
                new ParameterDao {
                    AccessType = VariableAccess.Public,
                    Name = "PublicParameter",
                    DefaultValue = "0",
                    Type = "int"
                },
                new ParameterDao {
                    AccessType = VariableAccess.Private,
                    Name = "PrivateParameter",
                    DefaultValue = "\"Tomi\"",
                    Type = "string"
                }
            }
            };

            var actual = dao.GenerateCode();
            Console.WriteLine(actual);
            var result = Compiler.Compile(actual, "DefaultOrganisation", out Organisation organisation);

            Assert.Equal("Default", organisation.Name);
            Assert.Equal("default", organisation.Id);

            // test properties

            var store = organisation.Resources;

            Assert.Equal(0, store.GetValue("PublicParameter"));
            Assert.Equal("Tomi", store.GetValue("PrivateParameter"));
            Assert.Single(store.FilterByAccess(null));
            Assert.Equal("PublicParameter", store.FilterByAccess(null)[0].Name);
            Assert.NotEqual(store, store.Clone());

            var type = store.GetType();
           
            var goalState = store.ToGoalState();
            Assert.Empty(goalState);

            // change object
            store.SetValue("PublicParameter", 4);
            goalState = store.ToGoalState();
            Assert.Single(goalState);
            Assert.Equal("PublicParameter", goalState[0].Name);
            Assert.Equal(4, goalState[0].Value);

            store.ResetDirty();
            goalState = store.ToGoalState();
            Assert.Empty(goalState);


            Assert.Null(result);
        }

        [Fact]
        public void GenerateWithInheritance() {
            var parentDao = new OrganisationDao {
                Description = "Parent",
                Id = "parent",
                Name = "Parent",
                Properties = new List<ParameterDao> {
                    new ParameterDao {
                        AccessType = VariableAccess.Public,
                        Name = "ParentParameter",
                        DefaultValue = "2",
                        Type = "int"
                    },
                    new ParameterDao {
                        AccessType = VariableAccess.Private,
                        Name = "ParentPrivateParameter",
                        DefaultValue = "\"Tomi\"",
                        Type = "string"
                    }
                }
            };

            var childDao = new OrganisationDao {
                Description = "Child",
                Id = "child",
                Name = "Child",
                Parent = "parent",
                Properties = new List<ParameterDao> {
                    new ParameterDao {
                        AccessType = VariableAccess.Public,
                        Name = "ChildParameter",
                        DefaultValue = "5",
                        Type = "int"
                    }
                }
            };

            var institution = new InstitutionDao {
                Organisations = new List<OrganisationDao> { parentDao, childDao }
            };



            var actual = childDao.GenerateCode() + "\n" + parentDao.GenerateCode();

            Console.WriteLine(actual);
            var result = Compiler.Compile(actual, "ChildOrganisation", out Organisation organisation);

            Assert.Null(result);

            Assert.Equal("Child", organisation.Name);
            Assert.Equal("child", organisation.Id);

            // test properties

            var store = organisation.Resources;

            Assert.Equal(2, store.GetValue("ParentParameter"));
            Assert.Equal(5, store.GetValue("ChildParameter"));
            Assert.Equal("Tomi", store.GetValue("ParentPrivateParameter"));
            Assert.Equal(2, store.FilterByAccess(null).Count);
            Assert.Equal("ParentParameter", store.FilterByAccess(null)[0].Name);
            Assert.Equal("ChildParameter", store.FilterByAccess(null)[1].Name);

            // test cloning
            var clone = store.Clone();
            Assert.NotEqual(store, clone);
            Assert.Equal(2, clone.GetValue("ParentParameter"));
            Assert.Equal(5, clone.GetValue("ChildParameter"));

            // check goal state

            var goalState = store.ToGoalState();
            Assert.Empty(goalState);

            // change object
            store.SetValue("ParentParameter", 4);
            goalState = store.ToGoalState();
            Assert.Single(goalState);
            Assert.Equal("ParentParameter", goalState[0].Name);
            Assert.Equal(4, goalState[0].Value);

            store.ResetDirty();
            goalState = store.ToGoalState();
            Assert.Empty(goalState);


            
        }
    }
}
