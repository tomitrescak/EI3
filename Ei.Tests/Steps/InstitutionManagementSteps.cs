using Ei.Logs;
using Ei.Ontology;
using Moq;
using TechTalk.SpecFlow;

namespace Ei.Tests.Steps
{
    [Binding]
    public class InstitutionManagementSteps
    {

        [Then(@"'(.*)' is logged with parameter '(.*)'")]
        public void ThenIsLoggedWithParameter(string logCode, string logParam)
        {
            var logMock = ScenarioContext.Current.Get<Mock<ILog>>();
            logMock.Verify(w => w.Log(
                It.Is<ILogMessage>(
                    i => i.Code == logCode &&
                         i.Parameters[0].ToString() == logParam)));
        }


    }
}
