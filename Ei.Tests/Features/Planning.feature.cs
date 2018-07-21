﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.9.0.77
//      SpecFlow Generator Version:1.9.0.0
//      Runtime Version:4.0.30319.42000
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Ei.Tests.Features
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Planning")]
    public partial class PlanningFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Planning.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Planning", "Agents can plan their actions depending on the final action", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.TestFixtureTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Create plan for action name")]
        [NUnit.Framework.CategoryAttribute("planning")]
        public virtual void CreatePlanForActionName()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Create plan for action name", new string[] {
                        "planning"});
#line 5
this.ScenarioSetup(scenarioInfo);
#line 6
 testRunner.Given("That institution \'OpenInstitution\' is launched", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 7
 testRunner.When("Agent \'test\' connects to organisation \'citizens\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 8
 testRunner.And("Agent \'test\' requests plan to perform \'bar\' with strategy \'ForwardSearch\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 9
 testRunner.Then("Plan length is \'3\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Plan action \"makeFire\"")]
        [NUnit.Framework.CategoryAttribute("planning")]
        public virtual void PlanActionMakeFire()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Plan action \"makeFire\"", new string[] {
                        "planning"});
#line 12
this.ScenarioSetup(scenarioInfo);
#line 13
 testRunner.Given("That institution \'Planning_MakeFire\' is launched", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 14
 testRunner.When("Agent \'test\' connects to organisation \'citizens\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 15
 testRunner.And("Agent \'test\' requests plan to perform \'makeFire\' with strategy \'ForwardSearch\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 16
 testRunner.Then("Plan length is \'4\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Binary plan action \"makeFire\"")]
        [NUnit.Framework.CategoryAttribute("planning")]
        public virtual void BinaryPlanActionMakeFire()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Binary plan action \"makeFire\"", new string[] {
                        "planning"});
#line 19
this.ScenarioSetup(scenarioInfo);
#line 20
 testRunner.Given("That institution \'Planning_MakeFire\' is launched", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 21
 testRunner.When("Agent \'test\' connects to organisation \'citizens\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 22
 testRunner.And("Agent \'test\' requests plan to perform \'makeFire\' with strategy \'ForwardSearchWith" +
                    "BinaryPredicates\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 23
 testRunner.Then("Plan length is \'4\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Plan to have resource \"a.hasFire\"")]
        [NUnit.Framework.CategoryAttribute("planning")]
        public virtual void PlanToHaveResourceA_HasFire()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Plan to have resource \"a.hasFire\"", new string[] {
                        "planning"});
#line 26
this.ScenarioSetup(scenarioInfo);
#line 27
 testRunner.Given("That institution \'Planning_MakeFire\' is launched", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 28
 testRunner.When("Agent \'test\' connects to organisation \'citizens\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 29
 testRunner.And("Agent \'test\' plans to change state to \'a.hasFire=true|a.hasFlint=true|a.hasWood=t" +
                    "rue\' with strategy \'ForwardSearch\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 30
 testRunner.Then("Plan length is \'4\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Backtrack plan for action \"makeFire\"")]
        [NUnit.Framework.CategoryAttribute("planning")]
        public virtual void BacktrackPlanForActionMakeFire()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Backtrack plan for action \"makeFire\"", new string[] {
                        "planning"});
#line 33
this.ScenarioSetup(scenarioInfo);
#line 34
 testRunner.Given("That institution \'Planning_MakeFire\' is launched", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 35
 testRunner.When("Agent \'test\' connects to organisation \'citizens\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 36
 testRunner.And("Agent \'test\' backward plans action \'makeFire\' with strategy \'BackwardSearch\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 37
 testRunner.Then("Plan length is \'3\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Backtrack plan to have resource \"a.hasFire\"")]
        [NUnit.Framework.CategoryAttribute("planning")]
        public virtual void BacktrackPlanToHaveResourceA_HasFire()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Backtrack plan to have resource \"a.hasFire\"", new string[] {
                        "planning"});
#line 40
this.ScenarioSetup(scenarioInfo);
#line 41
 testRunner.Given("That institution \'Planning_MakeFire\' is launched", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 42
 testRunner.When("Agent \'test\' connects to organisation \'citizens\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 43
 testRunner.And("Agent \'test\' backward plans to change state to \'a.hasFire=true\' with strategy \'Ba" +
                    "ckwardSearch\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 44
 testRunner.Then("Plan length is \'3\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Plan action with cycles \"makeFire\"")]
        [NUnit.Framework.CategoryAttribute("planning")]
        [NUnit.Framework.CategoryAttribute("cycles")]
        public virtual void PlanActionWithCyclesMakeFire()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Plan action with cycles \"makeFire\"", new string[] {
                        "planning",
                        "cycles"});
#line 47
this.ScenarioSetup(scenarioInfo);
#line 48
 testRunner.Given("That institution \'Planning_Cycles\' is launched", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 49
 testRunner.When("Agent \'test\' connects to organisation \'citizens\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 50
 testRunner.And("Agent \'test\' requests plan to perform \'makeFire\' with strategy \'ForwardSearch\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 51
 testRunner.Then("Plan length is \'8\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Plan to have resource \"a.hasFire\" with cycles")]
        [NUnit.Framework.CategoryAttribute("planning")]
        [NUnit.Framework.CategoryAttribute("cycles")]
        public virtual void PlanToHaveResourceA_HasFireWithCycles()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Plan to have resource \"a.hasFire\" with cycles", new string[] {
                        "planning",
                        "cycles"});
#line 54
this.ScenarioSetup(scenarioInfo);
#line 55
 testRunner.Given("That institution \'Planning_Cycles\' is launched", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 56
 testRunner.When("Agent \'test\' connects to organisation \'citizens\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 57
 testRunner.And("Agent \'test\' plans to change state to \'a.hasFire=true\' with strategy \'ForwardSear" +
                    "ch\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 58
 testRunner.Then("Plan length is \'8\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Backtrack cyclic plan to have resource \"a.hasFire\" with cycles")]
        [NUnit.Framework.CategoryAttribute("planning")]
        [NUnit.Framework.CategoryAttribute("cycles")]
        public virtual void BacktrackCyclicPlanToHaveResourceA_HasFireWithCycles()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Backtrack cyclic plan to have resource \"a.hasFire\" with cycles", new string[] {
                        "planning",
                        "cycles"});
#line 61
this.ScenarioSetup(scenarioInfo);
#line 62
 testRunner.Given("That institution \'Planning_Cycles\' is launched", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 63
 testRunner.When("Agent \'test\' connects to organisation \'citizens\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 64
 testRunner.And("Agent \'test\' backward plans to change state to \'a.hasFire=true\' with strategy \'Ba" +
                    "ckwardSearch\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 65
 testRunner.Then("Plan length is \'7\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
