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
        private readonly ILocator _tablaCanciones;
        private readonly ILocator _filasCanciones;
        private readonly ILocator _menuDropdownButton;
        private readonly ILocator _fogonMenuOption;
        private readonly ILocator _crearFogonMenuOption;
        private readonly ILocator _unirseFogonMenuOption;
        private FogonResultados fogonResultados;

        public FogonPagina(IPage page)
        {

            _page = page;
            fogonResultados = new FogonResultados(page);

            // Selectores para la aplicación del fogón
            _crearFogonButton = _page.Locator("button:has-text('Crear Fogón'), #crear-fogon, .crear-fogon");
            _unirseAFogonButton = _page.Locator("button:has-text('Unirse a Fogón'), #unirse-fogon, .unirse-fogon");
            _fogonIdInput = _page.Locator("input[placeholder*='ID del fogón'], #fogon-id, .fogon-id-input");
            _cargarCancionInput = _page.Locator("input[placeholder*='canción'], #cancion-input, .cancion-input");
            _cargarCancionButton = _page.Locator("button:has-text('Cargar Canción'), #cargar-cancion, .cargar-cancion");
            _cancionActualDisplay = _page.Locator("#cancion-actual, .cancion-actual, .current-song");
            _tablaCanciones = _page.Locator("table.tabla-canciones");
            _filasCanciones = _page.Locator("table.tabla-canciones tbody tr:not([data-detail])");
            _menuDropdownButton = _page.Locator("button#dropdownMenuButton.dropdown-toggle");
            _fogonMenuOption = _page.Locator("a.dropdown-item.dropdown-toggle:has-text('Fogon')");
            _crearFogonMenuOption = _page.Locator("a.dropdown-item:has-text('Crear Fogon')");
            _unirseFogonMenuOption = _page.Locator("a.dropdown-item i.bi-box-arrow-in-right");
        }

        public async Task NavigateAsync(string url)
        {
            await _page.GotoAsync(url);
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        }

   
        public async Task IniciarFogonAsync()
        {
            await AbrirMenu();
            await ClickFogon();
            
        }

        public async Task AbrirMenu()
        {
            if (await _menuDropdownButton.CountAsync() > 0)
            {
                await _menuDropdownButton.ClickAsync();
                await _page.WaitForTimeoutAsync(500); // Esperar a que se abra el menú
            }
        }

        public async Task ClickFogon()
        {
            // Primero click en la opción "Fogon" para expandir el submenú

                await _crearFogonMenuOption.ClickAsync();
        }

        public async Task ClickUnirseFogon()
        {
            await _unirseFogonMenuOption.ClickAsync();
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

            var anyTextInput = _page.Locator("input[type='text'], input:not([type])").First;
            if (await anyTextInput.CountAsync() > 0)
            {
                await anyTextInput.FillAsync(cancion);
                await anyTextInput.PressAsync("Enter");
                await _page.WaitForTimeoutAsync(1000);
            }

            await _page.WaitForTimeoutAsync(1000); // Esperar a que se cargue la canción
                await this.fogonResultados.ClickTocarPorTema(cancion);
                


                    }

        public async Task UnirseAlFogonAsync(string fogonId)
        {
            await AbrirMenu();
            await ClickUnirseFogon();
        }

        public async Task<string> ObtenerCancionEnReproduccionAsync()
        {
            // Buscar el div con la clase cancionNombre
            var cancionElement = _page.Locator("div.cancionNombre");
            
            if (await cancionElement.CountAsync() > 0)
            {
                var cancion = await cancionElement.TextContentAsync();
                if (!string.IsNullOrWhiteSpace(cancion))
                {
                    return cancion.Trim();
                }
            }
            
            return "No se encontró canción en reproducción";
        }

        internal async Task SearchAsync(string searchTerm)
        {
            // Llenar el campo de búsqueda
            var searchInput = _page.Locator("input[name='q'].input-busqueda");
            await searchInput.FillAsync(searchTerm);
            
            // Click en el botón de búsqueda
            var searchButton = _page.Locator("button.search-btn.primary");
            await searchButton.ClickAsync();
            
            
        }

        public async Task<string[]> ObtenerElementos()
        {
            if (await _tablaCanciones.CountAsync() == 0)
            {
                return Array.Empty<string>();
            }

            var elementos = new List<string>();
            var rowCount = await _filasCanciones.CountAsync();

            for (var i = 0; i < rowCount; i++)
            {
                var fila = new CancionTablaRow(_filasCanciones.Nth(i));
                var titulo = await fila.ObtenerTituloAsync();
                if (!string.IsNullOrWhiteSpace(titulo))
                {
                    elementos.Add(titulo);
                }
            }

            return elementos.ToArray();
        }

        internal async Task IrATocar()
        {
            // Hacer click en el elemento del navbar usando la estructura más estable
            var tocarButton = _page.Locator(".navbar-brand:has(.iconofogon)");
            await tocarButton.ClickAsync();
            
            // Esperar a que la página cargue
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        }
    }

    public class CancionTablaRow
    {
        private readonly ILocator _row;

        public CancionTablaRow(ILocator row)
        {
            _row = row;
        }

        public async Task<string?> ObtenerTituloAsync()
        {
            var titulo = await _row.Locator("td:first-child .textoGrande").First.TextContentAsync();
            return string.IsNullOrWhiteSpace(titulo) ? null : titulo.Trim();
        }
    }
}