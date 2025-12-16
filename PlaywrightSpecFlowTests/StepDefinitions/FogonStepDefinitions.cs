using fogonpruebas.Pages;
using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightSpecFlowTests.Configuration;
using TechTalk.SpecFlow;

namespace fogonpruebas
{
    [Binding]
    public class FogonStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly IPage _page;
        private readonly TestConfiguration _config;
        private FogonPagina _fogonPagina;
        private FogonResultados _fogonResultados;

        public FogonStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _page = (IPage)scenarioContext["Page"];
            _config = (TestConfiguration)scenarioContext["Configuration"];
            _fogonPagina = new FogonPagina(_page);
            _fogonResultados = new FogonResultados(_page);
        }

        [Given(@"Usuario va al fogon")]
        public async Task GivenINavigateToGoogle()
        {
            await _fogonPagina.NavigateAsync(_config.TestSettings.BaseUrl);
            
            var isSearchBoxVisible = await _fogonPagina.IsSearchBoxVisibleAsync();
            Assert.That(isSearchBoxVisible, Is.True, "Google search box should be visible");
        }
        [When(@"busca ""([^""]*)""")]
        public async void WhenBusca(string searchTerm)
        {
            _scenarioContext["CurrentSearchTerm"] = searchTerm;
            await _fogonPagina.SearchAsync(searchTerm);
        }


        [Then(@"aparecen resultados relacionados con ""([^""]*)""")]
        public async Task ThenResultsRelatedToAppear(string searchTerm)
        {
            try
            {
                var hasResults = await _fogonResultados.TieneResultadosAsync();
                Assert.That(hasResults, Is.True, "Search results should be displayed");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ThenResultsRelatedToAppear: {ex.Message}");
                Console.WriteLine($"Current URL: {_page.Url}");
                Console.WriteLine($"Page Title: {await _page.TitleAsync()}");
                throw;
            }
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
                
                var hasResults = await _fogonResultados.TieneResultadosAsync();
                Assert.That(hasResults, Is.True, "Search results should be displayed");
                
                var resultsCount = await _fogonResultados.GetResultsCountAsync();
                Assert.That(resultsCount, Is.GreaterThan(0), $"Expected to see search results, but found {resultsCount}");
                
                Console.WriteLine($"Found {resultsCount} search results");
                
                var resultsStats = await _fogonResultados.GetResultsStatsAsync();
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

    }
}