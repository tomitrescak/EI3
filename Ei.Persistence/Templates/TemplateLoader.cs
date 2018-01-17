using Ei.Persistence.Actions;
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
            // load helpers
            Handlebars.RegisterHelper("lowerCase", (writer, context, parameters) => {
                writer.WriteSafeString(parameters[0].ToString().ToLower()); //.WriteSafeString("<a href='" + context.url + "'>" + context.text + "</a>");
            });

            Handlebars.RegisterHelper("generateConstructor", (writer, context, parameters) => {
                writer.WriteSafeString(((ActionDao) parameters[0]).GenerateConstructor(parameters[1].ToString())); //.WriteSafeString("<a href='" + context.url + "'>" + context.text + "</a>");
            });

            Handlebars.RegisterHelper("generateParameters", (writer, context, parameters) => {
                writer.WriteSafeString(((ActionDao) parameters[0]).GenerateParameters(parameters[1].ToString())); //.WriteSafeString("<a href='" + context.url + "'>" + context.text + "</a>");
            });

            // preload partials
            LoadHelper("Resources");
            LoadHelper("ParentResources");
        }

        // templates
        public static Func<object, string> Organisation => LoadTemplate("Organisation");
        public static Func<object, string> Role => LoadTemplate("Role");
        public static Func<object, string> Institution => LoadTemplate("Institution");
        public static Func<object, string> Class => LoadTemplate("Class");
        public static Func<object, string> Workflow => LoadTemplate("Workflow");
        public static Func<object, string> Parameters => LoadTemplate("Parameters");
    }
}
