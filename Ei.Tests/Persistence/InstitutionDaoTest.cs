using Ei.Compilation;
using Ei.Ontology;
using Ei.Persistence;
using System;
using System.Collections.Generic;
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
                            },
                              new GroupDao {
                                OrganisationId = "OId"
                            },
                        }
                    }
                },
                Expressions = new List<string> {
                    "i.Count = (int) (i.TimeMs + i.TimeSeconds)"
                }
            };

            var actual = dao.GenerateCode();

            Console.WriteLine(actual);
            Assert.Null(Compiler.Compile(actual));
        }
    }
}
