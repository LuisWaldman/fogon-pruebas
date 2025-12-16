using Microsoft.Playwright;
using PlaywrightSpecFlowTests.Configuration;
using TechTalk.SpecFlow;

namespace PlaywrightSpecFlowTests.Hooks
{
    [Binding]
    public class BrowserHooks
    {
        private static IPlaywright? _playwright;
        private static IBrowser? _browser;
        
        [BeforeTestRun]
        public static async Task BeforeTestRun()
        {
            var config = TestConfiguration.Load();
            
            _playwright = await Playwright.CreateAsync();
            
            var launchOptions = new BrowserTypeLaunchOptions
            {
                Headless = config.TestSettings.Headless,
                SlowMo = config.BrowserSettings.SlowMo,
                Args = new[]
                {
                    "--no-sandbox",
                    "--disable-blink-features=AutomationControlled",
                    "--disable-web-security",
                    "--disable-dev-shm-usage",
                    "--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
                }
            };

            _browser = config.BrowserSettings.BrowserType.ToLower() switch
            {
                "firefox" => await _playwright.Firefox.LaunchAsync(launchOptions),
                "webkit" => await _playwright.Webkit.LaunchAsync(launchOptions),
                _ => await _playwright.Chromium.LaunchAsync(launchOptions)
            };
        }

        [AfterTestRun]
        public static async Task AfterTestRun()
        {
            if (_browser != null)
            {
                await _browser.CloseAsync();
            }
            
            _playwright?.Dispose();
        }

        public static IBrowser Browser => _browser ?? throw new InvalidOperationException("Browser not initialized");
        public static IPlaywright PlaywrightInstance => _playwright ?? throw new InvalidOperationException("Playwright not initialized");
    }
}