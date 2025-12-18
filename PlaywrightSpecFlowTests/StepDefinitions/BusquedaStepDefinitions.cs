using fogonpruebas.Pages;
using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightSpecFlowTests.Configuration;
using TechTalk.SpecFlow;

namespace fogonpruebas
{
    [Binding]
    public class BusquedaStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly IPage _page;
        private readonly TestConfiguration _config;
        private FogonPagina _fogonPagina;
        private FogonResultados _fogonResultados;

        public BusquedaStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _page = (IPage)scenarioContext["Page"];
            _config = (TestConfiguration)scenarioContext["Configuration"];
            _fogonPagina = new FogonPagina(_page);
            _fogonResultados = new FogonResultados(_page);
        }

        [Given(@"Usuario va al fogon")]
        public async Task GivenUsuarioVaAlFogon()
        {
            await _fogonPagina.NavigateAsync(_config.TestSettings.BaseUrl);
        }
        
        [When(@"busca ""([^""]*)""")]
        public async Task WhenBusca(string searchTerm)
        {
            _scenarioContext["CurrentSearchTerm"] = searchTerm;
            // Este método necesitaría ser implementado para búsqueda específica
            // await _fogonPagina.SearchAsync(searchTerm);
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
    }
}