using Ei.Compilation;
using Ei.Ontology;
using Ei.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Ei.Tests.Persistence
{
    public class RoleDaoTest
    {
        [Fact]
        public void GenerateWithNoProperties() {
            var dao = new RoleDao {
                Description = "Description",
                Id = "default",
                Name = "Default"
            };

            var actual = dao.GenerateCode();

            Assert.Null(Compiler.Compile(actual));
        }


        [Fact]
        public void GenerateWithProperties() {
            var dao = new RoleDao {
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
            var result = Compiler.Compile(actual, "DefaultRole", out Role role);
            Assert.Null(result);

            Assert.Equal("Default", role.Name);
            Assert.Equal("default", role.Id);

            // test properties

            var store = role.Resources;

            Assert.Equal(0, store.GetValue("PublicParameter"));
            Assert.Equal("Tomi", store.GetValue("PrivateParameter"));

            
        }
    }
}
