Feature: InstitutionManagement
	It is possible to launch or abort the institution.
	- When institution is launched a logger notifies all interested parties about the institution start.
	- When institution is aborted, all agents are notified about institution shutting down and institution exists
	- When institution is evolving, all agents are requested to move into safe areas within time limit, 
	  if the time limit is over, all agents in non-safe areas are disconnected and instituion evolves 

@institution
Scenario: Launch Institution
	Given That institution 'InstitutionStart' is launched
	Then 'InstitutionStarted' is logged with parameter 'Institution' 
