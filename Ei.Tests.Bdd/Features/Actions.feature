Feature: Actions
	Agent can perform various actions in the system
	- Say a message
	- Start a new workflow
	- Join existing workflow

@mytag
Scenario: Start and join a worfklow and say a message that is delivered to other agents
	Given That institution 'InstitutionStart' is launched
	When Agent 'user1' connects with '123' to organisation 'Default' 
	When Agent 'user2' connects with '123' to organisation 'Default' 
	When Agent 'user3' connects with '123' to organisation 'Default'

	# start a new workflow

	Then Agent 'user1' cannot perform 'joinSubWorkflow' with 'WorkflowInstanceNotRunning'
	When Agent 'user1' performs 'startSubWorkflow'
	When Agent 'user1' joins workflow 'joinSubWorkflow' with 'InstanceId=0'
	Then Agent 'user1' is in workflow 'subWorkflow' position 'start'
	When Agent 'user2' performs 'joinSubWorkflow'
	Then Agent 'user2' is in workflow 'subWorkflow' position 'start'

	Then Agent 'user3' can join '1' workflows in 'joinSubWorkflow'
	Then Agent 'user3' cannot perform 'joinSubWorkflow' with 'AccessDenied'

	# say message, test validations

	Then Agent 'user2' cannot perform 'send' with 'InvalidParameters'
	Then Agent 'user2' fails 'send' with 'Stones=0' and message 'Stones: Value Required'
	Then Agent 'user2' fails 'send' with 'Stones=2;Weight=100' and message 'Weight needs to be max 10'
	
	When Agent 'user2' performs action 'send' with 'Stones=3;Weight=3'
	Then Agent 'user1' is notified of 'send' by 'user2' with 'c' in 'subWorkflow'
	Then Agent 'user2' is notified of 'send' by 'user2' with 'c' in 'subWorkflow'

	# test timeouts

	When Agent 'user1' moves to 'wait'
	Then Agent 'user1' wait move to 'yield' in workflow 'subWorkflow' 

	# user 3 can now join workflow as number of stones is bigger

	Then Agent 'user3' can join '1' workflows in 'joinSubWorkflow'
	When Agent 'user3' performs 'joinSubWorkflow'
	Then Agent 'user3' is in workflow 'subWorkflow' position 'yield'

	# exit workflow, move agents to final state and try to re-enter

	When Agent 'user3' exits workflow
	When Agent 'user1' moves to 'end'
	Then Agent 'user1' exits workflow from 'subWorkflow' to 'main'
	Then Agent 'user2' exits workflow from 'subWorkflow' to 'main'

	# when agent tries to re-enter this instance=0 is no longer running
	Then Agent 'user1' is in position 'inc'

	When Agent 'user3' moves to 'start'
	Then Agent 'user3' cannot perform 'joinSubWorkflow' with 'WorkflowInstanceNotRunning'


@mytag
Scenario: Split and Join

	Given That institution 'InstitutionStart' is launched
	When Agent 'user' connects with '123' to organisation 'Default'
	When Agent 'user' moves to 'inc'
	When Agent 'user' moves to 'split'
	Then Agent 'user' splits to '2' 'shallow' clones
	When Clone 'user_Left' of 'user' moves to 'join' 
	Then Agent 'user' does not yet join
	When Clone 'user_Right' of 'user' moves to 'join' 
	Then Agent 'user' is in position 'joined'





