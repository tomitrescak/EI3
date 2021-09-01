﻿namespace Vittoria.Helpers
{
    using System.Linq;


    public static class I18N
    {
        public static string Get(string key, params object[] list) {

            string str = Properties.Resources.ResourceManager.GetString(key);
            if (string.IsNullOrEmpty(str)) {
                return "<" + key + "> " + string.Join(";", list.Select(w => w == null ? "null" : w.ToString()).ToArray());
            }

            if (list.Length > 0) {
                return string.Format(str, list);
            }
            return str;
        }
    }
}
