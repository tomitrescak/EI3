using Newtonsoft.Json;
using System.Linq;

namespace Ei.Persistence
{
    public class ConnectionDao
    {
        public string Id { get; set; }

        public string[] Join { get; set; }

        public AccessConditionDao[] Access { get; set; }

        public AccessConditionDao[] Effects { get; set; } 

        public string ActionId { get; set; }

        public int AllowLoops { get; set; }


        // helpers

        [JsonIgnore] 
        public string AssignedActionId {
            get {
                InstitutionDao.Instance.CurrentAction =
                    this.ActionId == null
                        ? "ParameterState"
                        : InstitutionDao.Instance.CurrentWorkflow.Actions.First(a => a.Id == this.ActionId).ParameterClassName;
                return this.ActionId;
            }
        }

        // public BacktrackDao Backtrack { get; set; }

        // [JsonIgnore]
        // public AccessConditionDao[] GeneratedNestedEffects { get; set; }

        public override string ToString()
        {
            if (Join.Length == 2)
            {
                return string.Format("{0} -> {1}", Join[0], Join[1]);
            }
            return string.Format("{0} [{1}] -> {2} [{3}]", Join[0], Join[1], Join[2], Join[3]);
        }
    }
}
