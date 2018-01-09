namespace Ei.Persistence
{
    public class OrganisationDao : ParametricEntityDao
    {
        public string GenerateCode() {
            return @"

#region class {0}Organisation
public class {0}Organisation : Organisation
{{
    public DefaultOrganisation() : base(""default"") {{
        this.Name = ""Default"";
    }}

    public class {0}OrganisationResources : Ei.Runtime.ResourceState {{ }}

    protected override Ei.Runtime.ResourceState CreateState() {{
        return new {0}OrganisationResources();
    }}
}}
#endregion

".FormatCode(this.Name.ToId());
        }
    }
}
