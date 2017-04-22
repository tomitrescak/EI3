using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ei.Ontology;
using Ei.Runtime;
using NUnit.Framework;

namespace Ei.Tests.Unit
{
    [TestFixture]
    internal class AccessConditionsTests
    {
        [TestCase("w.WinnerName == a.Name", "get('w.WinnerName') == get('a.Name')")]
        [TestCase("x.y + 3", "get('x.y') + 3")]
        [TestCase("x.y.z + 3", "get('x.y.z') + 3")]
        [TestCase("x.y.z - a.b.c.d", "get('x.y.z') - get('a.b.c.d')")]
        [TestCase("x.y.z = 3", "set('x.y.z', 3)")]
        [TestCase("(more.me + 3)", "(get('more.me') + 3)")]
        [TestCase("me.test = me.test2 + you.test", "set('me.test', get('me.test2') + get('you.test'))")]
        //[TestCase("x.y[3] + 3", "get('x','y',3) + 3")]
        public void Access_ParseExpression_ReturnsCorrectExpression(string expression, string parsedExpression)
        {
            var pe = EiExpression.ParseExpression(null, expression);
            Assert.AreEqual(parsedExpression, pe);
        }
    }
}
