using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightSpecFlowTests.Configuration;
using PlaywrightSpecFlowTests.Pages;
using TechTalk.SpecFlow;

namespace PlaywrightSpecFlowTests.StepDefinitions
{
    [Binding]
    public class GoogleSearchStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly IPage _page;
        private readonly TestConfiguration _config;
        private GoogleHomePage _googleHomePage;
        private GoogleSearchResultsPage _googleSearchResultsPage;

        public GoogleSearchStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _page = (IPage)scenarioContext["Page"];
            _config = (TestConfiguration)scenarioContext["Configuration"];
            _googleHomePage = new GoogleHomePage(_page);
            _googleSearchResultsPage = new GoogleSearchResultsPage(_page);
        }

        [Given(@"I navigate to Google")]
        public async Task GivenINavigateToGoogle()
        {
            await _googleHomePage.NavigateAsync(_config.TestSettings.BaseUrl);
            
            var isSearchBoxVisible = await _googleHomePage.IsSearchBoxVisibleAsync();
            Assert.That(isSearchBoxVisible, Is.True, "Google search box should be visible");
        }

        [When(@"I search for the configured search term")]
        public async Task WhenISearchForTheConfiguredSearchTerm()
        {
            var searchTerm = _config.TestSettings.SearchTerm;
            _scenarioContext["CurrentSearchTerm"] = searchTerm;
            
            await _googleHomePage.SearchAsync(searchTerm);
        }

        [When(@"I search for ""([^""]*)""")]
        public async Task WhenISearchFor(string searchTerm)
        {
            _scenarioContext["CurrentSearchTerm"] = searchTerm;
            await _googleHomePage.SearchAsync(searchTerm);
        }

        [Then(@"I should see search results")]
        public async Task ThenIShouldSeeSearchResults()
        {
            try
            {
                // Debug: Take a screenshot for analysis
                await _page.ScreenshotAsync(new PageScreenshotOptions 
                { 
                    Path = "debug_screenshot.png",
                    FullPage = true 
                });
                
                Console.WriteLine($"Current URL: {_page.Url}");
                Console.WriteLine($"Page Title: {await _page.TitleAsync()}");
                
                var hasResults = await _googleSearchResultsPage.HasSearchResultsAsync();
                Assert.That(hasResults, Is.True, "Search results should be displayed");
                
                var resultsCount = await _googleSearchResultsPage.GetResultsCountAsync();
                Assert.That(resultsCount, Is.GreaterThan(0), $"Expected to see search results, but found {resultsCount}");
                
                Console.WriteLine($"Found {resultsCount} search results");
                
                var resultsStats = await _googleSearchResultsPage.GetResultsStatsAsync();
                if (!string.IsNullOrEmpty(resultsStats))
                {
                    Console.WriteLine($"Results statistics: {resultsStats}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ThenIShouldSeeSearchResults: {ex.Message}");
                Console.WriteLine($"Current URL: {_page.Url}");
                Console.WriteLine($"Page Title: {await _page.TitleAsync()}");
                throw;
            }
        }

        [Then(@"the search results should contain the search term")]
        public async Task ThenTheSearchResultsShouldContainTheSearchTerm()
        {
            var searchTerm = (string)_scenarioContext["CurrentSearchTerm"];
            
            // Extract key words from the search term (in case it's a phrase)
            var keyWords = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var primaryKeyword = keyWords.First();
            
            var containsSearchTerm = await _googleSearchResultsPage.ResultsContainTextAsync(primaryKeyword);
            Assert.That(containsSearchTerm, Is.True, 
                $"Search results should contain the search term '{primaryKeyword}' from '{searchTerm}'");
            
            Console.WriteLine($"Verified that search results contain: {primaryKeyword}");
        }

        [Then(@"the search results should contain ""([^""]*)""")]
        public async Task ThenTheSearchResultsShouldContain(string expectedText)
        {
            var containsText = await _googleSearchResultsPage.ResultsContainTextAsync(expectedText);
            Assert.That(containsText, Is.True, 
                $"Search results should contain '{expectedText}'");
            
            Console.WriteLine($"Verified that search results contain: {expectedText}");
        }
    }
}