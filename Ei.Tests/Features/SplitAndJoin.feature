Feature: Split And Join
	Agent can split into clones.
	Deep clones use separate versions of their resources which are consolidated when joined.
	Shallow clones share their resources which are not consolidated when joined
	In this feature we test shallow clones

@mytag
Scenario: Agents perform shallow split
	Given That institution 'SplitJoin' is launched
	When Agent 'splitter' connects to organisation 'citizens'
	When Agent 'splitter' moves to 'shallowSplit'
	Then Agent 'splitter' splits to '3' 'shallow' clones

	Then Agent 'splitter' does not yet join

	When Clone 'splitter_lui' of 'splitter' performs action 'fart' with ''
	 
	Then Agent 'splitter' int parameter 'a.testMax' is equal to '1'
	Then Agent 'splitter' int parameter 'a.testMin' is equal to '0'
	Then Agent 'splitter' int parameter 'a.testAvg' is equal to '5'
	Then Agent 'splitter' bool parameter 'a.testOneTrue' is equal to 'true'
	Then Agent 'splitter' bool parameter 'a.testAllTrue' is equal to 'false'

Scenario: Agents perform deep split
	Given That institution 'SplitJoin' is launched
	When Agent 'splitter' connects to organisation 'citizens'
	When Agent 'splitter' moves to 'deepSplit'
	Then Agent 'splitter' splits to '3' 'deep' clones
	 
	Then Agent 'splitter' does not yet join

	When Clone 'splitter_clone_3' of 'splitter' performs action 'fart' with ''

	Then Agent 'splitter' int parameter 'a.testMax' is equal to '1'
	Then Agent 'splitter' int parameter 'a.testMin' is equal to '0'
	Then Agent 'splitter' int parameter 'a.testAvg' is equal to '3'
	Then Agent 'splitter' bool parameter 'a.testOneTrue' is equal to 'true'
	Then Agent 'splitter' bool parameter 'a.testAllTrue' is equal to 'false'