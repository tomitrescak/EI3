

using Ei.Core.Ontology;

namespace Ei.Core.Ontology.Transitions
{
  using System.Collections.Generic;
  using Core.Runtime;

  public class TransitionJoin : SimpleTransition
  {


    public TransitionJoin(string id, string name, string description, Workflow workflow) : base(id, name, description, workflow) {
    }

    

    protected override IActionInfo Perform(Governor performer) {
      var instance = this.GetInstance(performer.Workflow.InstanceId);

      // check if all agent clones are in the inout port of the join
      // if so, remove clones from the workflow and add the parent and flow it to the output port
      // if it was a deep clone, also consolidate the resources

      var inagents = new List<Governor>();
      foreach (var agent in instance.Agents) {
        if (agent.Parent == performer.Parent) {
          inagents.Add(agent);
        }
      }

      if (inagents.Count < performer.CloneCount) {
        return ActionInfo.Ok;
      }

      // remove all agents
      foreach (var clone in inagents) {
        performer.Workflow.RemoveAgent(clone);
      }

      // recover parent
      var parent = performer.Parent;

      // if it is a deep clone, consolidate
      parent.Join(inagents);

      // notify
      performer.NotifyJoin();

      // add parent back to workflow
      performer.Workflow.AddAgent(parent);

      // flow parent to out port
      this.Out.To.EnterAgent(parent);

      return ActionInfo.Ok;
    }
  }
}
