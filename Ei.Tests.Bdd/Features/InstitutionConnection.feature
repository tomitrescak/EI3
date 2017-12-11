Feature: InstitutionConnection
	When agent connects to a new running institution a "Governor" is created representing
	agent behavior in the institution. Governor is responsible for agent notifications.
	New governor notifies "JoinedInstitution" and "JoinedWorkflow" notification.
	Then, governor automatically tries to perform an inital step.
	If step is performed, agent is notified, otherwise it receives "waiting".

@general 
Scenario: Successfull connection, Agent in and out
	Given That institution 'InstitutionStart' is launched 
	When Agent 'user' connects to organisation 'Default' 
	Then Agent 'user' plays role 'Citizen' and belong to the started institution
	#And Logs 'EnteredInstitution' with 'user;cf5bb5f9-5a6a-4999-94ea-eeffd8c3d5d4;Institution'
	Then Agent 'user' notifies institution entry with id 'ConnectionTest' and name 'Connection Test'
	Then Logs 'EnteredWorkflow' with 'user;main;Main'
	Then Agent 'user' notifies workflow entry with id 'main' and name 'Main'
	Then Agent 'user' cannot move to 'end'
	
	When Agent 'user' moves to 'inc'
	Then Agent 'user' int parameter 'ParentParameter' is equal to '1'
	And Logs 'ChangedState' with 'user;inc;main;0'

	When Agent 'user' moves to 'start'
	When Agent 'user' moves to 'end'
	Then Logs 'ExitedWorkflow' with 'user;main;Main;;'
	Then Agent 'user' notifies workflow exit 'Main' id 'main' to 'null' id 'null'
	Then Logs 'ExitedInstitution' with 'user;ConnectionTest;Connection Test'
	Then Agent 'user' notifies institution exit 'Connection Test' id 'ConnectionTest' 
	Then Institution has '0' agents     
	
Scenario: Role inherits properties from parent
	Given That institution 'InstitutionStart' is launched 
	When Agent 'user' connects to organisation 'Default' 
	Then Agent 'user' plays role 'Citizen' and belong to the started institution
	And Agent 'user' has parameter 'ParentParameter'

	

