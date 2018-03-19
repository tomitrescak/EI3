using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Ei.Logs;
using Ei.Ontology;
using Ei.Runtime.Planning.Environment;
using YamlDotNet.Serialization;
using System.ComponentModel;
using System.Timers;
using Ei.Agents.Core.Behaviours;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Vittoria.Behaviours
{
    public class EiProjectStarter : EiBehaviour
    {
        // properties

        public string InstitutionPath { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public List<AgentCount> AgentCounts { get; set; }

        public double DayLengthInSeconds { get; set; }
        public double MetersPerPixel { get; set; }

        public double AgentSpeed { get; set; }
        public double[] SpeedDiversity { get; set; }
        public double[] PhysiologyDiversity { get; set; }

        public double HungerTreshold { get; set; }
        public double FatigueTreshold { get; set; }
        public double ThirstThreshold { get; set; }

        public double KillThirstThreshold { get; set; }
        public double KillHungerThreshold { get; set; }

        public double RestSpeed { get; set; }

        public string Organisation { get; set; }
        public string Password { get; set; }

        [ExpandableObject]
        public AgentEnvironmentDefinition EnvironmentDefinition { get; set; }

        public bool Started { get; set; }

        // constructor

        public EiProjectStarter() {
            this.AgentCounts = new List<AgentCount>();
            this.EnvironmentDefinition = new AgentEnvironmentDefinition();
        }

        // methods

        void Start() {

        }
    }

    public class AgentCount
    {
        public string[][] Groups { get; set; }
        public int Count { get; set; }
        public string[] FreeTimeGoals { get; set; }
        public byte[] Color { get; set; }
    }
}
