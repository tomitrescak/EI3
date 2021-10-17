//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Threading;
//using System.Timers;
//using Ei.Logs;
//using Ei.Core.Ontology;
//using Ei.Core.Runtime.Planning.Environment;

//namespace Ei.Simulation.Simulator
//{
//    public abstract class Project : INotifyPropertyChanged
//    {
//        // fields

//        private string directory;
//        private bool paused;
        
//        // properties
        
//        public string Organisation { get; set; }
        
//        public string Password { get; set; }

//        public float MetersPerPixel { get; set; }


//        public InstitutionManager Manager { get; private set; }
        
//        public Institution Ei { get; private set; }


    
//        public bool Started { get; set; }

//        // methods
        
//        public void SetDirectory(string dir) {
//            this.directory = dir;
//        }
    
//        public void Start(Institution ei) {
//            // init institution
//            this.Ei = ei;
//            this.Ei.Resources.Tick = (float)(86400f / this.DayLengthInSeconds);
//            this.Waiter = new ManualResetEvent(true);
            

//            // start institution
//            Manager = (InstitutionManager)InstitutionManager.Launch(this.Ei);
//            Ei = Manager.Ei;
//            Ei.Start();
            
//        }

        
        
//    }
//}
