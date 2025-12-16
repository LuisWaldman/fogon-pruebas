Feature: Google Search
    As a user
    I want to search for terms on Google
    So that I can find relevant information

    @smoke @search
    Scenario: Search for a configurable term on Google
        Given I navigate to Google
        When I search for the configured search term
        Then I should see search results
        And the search results should contain the search term

    @smoke @search
    Scenario: Search for a custom term on Google
        Given I navigate to Google
        When I search for "SpecFlow BDD testing"
        Then I should see search results
        And the search results should contain "SpecFlow"