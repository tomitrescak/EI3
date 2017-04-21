//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//
//namespace Ei
//{
//    public static class I18N
//    {
//        public static Dictionary<string, string> En = 
//            new Dictionary<string, string>
//                {
//                    { "ActionEnterExit", "Agent entered workflow" },
//                    { "ActionExit", "Agent exited workflow" }
//                };
//
//        public static Dictionary<string, string> CurrentDictionary = En;
//
//        public static string Get(string key, params object[] list)
//        {
//            if (!CurrentDictionary.ContainsKey(key))
//            {
//                return "<" + key + ">";
//            }
//
//            if (list.Length > 0)
//            {
//                return string.Format(CurrentDictionary[key], list);
//            }
//            return CurrentDictionary[key];
//        }
//    }
//}
