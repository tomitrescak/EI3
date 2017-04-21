Feature: NestedPlanning
	In order to plan more efficiently, we allow nested planning with hierarchical tasks

Scenario: Backtrack plan for action "makeFire"
	Given That institution 'NestedPlanning' is launched 
	When Agent 'elder' connects to organisation 'tribe'
	And Agent 'elder' backward plans to change state to 'a.hasFire=true' with strategy 'BackwardSearch'
	Then Plan length is '8'

Scenario: Backtrack plan for action "makeKnife"
	Given That institution 'NestedPlanning' is launched 
	When Agent 'elder' connects to organisation 'tribe'
	And Agent 'elder' backward plans to change state to 'a.hasKnife=true' with strategy 'BackwardSearch'
	Then Plan length is '6'

Scenario: Backtrack plan for action "makeSpear"
	Given That institution 'NestedPlanning' is launched 
	When Agent 'elder' connects to organisation 'tribe'
	And Agent 'elder' backward plans to change state to 'a.hasSpear=true' with strategy 'BackwardSearch'
	Then Plan length is '6'

Scenario: Backtrack plan for action "isHungry"
	Given That institution 'NestedPlanning' is launched 
	When Agent 'elder' connects to organisation 'tribe'
	And Agent 'elder' backward plans to change state to 'a.isHungry=false' with strategy 'BackwardSearch'
	Then Plan length is '25'

@planning
Scenario: Plan to have resource "a.isHungry=false"
	Given That institution 'NestedPlanning' is launched 
	When Agent 'elder' connects to organisation 'tribe'
	And Agent 'elder' plans to change state to 'a.isHungry=false' with strategy 'ForwardSearch' 
	Then Plan length is '31'

Scenario: Agent can ask to list all the possible goals before attempting to plan the action
	Given That institution 'NestedPlanning' is launched 
	When Agent 'elder' connects to organisation 'tribe'
	And Agent 'elder' int parameter 'a.hunger' is set to '18'
	Then Agent 'elder' has '3' possibilitites to satisfy 'a.hunger=Max;0' 