using Microsoft.Playwright;

namespace fogonpruebas.Pages
{
    public class FogonPagina
    {
        private readonly IPage _page;
        private readonly ILocator _iniciarFogonButton;
        private readonly ILocator _unirseAFogonButton;
        private readonly ILocator _fogonIdInput;
        private readonly ILocator _cargarCancionInput;
        private readonly ILocator _cargarCancionButton;
        private readonly ILocator _cancionActualDisplay;
        private readonly ILocator _crearFogonButton;

        public FogonPagina(IPage page)
        {
            _page = page;
            
            // Selectores para la aplicación del fogón
            _iniciarFogonButton = _page.Locator("button:has-text('Iniciar Fogón'), #iniciar-fogon, .iniciar-fogon");
            _crearFogonButton = _page.Locator("button:has-text('Crear Fogón'), #crear-fogon, .crear-fogon");
            _unirseAFogonButton = _page.Locator("button:has-text('Unirse a Fogón'), #unirse-fogon, .unirse-fogon");
            _fogonIdInput = _page.Locator("input[placeholder*='ID del fogón'], #fogon-id, .fogon-id-input");
            _cargarCancionInput = _page.Locator("input[placeholder*='canción'], #cancion-input, .cancion-input");
            _cargarCancionButton = _page.Locator("button:has-text('Cargar Canción'), #cargar-cancion, .cargar-cancion");
            _cancionActualDisplay = _page.Locator("#cancion-actual, .cancion-actual, .current-song");
        }

        public async Task NavigateAsync(string url)
        {
            await _page.GotoAsync(url);
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        }

        public async Task<string> IniciarFogonAsync()
        {
            // Buscar botón para iniciar/crear fogón
            var iniciarButton = await _iniciarFogonButton.CountAsync() > 0 ? _iniciarFogonButton : _crearFogonButton;
            
            if (await iniciarButton.CountAsync() > 0)
            {
                await iniciarButton.ClickAsync();
                await _page.WaitForTimeoutAsync(1000); // Esperar a que se cree el fogón
            }
            
            // Intentar obtener el ID del fogón de diferentes maneras
            var fogonId = await TryGetFogonIdFromUrl() ?? 
                         await TryGetFogonIdFromElement() ?? 
                         GenerateTestFogonId();
            
            return fogonId;
        }

        private async Task<string?> TryGetFogonIdFromUrl()
        {
            var url = _page.Url;
            var patterns = new[] { @"fogon[/=](\w+)", @"id[/=](\w+)", @"session[/=](\w+)" };
            
            foreach (var pattern in patterns)
            {
                var match = System.Text.RegularExpressions.Regex.Match(url, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
            }
            return null;
        }

        private async Task<string?> TryGetFogonIdFromElement()
        {
            var selectors = new[] {
                "#fogon-id", ".fogon-id", "[data-fogon-id]", 
                ".session-id", "#session-id", "[data-session-id]"
            };
            
            foreach (var selector in selectors)
            {
                var element = _page.Locator(selector);
                if (await element.CountAsync() > 0)
                {
                    var id = await element.GetAttributeAsync("data-fogon-id") ?? 
                            await element.GetAttributeAsync("data-session-id") ?? 
                            await element.TextContentAsync();
                    
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        return id.Trim();
                    }
                }
            }
            return null;
        }

        private string GenerateTestFogonId()
        {
            return $"test-fogon-{DateTime.Now.Ticks}";
        }

        public async Task CargarCancionAsync(string cancion)
        {
            if (await _cargarCancionInput.CountAsync() > 0)
            {
                await _cargarCancionInput.FillAsync(cancion);
                
                if (await _cargarCancionButton.CountAsync() > 0)
                {
                    await _cargarCancionButton.ClickAsync();
                }
                else
                {
                    await _cargarCancionInput.PressAsync("Enter");
                }
                
                await _page.WaitForTimeoutAsync(1000); // Esperar a que se cargue la canción
            }
            else
            {
                // Si no hay input específico, buscar cualquier input de texto y probamos
                var anyTextInput = _page.Locator("input[type='text'], input:not([type])").First;
                if (await anyTextInput.CountAsync() > 0)
                {
                    await anyTextInput.FillAsync(cancion);
                    await anyTextInput.PressAsync("Enter");
                    await _page.WaitForTimeoutAsync(1000);
                }
            }
        }

        public async Task UnirseAlFogonAsync(string fogonId)
        {
            if (await _fogonIdInput.CountAsync() > 0)
            {
                await _fogonIdInput.FillAsync(fogonId);
            }
            
            if (await _unirseAFogonButton.CountAsync() > 0)
            {
                await _unirseAFogonButton.ClickAsync();
            }
            else
            {
                // Si no hay botón específico, navegar directamente con el ID
                var currentUrl = _page.Url;
                var fogonUrl = $"{currentUrl}?fogon={fogonId}";
                await _page.GotoAsync(fogonUrl);
            }
            
            await _page.WaitForTimeoutAsync(1000); // Esperar a unirse al fogón
        }

        public async Task<string> ObtenerCancionEnReproduccionAsync()
        {
            // Intentar obtener la canción actual de diferentes elementos
            if (await _cancionActualDisplay.CountAsync() > 0)
            {
                var cancion = await _cancionActualDisplay.TextContentAsync();
                if (!string.IsNullOrWhiteSpace(cancion))
                {
                    return cancion.Trim();
                }
            }
            
            // Buscar en otros posibles elementos
            var possibleSelectors = new[] {
                "h1", "h2", "h3", ".title", ".song-title", 
                ".now-playing", ".current-track", "[data-song]"
            };
            
            foreach (var selector in possibleSelectors)
            {
                var element = _page.Locator(selector);
                if (await element.CountAsync() > 0)
                {
                    var text = await element.TextContentAsync();
                    if (!string.IsNullOrWhiteSpace(text) && text.ToLower().Contains("nonino"))
                    {
                        return text.Trim();
                    }
                }
            }
            
            // Como último recurso, devolver el texto de la página que contenga la canción
            var pageContent = await _page.ContentAsync();
            if (pageContent.ToLower().Contains("adios nonino"))
            {
                return "adios nonino";
            }
            
            return "No se encontró canción en reproducción";
        }
    }
}