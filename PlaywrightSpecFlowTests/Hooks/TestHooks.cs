using Microsoft.Playwright;
using PlaywrightSpecFlowTests.Configuration;
using PlaywrightSpecFlowTests.Hooks;
using TechTalk.SpecFlow;

namespace PlaywrightSpecFlowTests.Hooks
{
    [Binding]
    public class TestHooks
    {
        private readonly ScenarioContext _scenarioContext;
        private IPage? _page;

        public TestHooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            var config = TestConfiguration.Load();
            var context = await BrowserHooks.Browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize
                {
                    Width = config.BrowserSettings.ViewportWidth,
                    Height = config.BrowserSettings.ViewportHeight
                }
            });

            _page = await context.NewPageAsync();
            _page.SetDefaultTimeout(config.TestSettings.Timeout);
            
            _scenarioContext["Page"] = _page;
            _scenarioContext["BrowserContext"] = context;
            _scenarioContext["Configuration"] = config;
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            if (_page != null)
            {
                await _page.Context.CloseAsync();
            }
        }
    }
}