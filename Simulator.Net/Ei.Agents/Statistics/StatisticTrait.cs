using System;
using System.Collections.Generic;
using System.Linq;

namespace Ei.Simulation.Statistics
{
    public class DataPoint
    {
        public double X { get; }
        public double Y { get; }

        public DataPoint(double x, double y) {
            this.X = x;
            this.Y = y;
        }

        public override string ToString() {
            return string.Format("[{0}:{1}]", this.X, this.Y);
        }
    }
    public enum StatisticRelaxationStrategy
    {
        Min,
        Max,
        Average
    }

    public class StatisticTrait
    {
        protected List<DataPoint> newPoints;
        protected int relaxationPeriod;
        protected StatisticRelaxationStrategy relaxationStrategy;

        public string Name { get; set; }
        public string YAxisKey { get; set; }
        public string XAxisKey { get; set; }
        
        public List<DataPoint> EnvironmentDataPoints { get; }

        public StatisticTrait() {
            this.EnvironmentDataPoints = new List<DataPoint>();
        }

        public event Action<StatisticTrait, DataPoint> PointAdded;
        public event Action<StatisticTrait, List<DataPoint>> PointsReset;
 
        public void Relax() {
            if (this.newPoints != null) {
                if (this.relaxationStrategy != StatisticRelaxationStrategy.Average) return;

                for (var i = 1; i < this.newPoints.Count - 1; i++) {
                    this.newPoints[i] = new DataPoint(this.newPoints[i].X, (this.newPoints[i].Y + this.newPoints[i - 1].Y) / 2);
                }
                return;
            }

            // we know that the bottom line represents time, so we will recreate a new collection of points based on strategy
            newPoints = new List<DataPoint>();

            var maxValue = this.relaxationPeriod * 3600;

            var processing = new List<DataPoint>();

            foreach (var point in this.EnvironmentDataPoints) {
                if (point.X > maxValue) {
                    // calculate current
                    var x = maxValue;
                    double y = 0;

                    switch (this.relaxationStrategy) {
                        case StatisticRelaxationStrategy.Average:
                            y = processing.Average(w => w.Y);
                            break;
                        case StatisticRelaxationStrategy.Max:
                            y = processing.Max(w => w.Y);
                            break;
                        case StatisticRelaxationStrategy.Min:
                            y = processing.Min(w => w.Y);
                            break;
                    }

                    // eliminate small spikes

                    //                    if (newPoints.Count > 1)
                    //                    {
                    //                        var prev = newPoints[newPoints.Count - 1];
                    //                        if (prev.Y*2 < newPoints[newPoints.Count - 2].Y)
                    //                        {
                    //                            newPoints[newPoints.Count - 1] = new DataPoint(prev.X,
                    //                                (y + newPoints[newPoints.Count - 2].Y)/2);
                    //                        }
                    //                    }

                    newPoints.Add(new DataPoint(x, y));

                    // set new interval
                    maxValue += this.relaxationPeriod * 3600;

                    // clean processing interval
                    processing.Clear();
                }
                processing.Add(point);
            }

            this.OnPointsReset(newPoints);

        }

        public void Clear() {
            this.EnvironmentDataPoints.Clear();
        }

        public void Reset() {
            this.PointsReset?.Invoke(this, this.EnvironmentDataPoints);
            this.newPoints = null;
        }

        protected void OnPointsReset(List<DataPoint> points) {
            PointsReset?.Invoke(this, points);
        }
        
        protected void OnPointAdded(DataPoint point) {
            PointAdded?.Invoke(this, point);
        }
    }

    public class StatisticTrait<T> : StatisticTrait
    {
        private readonly Func<T, double> processData;
        
        
        // constructor

        public StatisticTrait(
            string name,
            int relaxationPeriod = 6,
            StatisticRelaxationStrategy relaxationStrategy = StatisticRelaxationStrategy.Average,
            double thickness = 3,
            LineStyle lineStyle = LineStyle.Solid): base()  {

            this.Name = name;
            this.relaxationStrategy = relaxationStrategy;
            this.relaxationPeriod = relaxationPeriod;

            base.OnPointsReset(this.EnvironmentDataPoints);
        }

        public StatisticTrait(
            string name, 
            Func<T, double> processData,
            int relaxationPeriod = 6,
            StatisticRelaxationStrategy relaxationStrategy = StatisticRelaxationStrategy.Average,
            double thickness = 3,
            LineStyle lineStyle = LineStyle.Solid) : this(name, relaxationPeriod, relaxationStrategy, thickness, lineStyle) {
            this.processData = processData;
        }

        

        // methods

        public virtual void Process(float x, T data) {
            var point = new DataPoint(x, this.processData(data));
            this.EnvironmentDataPoints.Add(point);
            
            base.OnPointAdded(point);
        }

        

        
    }
}
