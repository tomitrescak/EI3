using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Diagnostics;
using System.Threading;
using Vittoria.Statistics;

namespace Vittoria.Controls
{
    /// <summary>
    /// Interaction logic for Statistics.xaml
    /// </summary>
    public partial class Statistics : UserControl
    {
        public PlotModel PlotModel { get; private set; }

        // this holds all available statistics
        public List<StatisticTrait> StatisticData { get; set; }

        public double Rate { get; set; }
        public TextBlock LoadText { get; set; }
        public ProgressBar LoadProgress { get; set; }

        private Func<BinaryReader, bool> OpenDataSet;
        private Func<BinaryWriter, bool> SaveDataSet;
        private List<Thread> processThreads;

        public Statistics()
        {
            this.Rate = -0.1;

            InitializeComponent();
            // add line styles

            this.Styles.ItemsSource = Enum.GetValues(typeof(LineStyle)).Cast<LineStyle>();
        }

        public void ProcessStatistics(IStatistic statistic)
        {
            // add given traits
            this.StatisticData.AddRange(statistic.Traits);

            // init axes
            foreach (var axis in statistic.Axes) {
                if (this.PlotModel.Axes.All(a => a.Key != axis.Key)) {
                    // add exis if it does not exists
                    this.PlotModel.Axes.Add(axis);
                }
            }

            // sort traits by name
            this.StatisticData.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));

            // init datacontext
            var thread = new Thread(() => {
                while (true) {
                    statistic.ProcessSamples();
                    this.PlotModel.InvalidatePlot(true);
                    Thread.Sleep(statistic.ProcessTimeInMilliseconds);
                }
            });
            thread.IsBackground = true;
            thread.Start();

            this.processThreads.Add(thread);
            this.DataContext = this;
        }


        public void StartSampling()
        {
            // init plotmodel
            this.PlotModel = new PlotModel();
            this.StatisticData = new List<StatisticTrait>();
            this.processThreads = new List<Thread>();
        }

        public void StopSampling() {
            if (this.processThreads == null) {
                return;
            }

            foreach (var thread in this.processThreads) {
                thread.Abort();
            }
        }

        // UI Methods

        private void AddTrait(object sender, RoutedEventArgs e)
        {
            var ch = sender as CheckBox;
            var trait = this.StatisticData.Find(w => w.Name == ch.Content.ToString());

            this.PlotModel.Series.Add(trait.Series);
            this.PlotModel.InvalidatePlot(true);
        }

        private void RemoveTrait(object sender, RoutedEventArgs e)
        {
            var ch = sender as CheckBox;
            var trait = this.StatisticData.Find(w => w.Name == ch.Content.ToString());

            this.PlotModel.Series.Remove(trait.Series);
            this.PlotModel.InvalidatePlot(true);
        }

        // helpers

        public static double Parse(object val)
        {
            if (val is int) return (int) val;
            if (val is long) return (long)val;
            if (val is float) return (float) val;
            if (val is double) return (double) val;

            throw new ApplicationException("We can only process double data");
        }

        public void LoadDataset(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var trait in this.StatisticData)
                {
                    trait.Clear();
                }

                new Thread(() =>
                {
                    // Read from disk
                    using (Stream stream = File.Open(openFileDialog.FileName, FileMode.Open))
                    {
                        BinaryReader br = new BinaryReader(stream);
                        var count = br.ReadInt32();
                        var current = 0;
                        var total = count*2;

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            this.LoadText.Text = string.Format("Opening dataset ({0}) records", count);
                            this.LoadProgress.Visibility = Visibility.Visible;
                        });

                        this.OpenDataSet(br);
                    }
                }).Start();


            }
        }

        public void SaveDataset(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                using (Stream stream = File.Open(saveFileDialog.FileName, FileMode.Create))
                {
                    BinaryWriter bw = new BinaryWriter(stream);
                    // Save number of entries
                    this.SaveDataSet(bw);
                }
            }
        }

        private void ExportPDF(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                using (var stream = File.Create(saveFileDialog.FileName))
                {
                    try
                    {
                        PdfExporter.Export(this.PlotModel, stream, 800, 300);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public void Relax(object sender, RoutedEventArgs e)
        {
            // remove every second point in the dataset
            int cutAmount;

            //if (int.TryParse(this.CutAmount.Text, out cutAmount))
            //{
                foreach (var trait in this.StatisticData)
                {
//                    for (var i = trait.EnvironmentDataPoints.Count - 1; i >= 0; i = i - cutAmount)
//                    {
//                        trait.EnvironmentDataPoints.RemoveAt(i);
//                    }
                    trait.Relax();
                }
            //}
            this.PlotModel.InvalidatePlot(true);
        }

//        public void CutLeft(object sender, RoutedEventArgs e)
//        {
//            int cutAmount;
//            
//            if (int.TryParse(this.CutAmount.Text, out cutAmount))
//            {
//                foreach (var trait in this.StatisticData)
//                {
//                    for (var i = 0; i < cutAmount; i++)
//                    {
//                        trait.EnvironmentDataPoints.RemoveAt(0);
//                    }
//                }
//            }
//            this.PlotModel.InvalidatePlot(true);
//        }
//
//        public void CutRight(object sender, RoutedEventArgs e)
//        {
//            int cutAmount;
//
//            if (int.TryParse(this.CutAmount.Text, out cutAmount))
//            {
//                foreach (var trait in this.StatisticData)
//                {
//                    for (var i = 0; i < cutAmount; i++)
//                    {
//                        trait.EnvironmentDataPoints.RemoveAt(trait.EnvironmentDataPoints.Count - 1);
//                    }
//                }
//            }
//            this.PlotModel.InvalidatePlot(true);
//        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            foreach (var trait in this.StatisticData)
            {

                trait.Reset();

            }
            this.PlotModel.InvalidatePlot(true);
        }

        private StatisticTrait selectedTrait;
        private void ApplyLineStyle(object sender, SelectionChangedEventArgs e)
        {
            if (this.selectedTrait != null)
            {
                this.selectedTrait.Series.LineStyle = (LineStyle) ((ComboBox) sender).SelectedValue;
            }
            this.PlotModel.InvalidatePlot(true);

        }

        private void SelectLine(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            var trait = cb.DataContext as StatisticTrait;

            this.selectedTrait = trait;
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.selectedTrait = ((ListBox) sender).SelectedValue as StatisticTrait;
        }

        private void ChangeFont(object sender, SelectionChangedEventArgs e)
        {
            var sel = (ComboBoxItem) ((ComboBox) sender).SelectedValue;

            if (sel==null || sel.Content == null) return;

            var val = int.Parse(sel.Content.ToString());
            this.PlotModel.DefaultFontSize = val;
            this.PlotModel.InvalidatePlot(true);
        }
    }
}
