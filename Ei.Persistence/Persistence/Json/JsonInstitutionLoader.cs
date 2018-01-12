using Ei.Persistence.Actions;
using Ei.Persistence.Transitions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ei.Persistence.Json
{
    public class JsonInstitutionLoader : InstitutionLoader
    {
        // fields

        private static JsonInstitutionLoader instance;
        private KnownTypesBinder knownTypesBinder = new KnownTypesBinder {
            KnownTypes = new List<Type> {
                typeof(ActionJoinWorkflowDao),
                typeof(ActionMessageDao),
                typeof(ActionStartWorkflowDao),
                typeof(ActionStartAgentDao),
                typeof(ActionTimeoutDao),

                typeof(TransitionBinaryDecisionDao),
                typeof(TransitionSplitDao),
                typeof(TransitionJoinDao)
            }
        };

        // properties

        public static JsonInstitutionLoader Instance {
            get {
                if (instance == null) {
                    instance = new JsonInstitutionLoader();
                }
                return instance;
            }

        }


        // methods


        public override string SaveAccess(AccessConditionDao[] dao) {
            string json = JsonConvert.SerializeObject(dao, Formatting.Indented, new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.Objects,
                SerializationBinder = knownTypesBinder
            });
            return json;

        }

        public override T Deserialise<T>(string source) {
            T newValue = JsonConvert.DeserializeObject<T>(source, new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.Objects,
                MissingMemberHandling = MissingMemberHandling.Error,
                SerializationBinder = knownTypesBinder
            });
            return newValue;
        }
    }
}
