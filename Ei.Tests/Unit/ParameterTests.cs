using System;
using Ei.Ontology;
using Ei.Persistence;
using NUnit.Framework;

namespace Ei.Tests.Unit
{
    [TestFixture]
    class ParameterTests
    {
        Institution ei = new Institution();

        [TestCase("Par1", "string", "", true)]
        [TestCase("Par2", "string", "", false)]
        [TestCase("Par3", "int", 0, false)]
        [TestCase("Par4", "float", 3f, false)]
        [TestCase("Par6", "bool", true, false)]
        //[TestCase("Par7", "agent", null, false)]
        [TestCase("Par5", "date", "1/1/1970", false)]
        public void ParameterFactory_CorrectJsonWithoutAccessInfo_ParsesAndReturnsInstance(string name, string type, object defaultValue, bool optional)
        {
            var dao = CreateParameter(name, type, defaultValue, optional);        
            var param = new Parameter(ei, dao);
         
            Assert.AreEqual(name, param.Name);
            Assert.AreEqual(type, param.Type);
            Assert.AreEqual(type == "date" ? DateTime.Parse(defaultValue.ToString()) : defaultValue,  param.DefaultValue);
            Assert.AreEqual(optional, param.Optional);
        }

        [TestCase("Par1", "string[]", "a,b", new [] { "a", "b"})]
        [TestCase("Par3", "int[]", "0,1", new [] { 0, 1 })]
        [TestCase("Par4", "float[]", "3,4", new [] { 3f, 4f })]
        [TestCase("Par6", "bool[]", "true, false", new [] { true, false })]
        public void ParameterFactory_CorrecArrays_ParsesAndReturnsInstance(string name, string type, string parseValue, object defaultValue)
        {
            var dao = CreateParameter(name, type, parseValue, false);
            var param = new Parameter(ei, dao);

            Assert.AreEqual(name, param.Name);
            Assert.AreEqual(defaultValue, param.DefaultValue);
        }


        [TestCase("", "")]
        [TestCase("", "int")]
        [TestCase("Param", "")]
        // Punit, try to add more tes cases here ...
        public void ParameterFactory_IncorrectJSONMissingParameters_ThrowsFactoryException(string name, string type)
        {
            var dao = CreateParameter(name, type, true, true);

            Assert.Throws<ArgumentException>(() => new Parameter(ei, dao));
        }

        [TestCase("string", "", true)]
        [TestCase("string", 2, false)]
        [TestCase("int", 1, true)]
        [TestCase("int", "2", false)]
        [TestCase("float", 1f, true)]
        [TestCase("float", "2", false)]
        [TestCase("bool", true, true)]
        [TestCase("bool", "true", false)]
        // Punit, try to add more tes cases here ...
        public void ParameterFactory_CheckParameter_EvaluatesCorrectly(string type, object val, bool eva)
        {
            var dao = CreateParameter("Par", type, val, true);
            var param = new Parameter(ei, dao);

            Assert.AreEqual(param.Check(val), eva);
        }

        [TestCase("string", "", "", true)]
        [TestCase("string", 2, 2, false)]
        [TestCase("int", 1, 1, true)]
        [TestCase("int", "2", 2, true)]
        [TestCase("int", "2f", "", false)]
        [TestCase("float", 1f, 1f, true)]
        [TestCase("float", "1", 1f, true)]
        [TestCase("float", "2r", "", false)]
        [TestCase("bool", true, true, true)]
        [TestCase("bool", "true", true, true)]
        [TestCase("bool", "true1", true, false)]
        public void ParameterInstance_SetValueToParameter_EvaluatesCorrectly(string type, object val, object checkVal, bool eva)
        {
            var dao = CreateParameter("Par", type, "", true);
            var param = new Parameter(ei, dao).Instance();
            var arr = new[] {param};
            
            if (eva)
            {
                param.Value = val;
                arr[0] = param;
             
                Assert.AreEqual(checkVal, param.Value);
                Assert.AreEqual(checkVal, arr[0].Value);
            }
            else
            {
                Assert.Throws<FormatException>(() => param.Value = val);
            }          
        }

        private static ParameterDao CreateParameter(string name, string type, object defaultValue, bool optional)
        {
            return new ParameterDao
            {
                Name = name,
                Type = type,
                DefaultValue = defaultValue.ToString(),
                Optional = optional
            };
        }
    }
}
