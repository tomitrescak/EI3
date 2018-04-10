namespace Ei.Simulation.Statistics
{
    public enum LineStyle
    {
        Solid, 
        Dot,
        LongDash
    }
    public enum AxisPosition
    {
        Left,
        Bottom
    }  
    
    public class Axis
    {
        public string Key { get; set; }
        public float Maximum { get; set; }
        public float Minimum { get; set; }
        public AxisPosition Position { get; set; }
    }
}