using Ei.Compilation;
using Ei.Persistence;
using Ei.Persistence.Actions;
using Ei.Persistence.Transitions;
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

            var organisation = new OrganisationDao {
                Id = "o",
                Name = "Default",
                Properties = new List<ParameterDao> {
                    new ParameterDao {
                        Name = "Op",
                        Type = "int",
                        DefaultValue = "0"
                    }
                }
            };

            var role = new RoleDao {
                Id = "r",
                Name = "Citizen",
                Properties = new List<ParameterDao> {
                    new ParameterDao {
                        Name = "Rp",
                        Type = "int",
                        DefaultValue = "0"
                    }
                }
            };

            var ei = new InstitutionDao {
                Name = "Ei",
                Organisations = new List<OrganisationDao> {
                    organisation
                },
                Roles = new List<RoleDao> {
                    role
                }
            };

            var postconditionOnly = new AccessConditionDao {
                Organisation = "o",
                Role = "r",
                Postconditions = new PostconditionDao[] {
                        new PostconditionDao {
                            Action = "r.Rp = o.Op + 1"
                        }
                    }
            };

            var withPreconditon = new AccessConditionDao {
                Organisation = "o",
                Role = "r",
                Precondition = "o.Op == 1",
                Postconditions = new PostconditionDao[] {
                    new PostconditionDao {
                        Condition = "r.Rp > 0",
                        Action = "r.Rp = o.Op + 1"
                    }
                }
            };

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
                },
                States = new StateDao[] {
                   new StateDao {
                       Id = "s1",
                       Name = "state1",
                       IsEnd = true,
                       IsStart = true,
                       IsOpen = true,
                       Timeout = 20,
                       EntryRules = new AccessConditionDao[] {
                           withPreconditon,
                           postconditionOnly
                       },
                       Description = "Description"
                   },
                   new StateDao {
                       Id = "s2",
                       ExitRules = new AccessConditionDao[] {
                           postconditionOnly
                       },
                   }
                },
                Transitions = new TransitionDao[] {
                    //new TransitionBinaryDecisionDao {
                    //    Id = "binary",
                    //    Name = "Binary",
                    //    Decision = new AccessConditionDao[] {
                    //        new AccessConditionDao {
                    //            Organisation = "o",
                    //            Role = "r",
                    //            Postconditions = new PostconditionDao[] {
                    //               new PostconditionDao {
                    //                   Action = "r.Rp = o.Op + 1"
                    //               }
                    //           }
                    //        }
                    //    }
                    //},
                    new TransitionJoinDao {
                        Id = "join"
                    },
                    new TransitionSplitDao {
                        Id = "split",
                        Shallow = true,
                        Name = "Split",
                        Names = new string[][] {
                            new string[] { "s1", "Left" },
                            new string[] { "s2", "Right" }
                        }
                    }
                },
                Connections = new ConnectionDao[] {
                    new ConnectionDao {
                        Id = "conn3",
                        AllowLoops = 3,
                        Join = new string[] { "s1", "s2" },
                        Access = new AccessConditionDao[] {
                           postconditionOnly
                        },
                        Effects = new AccessConditionDao[] {
                           postconditionOnly,
                           postconditionOnly
                        },
                    },
                    new ConnectionDao {
                        Id = "conn2",
                        AllowLoops = 0,
                        Join = new string[] { "s1", "s2" },
                        ActionId = "message",
                        Access = new AccessConditionDao[] {
                            new AccessConditionDao {
                                Organisation = "o",
                                Role = "r",
                                Postconditions = new PostconditionDao[] {
                                    new PostconditionDao {
                                        Action = "r.Rp = a.Param + 1"
                                    }
                                }
                            }
                        }

                    },

                },
                AllowCreate = new AccessConditionDao[] {
                    withPreconditon
                },
                AllowJoin = new AccessConditionDao[] {
                    withPreconditon
                }
            };

            var actual = workflow.GenerateCode();
            var full = actual + "\n" + role.GenerateCode() + "\n" + organisation.GenerateCode() + "\n" + ei.GenerateCode();
            Console.WriteLine(actual);

            var result = Compiler.Compile(full, "MainWorkflow", out dynamic Activated);
            Assert.Null(result);
        }
    }
}
