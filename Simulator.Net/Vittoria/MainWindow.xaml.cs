using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Vittoria.Properties;
using Ei.Agents.Core;
using Vittoria.Core;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Ei.Logs;
using System.Diagnostics;
using Vittoria.Controls;
using Vittoria.Behaviours;
using UnityEngine;
using Vittoria.Statistics;
using System.Windows.Data;
using Ei.Agents.Sims;
using Ei.Agents.Core.Behaviours;
using Ei.Agents.Planning;

namespace Vittoria
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
 
        private Border selectedAvatar;
        private GameObject selectedAgent;
        private static bool CanRender;
        // private DrawingCanvas canvas;

        public MainWindow() {
            // this.canvas = new DrawingCanvas();
 
            InitializeComponent();

            // init sources
            this.GameObjects = new CollectionViewSource();
            this.GameObjects.Filter += FilterGameObjects;

            this.LoadLastProject();

            // re-set the data context
            DataContext = this;

            // add the event listener
            CompositionTarget.Rendering += RenderFrame;
        }

        private void FilterGameObjects(object sender, FilterEventArgs e) {
            e.Accepted = false;
            if (this.FilterObjects.SelectedIndex == 0) {
                e.Accepted = true;
            } else if (this.FilterObjects.SelectedIndex == 1) {
                e.Accepted = ((GameObject)e.Item).GetComponent<Sim>() != null;
            } else if (this.FilterObjects.SelectedIndex == 2) {
                e.Accepted = ((GameObject)e.Item).GetComponent<EiAgent>() != null;
            } else if (this.FilterObjects.SelectedIndex == 3) {
                e.Accepted = ((GameObject)e.Item).GetComponent<SimObject>() != null;
            }
        }

        // properties

        public Project Project { get; set; }
        public Simulation Simulation { get; set; }
        public CollectionViewSource GameObjects { get; set; }

        public GameObject SelectedAgent {
            get { return this.selectedAgent; }
            set {
                this.selectedAgent = value;
                this.OnPropertyChanged("SelectedAgent");
            }
        }

        // methods

        private void LoadLastProject() {
            if (!string.IsNullOrEmpty(Settings.Default.LastProjectFile)) {
                this.LoadProject(Settings.Default.LastProjectFile);
            }
        }

        private void LoadProject(string path) {
            if (path.EndsWith(".yaml")) {
                this.Project = Vittoria.Core.Project.OpenProject(System.IO.Path.Combine(Environment.CurrentDirectory, Settings.Default.LastProjectFile));
            } else {
                MessageBox.Show("This project type is not implemented!");
            }
            this.Project.ProjectPath = path;

            this.Reset();
        }

        private int id;

        private void RenderFrame(object sender, EventArgs e) {
            if (CanRender) {
                this.ProcessFrame();
            }
        }

        private void Reset(object s, RoutedEventArgs e) {
            CanRender = false;

            this.LoadLastProject();
        }

        private void ChangeFilter(object s, RoutedEventArgs e) {
            if (this.GameObjects != null) {
                this.GameObjects.View.Refresh();
            }
        }

        private void Reset() {
            // clear current canvas
            this.Statistics.StopSampling();
            this.AgentCanvas.Children.Clear();

            // init simulation
            WriteableBitmap writeableBmp = BitmapFactory.New(1000, 800);
            ImageControl.Source = writeableBmp;

            this.Simulation = new Simulation(this.Project, this.AgentCanvas, writeableBmp);
            this.Simulation.Statistics = this.Statistics;

            this.GameObjects.Source = this.Simulation.Agents;
            this.GameObjects.View.Refresh();

            this.OnPropertyChanged("GameObjects");
        }

        private void Save(object s, RoutedEventArgs e) {
            this.Project.Save();
        }

        private void SaveAndRestart(object s, RoutedEventArgs e) {
            this.Project.Save();

            this.Reset(s, e);
            this.Start(s, e);
        }

        private void Start(object s, RoutedEventArgs e) {
            Time.Start();

            // start sampling
            this.Statistics.StartSampling();

            // start all agents 
            this.Simulation.Start();

            // refresh list
            this.OnPropertyChanged("Simulation");

            // start rendering
            CanRender = true;
        }

        private void ProcessFrame() {
            CanRender = false;
            this.SimulatedTime.Text = Time.time.ToString("0.0") + " sec";
            this.RealTime.Text = Time.Fps.ToString() + " FPS";

            // start all agents 
            this.Simulation.ProcessFrame();
            Time.FrameEnd();

            CanRender = true;

            // Debug.WriteLine($"Processed in {sw.ElapsedTicks / (float)TimeSpan.TicksPerSecond}");

        }

        private void SelectAgent(object sender, SelectionChangedEventArgs e) {
            var cmb = sender as ListBox;
            var value = cmb.SelectedValue as GameObject;

            if (this.SelectedAgent != null) {
                this.SelectedAgent.Selected = false;
            }
            this.SelectedAgent = value;
            if (this.SelectedAgent != null) {
                this.SelectedAgent.Selected = true;
            }

            if (this.SelectedAgent == null) {
                return;
            }

            this.ShowAgentProperties();
        }

        private void ShowAgentProperties() {
            // add new property controls
            var coll = new List<Object>();

            // UIElement[] a = new UIElement[this.PropertyStack.Children.Count];
            // this.PropertyStack.Children.CopyTo(a, 0);
            this.PropertyStack.Children.Clear();

            var agentProperties = new PropertyGrid();
            agentProperties.AutoGenerateProperties = true;
            agentProperties.ShowDescriptionByTooltip = false;
            agentProperties.ShowPreview = false;
            agentProperties.ShowSearchBox = false;
            agentProperties.ShowSortOptions = false;
            agentProperties.ShowSummary = false;
            agentProperties.SelectedObject = this.SelectedAgent;

            this.PropertyStack.Children.Add(agentProperties);

            // add all components
            foreach (var component in this.SelectedAgent.Components) {
                var componentProperties = new PropertyGrid();
                componentProperties.ShowDescriptionByTooltip = false;
                componentProperties.ShowPreview = false;
                componentProperties.ShowSearchBox = false;
                componentProperties.ShowSortOptions = false;
                componentProperties.ShowSummary = false;
                componentProperties.AutoGenerateProperties = true;
                componentProperties.SelectedObject = component;

                this.PropertyStack.Children.Add(componentProperties);
            }

            // omit render
            if (this.SelectedAgent.GetComponent<WpfRenderer>() == null) {
                return;
            }

            if (this.selectedAvatar != null) {
                this.selectedAvatar.BorderThickness = new Thickness(1);
                this.selectedAvatar.BorderBrush = new SolidColorBrush(Colors.Black);
                this.selectedAvatar.Width = 20;
                this.selectedAvatar.Height = 20;
            }

            this.selectedAvatar = this.SelectedAgent.GetComponent<WpfRenderer>().Avatar;
            this.selectedAvatar.Width = 40;
            this.selectedAvatar.Height = 35;
            this.selectedAvatar.BorderThickness = new Thickness(10);
            this.selectedAvatar.BorderBrush = new SolidColorBrush(Colors.OrangeRed);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CreateGameObject(object sender, RoutedEventArgs e) {
            var gameobject = this.Simulation.CreateInstance(new GameObject("Empty"));

            this.Project.Agents.Add(gameobject);
        }

        private void DeleteGameobject(object sender, RoutedEventArgs e) {
            if (this.SelectedAgent == null) {
                MessageBox.Show("Please select an agent");
                return;
            }
            this.Project.Agents.Remove(this.SelectedAgent);
            GameObject.Destroy(this.SelectedAgent);
        }

        private void CreateComponent(object sender, RoutedEventArgs e) {
            if (this.SelectedAgent == null) {
                MessageBox.Show("Please select an agent");
                return;
            }
            var item = sender as MenuItem;
            MonoBehaviour behaviour = null;
            switch (item.Tag.ToString()) {
                case "Spawn":
                    behaviour = this.SelectedAgent.AddComponent<Spawn>();
                    break;
                case "EiProject":
                    behaviour = this.SelectedAgent.AddComponent<EiProject>();
                    break;
                case "EiProjectStarter":
                    behaviour = this.SelectedAgent.AddComponent<EiProjectStarter>();
                    break;
                case "LinearNavigation":
                    behaviour = this.SelectedAgent.AddComponent<LinearNavigation>();
                    break;
                case "RandomNavigation":
                    behaviour = this.SelectedAgent.AddComponent<RandomNavigation>();
                    break;
                case "Sim":
                    behaviour = this.SelectedAgent.AddComponent<Sim>();
                    break;
                case "SimObject":
                    behaviour = this.SelectedAgent.AddComponent<SimObject>();
                    break;
                case "FastRenderer":
                    behaviour = this.SelectedAgent.AddComponent<FastRenderer>();
                    break;
                case "SimRenderer":
                    behaviour = this.SelectedAgent.AddComponent<SimRenderer>();
                    break;
                case "IconRenderer":
                    behaviour = this.SelectedAgent.AddComponent<IconRenderer>();
                    break;


            }

            // init this behaviour
            if (behaviour != null && behaviour.InitAction != null) {
                behaviour.InitAction();
            }
            // update gameobject's components
            behaviour.gameObject.InitComponent(behaviour, this.Simulation);
            behaviour.gameObject.UpdateComponents(behaviour);
            

            this.ShowAgentProperties();

        }
    }
}
