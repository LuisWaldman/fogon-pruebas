using Microsoft.Playwright;

namespace PlaywrightSpecFlowTests.Pages
{
    public class GoogleSearchResultsPage
    {
        private readonly IPage _page;
        private readonly ILocator _searchResults;
        private readonly ILocator _resultsStats;

        public GoogleSearchResultsPage(IPage page)
        {
            _page = page;
            _searchResults = _page.Locator("h3, .LC20lb, .yuRUbf a h3, .tF2Cxc h3, div[data-ved] h3");
            _resultsStats = _page.Locator("#result-stats, .LHJvCe, #search");
        }

        public async Task<bool> HasSearchResultsAsync()
        {
            try
            {
                // Wait for page to load
                await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                await _page.WaitForTimeoutAsync(3000); // Give time for dynamic content
                
                // Check if we're on a search results page by URL
                var url = _page.Url;
                Console.WriteLine($"Debug: Current URL: {url}");
                
                if (url.Contains("google.com/search") || url.Contains("search?q="))
                {
                    Console.WriteLine("Debug: URL indicates search results page");
                    return true;
                }
                
                // Also check for presence of search elements
                var searchIndicators = new[]
                {
                    "#search",      // Main search container
                    "#res",         // Results container
                    ".g",           // Result items
                    ".tF2Cxc",     // New result format
                    "#result-stats" // Results statistics
                };
                
                foreach (var selector in searchIndicators)
                {
                    try
                    {
                        var element = _page.Locator(selector);
                        if (await element.CountAsync() > 0)
                        {
                            Console.WriteLine($"Debug: Found search indicator: {selector}");
                            return true;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Debug: Exception in HasSearchResultsAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ResultsContainTextAsync(string text)
        {
            try
            {
                // Wait for results to load
                await _searchResults.First.WaitForAsync(new LocatorWaitForOptions { Timeout = 10000 });
                
                // Get all result titles and check if any contains the text
                var resultElements = await _searchResults.AllAsync();
                
                foreach (var element in resultElements.Take(5)) // Check first 5 results
                {
                    try
                    {
                        var resultText = await element.TextContentAsync();
                        if (!string.IsNullOrEmpty(resultText) && 
                            resultText.Contains(text, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                    catch
                    {
                        continue; // Skip this element if there's an error
                    }
                }
                
                // Also check the page content
                var pageContent = await _page.ContentAsync();
                return pageContent.Contains(text, StringComparison.OrdinalIgnoreCase);
            }
            catch (TimeoutException)
            {
                return false;
            }
        }

        public async Task<int> GetResultsCountAsync()
        {
            return await _searchResults.CountAsync();
        }

        public async Task<string> GetResultsStatsAsync()
        {
            try
            {
                await _resultsStats.WaitForAsync(new LocatorWaitForOptions { Timeout = 5000 });
                return await _resultsStats.TextContentAsync() ?? "";
            }
            catch (TimeoutException)
            {
                return "";
            }
        }
    }
}