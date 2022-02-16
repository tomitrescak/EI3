using Newtonsoft.Json;
using System.ComponentModel;

namespace UnityEngine
{
    public class Transform : MonoBehaviour
    {
        private Vector3 pos;

        public Transform() {
        }

        [Browsable(false)]
        public Vector3 position {
            get { return this.pos; }
            set {
                this.pos = value;
                this.OnPropertyChanged("X");
                this.OnPropertyChanged("Y");
            }
        }

        [JsonIgnore]
        public float X {
            get { return this.position.x; }
            set { this.position = new Vector3(value, this.position.y, this.position.z); }
        }

        [JsonIgnore]
        public float Y {
            get { return this.position.y; }
            set { this.position = new Vector3(this.position.x, value, this.position.z); }
        }
    }
}
