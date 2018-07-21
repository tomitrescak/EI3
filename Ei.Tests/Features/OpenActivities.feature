Feature: OpenActivities
	We allow open activities, that do not have to be connected tvia any connection

@mytag
Scenario: Agent can flow between open activities
	Given That institution 'OpenInstitution' is launched 
	When Agent 'test' connects to organisation 'citizens'
	Then Agent 'test' has '1' possibilities 

	Then Agent 'test' moves to 'state1'
	Then Agent 'test' has '2' possibilities 

	When Agent 'test' performs 'foo'
	Then Agent 'test' is in position 'state1'

	When Agent 'test' performs 'bar'
	Then Agent 'test' is in position 'state2'
	And Agent 'test' has '3' possibilities 
