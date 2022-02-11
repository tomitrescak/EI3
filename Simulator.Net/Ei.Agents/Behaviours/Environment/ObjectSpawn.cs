using Ei.Simulation.Behaviours.Environment.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ei.Simulation.Behaviours.Environment
{
    public class ObjectSpawn : MonoBehaviour
    {
        private AgentEnvironment environment;
        private readonly System.Random rnd = new System.Random();

        public EnvironmentObjectSeed[] Elements { get; set; }

        [JsonIgnore]
        public int RandomX
        {
            get
            {
                return (int)(rnd.NextDouble() * (this.environment.Definition.Width - 50));
            }
        }

        [JsonIgnore]
        public int RandomY
        {
            get
            {
                return (int)(rnd.NextDouble() * (this.environment.Definition.Height - 50));
            }
        }

        public void Start()
        {
            this.environment = GetComponent<AgentEnvironment>();
            if (this.environment == null)
            {
                throw new Exception("Spawn needs to exists on the same game object as environment definition");
            }

            var data = new List<EnvironmentObject>();

            if (this.Elements != null)
            {
                foreach (var def in this.Elements)
                {
                    var id = def.NewId();
                    var agent = new GameObject(id);

                    agent.transform.position = new Vector3(this.RandomX, this.RandomY, 0);

                    // add simobject
                    var sim = agent.AddComponent<EnvironmentObject>();
                    sim.Icon = def.Image;
                    sim.Actions = def.Actions;
                    

                    // initialise actions

                    //foreach (var action in def.Actions)
                    //{
                    //    if (!this.environment.Actions.ContainsKey(action.Id))
                    //    {
                    //        this.environment.Actions.Add(action.Id, new List<EnvironmentData>());
                    //    }
                    //}

                    //// initialise objects


                    //for (var i = 0; i < def.Seed; i++)
                    //{
                    //    // initiate element at a random position
                    //    data.Add(new EnvironmentData(def, this.RandomX, this.RandomY));
                    //}
                }
            }


        }

    }
}
