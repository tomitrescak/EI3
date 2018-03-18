using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ei.Logs;
using Vittoria.Helpers;
using System.Diagnostics;

namespace Vittoria.Controls
{
    /// <summary>
    /// Interaction logic for LogViewer.xaml
    /// </summary>
    public partial class LogViewer : UserControl, ILog, INotifyPropertyChanged
    {
        private string logSource;
        private Dictionary<string, ObservableCollection<LogEntry>> sources;
        private ObservableCollection<LogEntry> logEntries;

        public bool CollectAll { get; set; }

        public string[] IgnoreSources { get; set; }

        public ObservableCollection<LogEntry> LogEntries
        {
            get { return this.logEntries; }
            set
            {
                this.logEntries = value;
                this.OnPropertyChanged("LogEntries");
            }
        }

        public ObservableCollection<string> LogSources { get; set; }


        public LogViewer()
        {
            this.sources = new Dictionary<string, ObservableCollection<LogEntry>>();

            // init collection
            this.LogEntries = new ObservableCollection<LogEntry>();
            this.LogSources = new ObservableCollection<string>();
            this.LogSources.Add("Nothing");
            this.LogSources.Add("Planner");

            // init            
            Ei.Logs.Log.Register(this);
            //Ei.Logs.Log.Register(new ConsoleLog());

            InitializeComponent();
        }

        public void Log(ILogMessage message)
        {
            // return;

            if (this.LogEntries == null) {
                this.LogEntries = new ObservableCollection<LogEntry>();
            }

            if (Application.Current == null) return;

            if (!string.IsNullOrEmpty(message.Message) && 
                (message.Message.IndexOf("PLANNING") >= 0 || message.Message.IndexOf("REUSING") >= 0) &&
                message.Message.IndexOf("door") == -1 && 
                (message.Source == null || message.Source.ToLower().IndexOf("physio") == -1 && message.Source.IndexOf("Planner") == -1)) {
                Debug.WriteLine(message.Source + ": " + message.Message);
            }

            Application.Current.Dispatcher.BeginInvoke((Action) (() =>
            {
                if (CollectAll)
                {
                    if (message.Source == "Planner") return;

                    if (message.Source == null) message.Source = "NONE";
                    if (!this.sources.ContainsKey(message.Source))
                    {
                        this.sources.Add(message.Source, new ObservableCollection<LogEntry>());
                    }

                    // add it to the bucket

                    this.sources[message.Source].Add(
                        new LogEntry
                        {
                            DateTime = DateTime.Now,
                            Source = message.Source,
                            Message =
                                string.IsNullOrEmpty(message.Code)
                                    ? message.Message
                                    : I18N.Get(message.Code, message.Parameters)
                        }
                        );
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.logSource) && this.logSource != message.Source) return;
                    this.LogEntries.Insert(0, new LogEntry {
                        DateTime = DateTime.Now,
                        Source = message.Source,
                        Message =
                                string.IsNullOrEmpty(message.Code)
                                    ? message.Message
                                    : I18N.Get(message.Code, message.Parameters)
                    });
                }

                if (!string.IsNullOrEmpty(message.Source) && !this.LogSources.Contains(message.Source))
                {
                    this.LogSources.Add(message.Source);
                }

                if (this.LogEntries.Count > 100) {
                    this.LogEntries.RemoveAt(100);
                }
//
//                if (this.logSource == null || this.logSource != message.Source)
//                {
//                    return;
//                }
//
//                this.LogEntries.Insert(0, new LogEntry
//                {
//                    DateTime = DateTime.Now,
//                    Source = message.Source,
//                    Message =
//                        string.IsNullOrEmpty(message.Code)
//                            ? message.Message
//                            : I18N.Get(message.Code, message.Parameters)
//                });
            }));
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get the ComboBox.
            var comboBox = sender as ComboBox;
            Ei.Logs.Log.LogLevel = (Ei.Logs.Log.Level) comboBox.SelectedIndex;
        }

        private void ClearLog(object sender, RoutedEventArgs e)
        {
            this.LogEntries.Clear();
        }

        private void FilterLogSource(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            this.logSource = comboBox.SelectedValue.ToString();
            if (this.logSource == "Nothing")
            {
                this.logSource = null;
                this.LogEntries = null;
                return;
            }

            if (this.sources.ContainsKey(this.logSource))
            {
                this.LogEntries = this.sources[this.logSource];
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

        private void Pause(object sender, RoutedEventArgs e)
        {
            //var b = sender as Button;
            //Project.Current.Paused = !Project.Current.Paused;
            //b.Content = Project.Current.Paused ? "Play" : "Pause";

        }
    }
}
