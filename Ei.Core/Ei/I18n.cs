using System;
using System.Linq;
using Ei.Core.Properties;
using EiLog=Ei.Logs.Log;

namespace Ei.Core
{
    public static class I18N
    {
        public static string Get(string key, params object[] list)
        {
            
            var str = Resources.ResourceManager.GetString(key);
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
        
        public static string Get(Enum key, params object[] list)
        {
            return Get(key.ToString(), list);
        }
    }

    public static class Logger
    {
        public static void Info(string agent, string component, InstitutionCodes code, params object[] parameters) {
            if (EiLog.IsInfo) EiLog.Info(agent, component, I18N.Get(code, parameters));
        }
    }
}
