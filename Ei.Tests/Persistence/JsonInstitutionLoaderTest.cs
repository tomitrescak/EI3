using Ei.Persistence.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Ei.Tests.Persistence
{
    public class JsonInstitutionLoaderTest
    {
        [Fact]
        public void LoadFromString() {

        }

        [Fact]
        public void LoadFromFile() {
            var institution = JsonInstitutionLoader.Instance.LoadFromFile("Files/Fishing.json");

            Assert.Equal(2, institution.Workflows.Count);

            var w = institution.Workflows[1];
            Assert.Equal(7, w.Connections.Length);

            Assert.Equal("Fishing", institution.Name);
        }
        
        [Fact]
        public void LoadFromEiFile() {
            var institution = JsonInstitutionLoader.Instance.LoadFromFile("Files/Ei.json");

            Assert.Equal(2, institution.Workflows.Count);

            var w = institution.Workflows[1];
            Assert.Equal(7, w.Connections.Length);

            Assert.Equal("Fishing", institution.Name);
        }
    }
}
