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
            };

            var child = new ClassDao {
                Id = "2",
                Name = "My Class",
                Description = "Description",
                Parent = "MyParentClass",
                Properties = new List<ParameterDao> {
                    new ParameterDao {
                        Name = "ChildParame",
                        Type = "int",
                        DefaultValue = "0"
                    }
                }
            };

            var actual = parent.GenerateCode() + "\n" + child.GenerateCode();

            Console.WriteLine(actual);
            Assert.Null(Compiler.Compile(actual));
        }
    }
}
