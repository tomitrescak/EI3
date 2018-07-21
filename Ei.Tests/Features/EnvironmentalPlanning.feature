Feature: Agents can plan actions depending on their environment.
    Agents should consider the state of the environment when planning their actions.
	The cost of action is represented by the distance that agent has to travel from the last executed action. 

@planning
Scenario: Create plan prefering Fishing
	Given That institution 'NestedPlanning' is launched 
	When Agent 'elder' connects to organisation 'tribe'
	And Agent 'elder' plans state 'a.isHungry=false|a.hasSpear=true|a.hasFire=true|a.hasKnife=true' with environment 'PreferFish'
	Then Then the plan contains 'cookFish'

@planning
Scenario: Create plan prefering Kangaroo
	Given That institution 'NestedPlanning' is launched 
	When Agent 'elder' connects to organisation 'tribe'
	And Agent 'elder' plans state 'a.isHungry=false|a.hasSpear=true|a.hasFire=true|a.hasKnife=true' with environment 'PreferKangaroo'
	Then Then the plan contains 'cookKangaroo'
