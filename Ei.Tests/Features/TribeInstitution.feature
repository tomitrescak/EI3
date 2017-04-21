Feature: Institutions used to test advanced planning features
@mytag
Scenario:Making a Fire
	Given That institution 'Tribes' is launched 
	When Agent 'elder' connects to organisation 'tribe'
	And Agent 'elder' automatically continues
	And Agents 'elder' clone 'elder_Main' backward plans to change state to 'a.hasFire=true' with strategy 'BackwardSearch' 
	Then Plan length is '6'


Scenario: Making a Knife
	Given That institution 'Tribes' is launched 
	When Agent 'elder' connects to organisation 'tribe'
	And Agent 'elder' automatically continues
	And Agents 'elder' clone 'elder_Main' backward plans to change state to 'a.hasKnife=true' with strategy 'BackwardSearch' 
	Then Plan length is '4'

Scenario: Making a spear
	Given That institution 'Tribes' is launched 
	When Agent 'elder' connects to organisation 'tribe'
	And Agent 'elder' automatically continues
	And Agents 'elder' clone 'elder_Main' backward plans to change state to 'a.hasSpear=true' with strategy 'BackwardSearch' 
	Then Plan length is '4'

Scenario: Catching a fish
	Given That institution 'Tribes' is launched 
	When Agent 'elder' connects to organisation 'tribe'
	And Agent 'elder' automatically continues
	And Agents 'elder' clone 'elder_Main' backward plans to change state to 'a.fish=1' with strategy 'BackwardSearch' 
	Then Plan length is '4'
