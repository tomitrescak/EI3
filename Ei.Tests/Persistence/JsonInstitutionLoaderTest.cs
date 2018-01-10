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
            var institution = JsonInstitutionLoader.Instance.LoadFromFile("Files/AllComponents.json");
            Assert.Equal("Institution", institution.Name);
        }
    }
}
