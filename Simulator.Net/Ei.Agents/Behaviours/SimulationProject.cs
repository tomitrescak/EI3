using Ei.Compilation;
using Ei.Core.Ontology;
using Ei.Persistence.Json;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Ei.Simulation.Behaviours
{
    public class SimulationProject: MonoBehaviour
    {
        public string Organisation;
        public string Password;
        public string InstitutionSource;

        [JsonIgnore]
        public InstitutionManager Manager { get; private set; }

        [JsonIgnore]
        public Institution Ei { get; set; }

        public string ProjectDefinition;

        public void Init()
        {
            var timer = FindObjectOfType<SimulationTimer>();

            if (timer == null)
            {
                throw new Exception("You must add SimulationTimer to the simulation");
            }

            // compile institution
            if (this.InstitutionSource != null)
            {
                var institution = JsonInstitutionLoader.Instance.Load(this.InstitutionSource, null);
                var code = institution.GenerateAll();
                var result = Compiler.Compile(code, "DefaultInstitution", out Institution TestEi);

                this.Ei = TestEi;
            }

            // init institution
            this.Ei.Resources.Tick = (float)(86400f / timer.DayLengthInSeconds);

            
        }

        public void Start()
        {
            // start institution
            this.Manager = InstitutionManager.Launch(this.Ei);
            this.Ei = Manager.Ei;
            this.Ei.Start();
        }
    }

    // public void 
}
