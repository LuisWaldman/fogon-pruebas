using Microsoft.Playwright;
using System.Text.Json;

namespace TestProject1
{
    [TestClass]
    public class Test1 : PageTest
    {
        private static TestConfiguration? _config;
        
        private static TestConfiguration GetConfiguration()
        {
            if (_config == null)
            {
                var configPath = Path.Combine(AppContext.BaseDirectory, "testsettings.json");
                if (File.Exists(configPath))
                {
                    var json = File.ReadAllText(configPath);
                    _config = JsonSerializer.Deserialize<TestConfiguration>(json) ?? new TestConfiguration();
                }
                else
                {
                    _config = new TestConfiguration();
                }
            }
            return _config;
        }

        [TestInitialize]
        public async Task Setup()
        {
            var config = GetConfiguration();
            var isHeaded = Environment.GetEnvironmentVariable("HEADED") == "1" || config.Headed;
            
            if (isHeaded)
            {
                // Configurar modo headed si es necesario
                Environment.SetEnvironmentVariable("PLAYWRIGHT_LAUNCH_OPTIONS", 
                    JsonSerializer.Serialize(new { Headless = false, SlowMo = config.SlowMo }));
            }
        }

        [TestMethod]
        public async Task PruebaNueva()
        {
            var config = GetConfiguration();
            var baseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? config.BaseUrl;
            await Page.GotoAsync(baseUrl);

            // Expect a title "to contain" a substring.
            await Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));

            // create a locator
            var getStarted = Page.Locator("text=Get Started");

            // Expect an attribute "to be strictly equal" to the value.
            await Expect(getStarted).ToHaveAttributeAsync("href", "/docs/intro");

            // Click the get started link.
            await getStarted.ClickAsync();

            // Expects the URL to contain intro.
            await Expect(Page).ToHaveURLAsync(new Regex(".*intro"));
        }
    }

    public class TestConfiguration
    {
        public string BaseUrl { get; set; } = "https://playwright.dev";
        public bool Headed { get; set; } = false;
        public float SlowMo { get; set; } = 0;
    }
}