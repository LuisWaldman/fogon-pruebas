using Microsoft.Extensions.Configuration;

namespace PlaywrightSpecFlowTests.Configuration
{
    public class TestConfiguration
    {
        public TestSettings TestSettings { get; set; } = new();
        public BrowserSettings BrowserSettings { get; set; } = new();
        
        public static TestConfiguration Load()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var testConfig = new TestConfiguration();
            configuration.Bind(testConfig);
            
            return testConfig;
        }
    }

    public class TestSettings
    {
        public string SearchTerm { get; set; } = "Playwright testing";
        public string BaseUrl { get; set; } = "https://www.google.com";
        public int Timeout { get; set; } = 30000;
        public bool Headless { get; set; } = false;
    }

    public class BrowserSettings
    {
        public string BrowserType { get; set; } = "chromium";
        public int SlowMo { get; set; } = 100;
        public int ViewportWidth { get; set; } = 1920;
        public int ViewportHeight { get; set; } = 1080;
    }
}