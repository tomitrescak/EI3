using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Ei.Runtime.Planning;

namespace Ei.Simulator.Core
{
    public interface IAgentView
    {
        void MoveToLocation(double x, double y, double speedModifier);
        void Rest(double restingTimeInSeconds);
        void AddProperty(AgentProperty agentProperty);
        void PropertyValueChanged();
        void RunAfter(Action action, float waitTimeInSeconds);
        // void RemovePlanItem(List<AStarNode> plan, int p1);
        void UpdateOnUi(Action action);
        void Sleep();
        void WakeUp();
    }
}
