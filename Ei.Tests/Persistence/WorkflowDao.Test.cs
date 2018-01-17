using Ei.Compilation;
using Ei.Persistence;
using Ei.Persistence.Actions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Ei.Tests.Persistence
{
    public class WorkflowDaoTest
    {
        [Fact]
        public void GenerateWorkflow() {
            var workflow = new WorkflowDao {
                Id = "main",
                Name = "Main",
                Stateless = true,
                Static = true,
                Properties = new List<ParameterDao> {
                    new ParameterDao {
                        Name = "WParam",
                        Type = "int"
                    }
                },
                Actions = new ActionDao[] {
                    new ActionJoinWorkflowDao {
                        Id = "join",
                        WorkflowId = "wid",
                        Properties = new List<ParameterDao> {
                            new ParameterDao {
                                Name = "WParam",
                                Type = "int"
                            }
                        }
                    },
                    new ActionMessageDao {
                        Id = "message",
                        Validations = new [] { "Param > 0" },
                        Properties = new List<ParameterDao> {
                            new ParameterDao {
                                Name = "Param",
                                Type = "int"
                            }
                        }
                    },
                    new ActionTimeoutDao {
                        Id = "timeout"
                    }
                }
            };

            var actual = workflow.GenerateCode();
            Console.WriteLine(actual);

            var result = Compiler.Compile(actual, "MainWorkflow", out dynamic Activated);
            Assert.Null(result);
            Assert.Equal(Activated.ChildParam, 3);
            Assert.Equal(Activated.ParentParam, 2);


        }
    }
}
