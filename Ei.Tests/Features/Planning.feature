Feature: Planning
	Agents can plan their actions depending on the final action

@planning
Scenario: Create plan for action name
	Given That institution 'OpenInstitution' is launched 
	When Agent 'test' connects to organisation 'citizens'
	And Agent 'test' requests plan to perform 'bar' with strategy 'ForwardSearch' 
	Then Plan length is '3'

@planning
Scenario: Plan action "makeFire"
	Given That institution 'Planning_MakeFire' is launched 
	When Agent 'test' connects to organisation 'citizens'
	And Agent 'test' requests plan to perform 'makeFire' with strategy 'ForwardSearch' 
	Then Plan length is '4'

@planning
Scenario: Binary plan action "makeFire"
	Given That institution 'Planning_MakeFire' is launched 
	When Agent 'test' connects to organisation 'citizens'
	And Agent 'test' requests plan to perform 'makeFire' with strategy 'ForwardSearchWithBinaryPredicates'
	Then Plan length is '4'

@planning
Scenario: Plan to have resource "a.hasFire"
	Given That institution 'Planning_MakeFire' is launched 
	When Agent 'test' connects to organisation 'citizens'
	And Agent 'test' plans to change state to 'a.hasFire=true|a.hasFlint=true|a.hasWood=true' with strategy 'ForwardSearch' 
	Then Plan length is '4'

@planning
Scenario: Backtrack plan for action "makeFire"
	Given That institution 'Planning_MakeFire' is launched 
	When Agent 'test' connects to organisation 'citizens'
	And Agent 'test' backward plans action 'makeFire' with strategy 'BackwardSearch' 
	Then Plan length is '3'

@planning
Scenario: Backtrack plan to have resource "a.hasFire"
	Given That institution 'Planning_MakeFire' is launched 
	When Agent 'test' connects to organisation 'citizens'
	And Agent 'test' backward plans to change state to 'a.hasFire=true' with strategy 'BackwardSearch' 
	Then Plan length is '3'

@planning @cycles
Scenario: Plan action with cycles "makeFire"
	Given That institution 'Planning_Cycles' is launched 
	When Agent 'test' connects to organisation 'citizens'
	And Agent 'test' requests plan to perform 'makeFire' with strategy 'ForwardSearch' 
	Then Plan length is '8'

@planning @cycles
Scenario: Plan to have resource "a.hasFire" with cycles
	Given That institution 'Planning_Cycles' is launched 
	When Agent 'test' connects to organisation 'citizens'
	And Agent 'test' plans to change state to 'a.hasFire=true' with strategy 'ForwardSearch' 
	Then Plan length is '8'

@planning @cycles
Scenario: Backtrack cyclic plan to have resource "a.hasFire" with cycles
	Given That institution 'Planning_Cycles' is launched 
	When Agent 'test' connects to organisation 'citizens'
	And Agent 'test' backward plans to change state to 'a.hasFire=true' with strategy 'BackwardSearch' 
	Then Plan length is '7'