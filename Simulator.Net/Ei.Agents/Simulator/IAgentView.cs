using System;

namespace Ei.Simulation.Simulator
{
    public interface IAgentView
    {
        void MoveToLocation(double x, double y, double speedModifier);
        void AddProperty(AgentProperty agentProperty);
        void PropertyValueChanged();
        void RunAfter(Action action, float waitTimeInSeconds);
        void UpdateOnUi(Action action);
    }
}
