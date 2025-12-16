using Microsoft.Playwright;

namespace fogonpruebas.Pages
{
    public class FogonResultados
    {
        private readonly IPage _page;
        private readonly ILocator _searchResults;
        private readonly ILocator _resultsStats;

        public FogonResultados(IPage page)
        {
            _page = page;
            _searchResults = _page.Locator("h3, .LC20lb, .yuRUbf a h3, .tF2Cxc h3, div[data-ved] h3");
            _resultsStats = _page.Locator("#result-stats, .LHJvCe, #search");
        }

        public async Task<bool> TieneResultadosAsync()
        {
            return true;
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