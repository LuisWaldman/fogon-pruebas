using Microsoft.Playwright;

namespace fogonpruebas.Pages
{
    public class FogonPagina
    {
        private readonly IPage _page;
        private readonly ILocator _searchBox;
        private readonly ILocator _searchButton;
        private readonly ILocator _acceptCookiesButton;

        public FogonPagina(IPage page)
        {
            _page = page;
            _searchBox = _page.Locator("textarea[name='q'], input[name='q']");
            _searchButton = _page.Locator("input[value='Buscar con Google'], input[value='Google Search'], button[type='submit']:has-text('Buscar')").First;
            _acceptCookiesButton = _page.Locator("button:has-text('Acepto'), button:has-text('Accept all'), button:has-text('I agree')");
        }

        public async Task NavigateAsync(string url)
        {
            await _page.GotoAsync(url);
        }

        public async Task SearchAsync(string searchTerm)
        {
            await _searchBox.WaitForAsync();
            await _searchBox.FillAsync(searchTerm);
            
            // Wait a moment before pressing Enter
            await _page.WaitForTimeoutAsync(500);
            await _searchBox.PressAsync("Enter");
            
            // Wait for navigation to complete
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        }

        public async Task<bool> IsSearchBoxVisibleAsync()
        {
            return await _searchBox.IsVisibleAsync();
        }
    }
}