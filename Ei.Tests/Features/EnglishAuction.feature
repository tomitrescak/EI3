Feature: Implementation of English Auction
	In this feature we implement several important features
	such as messaging and timeouts
	  
@cases
Scenario: EnglishAuctionWithThreeAgents
	Given That institution 'EnglishAuctionFlat' is launched
	
	When Agent 'seller1' connects with organisation 'sellers' password 'pass'
	Then Agent 'seller1' plays role 'seller'

	When Agent 'seller1' joins 'register'
	Then Agent 'seller1' is in workflow 'registerWorkflow' position 'door'
	
	When Agent 'seller1' performs action 'register' with 'item=Name:Test,Description:TestD,StartingPrice: 1000'
    Then Agent 'seller1' is in position 'state'
	And Agent 'seller1' parameter 'w.Item.Name' is equal to 'Test'  
	#And Logs 'ActionPerformed' with 'seller1;register;{item: {Name: 'Test', Description: 'TestD', StartingPrice: 1000}}'
	
	When Agent 'seller1' performs 'startWorkflow'
	#Then Logs 'WorkflowStarted' with 'EnglishAuction;auctionWorkflow;{Item: {Name: 'Test', Description: 'TestD', StartingPrice: 1000}, CurrentPrice: 1000, WinnerName: '', WinnerPaid: false, agentCount: 0, last: }'
	Then Agent 'seller1' has '2' possibilities 
	And Agent 'seller1' possible action id is 'register'
	
	When Agent 'seller1' performs action 'register' with 'item=Name:Test_2,Description:Test2,StartingPrice: 4'
	Then Agent 'seller1' is in position 'state'

	When Agent 'seller1' performs 'startWorkflow'
	#Then Logs 'WorkflowStarted' with 'EnglishAuction;auctionWorkflow;{Item: {Name: 'Test_2', Description: 'Test2', StartingPrice: 4}, CurrentPrice: 4, WinnerName: '', WinnerPaid: false, agentCount: 0, last: }'
	
	When Agent 'seller1' moves to 'exit'
	Then Agent 'seller1' exits workflow from 'registerWorkflow' to 'main'
#	And Agent 'seller1' exits institution    

	# Buyers join the workflow	
	When Agent 'buyer1' connects with organisation 'buyers' password 'pass'
	Then Agent 'buyer1' plays role 'buyer'
	And Agent 'buyer1' can join '2' workflows in 'auction'
	 
	Then Agent 'buyer1' cannot join workflow 'auction' with ''

	When Agent 'buyer1' joins workflow 'auction' with 'instanceId=0' 
	Then Agent 'buyer1' is in workflow 'auctionWorkflow' position 'start' 
	 
	When Agent 'buyer2' connects with organisation 'buyers' password 'pass'   
	When Agent 'buyer2' joins workflow 'auction' with 'instanceId=0' 

	When Agent 'buyer1' moves to 'timer'
	Then Agent 'buyer2' is in position 'timer' 
	Then Agent 'buyer1' is in position 'timer' 

	# First round of bidding
	Then Agent 'buyer1' fails action 'bid' with 'bidValue=10' 
	
	When Agent 'buyer1' performs action 'bid' with 'bidValue=2000'
	Then Agent 'buyer2' is notified of 'bid' by 'buyer1' with 'bidValue: 2000' in 'auctionWorkflow'	 
	Then Agent 'buyer2' fails action 'bid' with 'bidValue=1000'
	# Second round of bidding
	When Agent 'buyer2' performs action 'bid' with 'bidValue=3000'
	Then Agent 'buyer1' is notified of 'bid' by 'buyer2' with 'bidValue: 3000' in 'auctionWorkflow'	 

	# Timeout for winner (1 second)
	Then Agent 'buyer2' wait move to 'end' in workflow 'auctionWorkflow'
	Then Agent 'buyer1' wait move to 'wait' in workflow 'main'
	 
	# Losers automatically exit, winner stays
	Then Agent 'buyer1' exits workflow from 'auctionWorkflow' to 'main'   
	# Buyer 2 cannot exit until it pays for the auctioned item
	Then Agent 'buyer2' fails exit workflow

	#Buyer now pays for the item
	When Agent 'buyer2' performs action 'pay' with 'creditCard=Number:123,ExpirationDate:qwe,Cvc:111' 


