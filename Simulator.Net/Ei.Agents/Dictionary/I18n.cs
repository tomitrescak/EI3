using System.Linq;
using System.Reflection;
using System.Resources;

namespace Ei
{
    public static class I18N
    {
        public static string Get(string key, params object[] list)
        {
            
            var str = Ei.Agents.Properties.Resources.ResourceManager.GetString(key);
            if (string.IsNullOrEmpty(str))
            {
                return "<" + key + "> " + string.Join(";", list.Select(w => w.ToString()).ToArray());
            }

            if (list.Length > 0)
            {
                return string.Format(str, list);
            }
            return str;
        }
    }
}
