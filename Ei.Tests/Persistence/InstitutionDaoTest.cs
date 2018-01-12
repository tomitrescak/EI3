﻿using Ei.Compilation;
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
                }
            };

            var actual = dao.GenerateCode();

            Console.WriteLine(actual);
            Assert.Null(Compiler.Compile(actual));
        }
    }
}