Feature: Planning
	We need to be able to plan the resource changes

@mytag
@planning @cycles
Scenario: Plan to have resource "a.hasFire" without cycles
	Given That institution 'InstitutionStart' is launched 
	When Agent 'user' connects with '123' to organisation 'Default'
	And Agent 'user' plans to change state to 'ParentParameter=1' with strategy 'ForwardSearch' 
	Then Plan length is '2'

Scenario: Plan to have resource "a.hasFire" with cycles
	Given That institution 'InstitutionStart' is launched 
	When Agent 'user' connects with '123' to organisation 'Default'
	And Agent 'user' plans to change state to 'ParentParameter=2' with strategy 'ForwardSearch' 
	Then Plan length is '4'
