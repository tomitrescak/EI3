using Ei.Persistence.Templates;
using Newtonsoft.Json;

namespace Ei.Persistence
{
    public class StateDao : EntityDao
    {
        public bool IsOpen { get; set; }

        public bool IsStart { get; set; }

        public bool IsEnd { get; set; }

        [JsonIgnore]
        public bool HasEntryRules => this.EntryRules != null && this.EntryRules.Length > 0;
        [JsonIgnore]
        public bool HasExitRules => this.ExitRules != null && this.ExitRules.Length > 0;

        public AccessConditionDao[] EntryRules { get; set; }
 
        public AccessConditionDao[] ExitRules { get; set; }
        
        public int Timeout { get; set; }

        public override string GenerateCode() {
            //return result;
            return CodeGenerator.State(this);
        }
    }
}
