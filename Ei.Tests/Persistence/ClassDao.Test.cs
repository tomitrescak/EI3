using Ei.Compilation;
using Ei.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Ei.Tests.Persistence
{
    public class ClassDaoTest
    {
        [Fact]
        public void TestClass() {
            var parent = new ClassDao {
                Id = "1",
                Name = "My Parent Class",
                Properties = new List<ParameterDao> {
                    new ParameterDao {
                        Name = "ParentParam",
                        Type = "int",
                        DefaultValue = "2"
                    }
                }
            };

            var child = new ClassDao {
                Id = "2",
                Name = "My Class",
                Description = "Description",
                Parent = "MyParentClass",
                Properties = new List<ParameterDao> {
                    new ParameterDao {
                        Name = "ChildParam",
                        Type = "int",
                        DefaultValue = "3"
                    }
                }
            };

            var actual = parent.GenerateCode() + "\n" + child.GenerateCode();

            // Console.WriteLine(actual);

            var result = Compiler.Compile(actual, "MyClass", out dynamic Activated);
            Assert.Null(result);
            Assert.Equal(Activated.ChildParam, 3);
            Assert.Equal(Activated.ParentParam, 2);

            
        }
    }
}
