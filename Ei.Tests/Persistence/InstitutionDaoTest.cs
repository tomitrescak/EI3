﻿using Ei.Compilation;
using Ei.Ontology;
using Ei.Persistence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;

namespace Ei.Tests.Persistence
{
    public class InstitutionDaoTest
    {
        [Fact]
        public void GenerateWithNoProperties() {
            var dao = new InstitutionDao {
                Description = "Description",
                Id = "default",
                Name = "Default",
                Properties = new List<ParameterDao> {
                    new ParameterDao {
                        Name = "Count",
                        Type = "int",
                        DefaultValue = "0"
                    }
                },
                Authorisation = new List<AuthorisationDao> {
                    new AuthorisationDao {
                        User = "User",
                        Password = "Password",
                        Organisation = "Organisation",
                        Groups = new List<GroupDao> {
                            new GroupDao {
                                OrganisationId = "OId",
                                RoleId = "RId"
                            },
                            new GroupDao {
                                RoleId = "RId"
                            }
                        }
                    }
                },
                Expressions = new List<string> {
                    "i.Count = (int) (i.TimeMs + i.TimeSeconds)"
                },
                Workflows = new List<WorkflowDao> {
                    new WorkflowDao {
                        Id = "main",
                        Name = "Main",
                        States = new StateDao[] {
                            new StateDao {
                                Id = "start",
                                IsStart = true,
                                IsEnd = true
                            }
                        }
                    }
                },
                Organisations = new List<OrganisationDao> {
                    new OrganisationDao {
                        Id = "OId",
                        Name = "Default"
                    }
                },
                Roles = new List<RoleDao> {
                    new RoleDao {
                        Id = "RId",
                        Name = "Citizen"
                    }
                }

            };

            var actual = dao.GenerateAll();

            // Debug.WriteLine(actual);
            Console.WriteLine(actual);

            var result = Compiler.Compile(actual, "DefaultInstitution", out Institution TestEi);
            Assert.Null(result);
            

            var auth = TestEi.AuthenticationPermissions.Authenticate("", "User", "Password");
            Assert.False(auth.IsEmpty);
        }
    }
}
