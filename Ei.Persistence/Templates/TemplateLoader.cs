using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Ei.Persistence.Templates
{
    public static class CodeGenerator
    {
        private static Dictionary<string, Func<object, string>> Templates = new Dictionary<string, Func<object, string>>();

        static string LoadFile(string name) {
            return File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Templates/" + name + ".mustache"));
        }

        static Func<object, string> LoadTemplate(string name) {
            if (!Templates.ContainsKey("name")) { 
                var source = LoadFile(name);
                var template = Handlebars.Compile(source);
                Templates[name] = template;
            }
            return Templates[name];
        }

        static void LoadHelper(string name) {
            var source = LoadFile(name);
            Handlebars.RegisterTemplate(name, source);

        }

        static CodeGenerator() {
            // preload partials
            LoadHelper("Resources");
            LoadHelper("ParentResources");
        }

        // templates
        public static Func<object, string> Organisation => LoadTemplate("Organisation");
        public static Func<object, string> Role => LoadTemplate("Role");
        public static Func<object, string> Institution => LoadTemplate("Institution");
    }
}
