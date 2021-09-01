
using System.Collections.Generic;
using System.IO;
using Ei.Simulation.Core;
using Ei.Simulation.Physiology;
using Ei.Simulation.Physiology.Behaviours;
using UnityEngine;
using Vittoria.Behaviours;
using YamlDotNet.Serialization;

namespace Vittoria.Core
{
    public class Project 
    {
        public string ProjectPath { get; set; } 

        public List<GameObject> Agents { get; set; }

        public Project() {
            this.Agents = new List<GameObject>();
        }

        public static Project OpenProject(string path) {
            if (File.Exists(path)) {
                var source = File.ReadAllText(path);
                var project = Deserialise<Project>(source);
                return project;
            }
            return null;
        }

        public string ResolvePath(string path) {
            return Path.Combine(this.ProjectPath, "resources", path);
        }

        static T Deserialise<T>(string source) {
            var input = new StringReader(source);
            var deserializer = new Deserializer();

            deserializer.RegisterTagMapping("!spawn", typeof(Spawn));
            deserializer.RegisterTagMapping("!transform", typeof(Transform));
            deserializer.RegisterTagMapping("!linearNavigation", typeof(LinearNavigation));
            deserializer.RegisterTagMapping("!randomNavigation", typeof(RandomNavigation));
            deserializer.RegisterTagMapping("!renderer", typeof(WpfRenderer));
            deserializer.RegisterTagMapping("!eiproject", typeof(PhysiologyProjectRunner));


            return deserializer.Deserialize<T>(input);
        }

        //public void Start() {
        //    foreach (var agent in this.Agents) {
        //        agent.Start();
        //    }
        //}

        internal void Save() {
            var serializer = new Serializer(SerializationOptions.Roundtrip);

            var yaml = serializer.Serialize(this);
            File.WriteAllText(this.ProjectPath, yaml);
        }
    }
}
