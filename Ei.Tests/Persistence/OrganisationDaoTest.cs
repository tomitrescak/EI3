using System;using Ei.Compilation;using Ei.Persistence;using Xunit;namespace Ei.Tests.Persistence{    public class OrganisationDaoTest    {        [Fact]        public void Generate()        {            var dao = new OrganisationDao            {                Description = "Description",                Id = "default",                Name = "Default"            };            var code = dao.GenerateCode();            Assert.Equal(code, @"#region class DefaultOrganisationpublic class DefaultOrganisation : Organisation{    public DefaultOrganisation() : base(""default"") {        this.Name = ""Default"";    }    public class DefaultOrganisationResources : Ei.Runtime.ResourceState { }    protected override Ei.Runtime.ResourceState CreateState() {        return new DefaultOrganisationResources();    }}#endregion".Trim());            Assert.Null(Compiler.Compile(code));        }    }}