Feature: CallingApi

Calling scenarios with different cases

Scenario: Calling proper api with success result
	Given I have proper client
	When I call api with proper parameters
    Then I should get success result
