using System.Linq;

namespace Ei.Logs
{
    public class LogMessage : ILogMessage
    {
        public int Level { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string Code { get; set; }
        public string[] Parameters { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", this.Code, this.Parameters != null ? string.Join(";", this.Parameters) : "");
        }
    }
}
