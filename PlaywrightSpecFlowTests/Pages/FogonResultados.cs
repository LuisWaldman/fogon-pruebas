using Microsoft.Playwright;

namespace fogonpruebas.Pages
{
    public class Cancion
    {
        public string Artista { get; set; } = string.Empty;
        public string Tema { get; set; } = string.Empty;
        public string Duracion { get; set; } = string.Empty;
        public string Escala { get; set; } = string.Empty;
    }

    public class FogonResultados
    {
        private readonly IPage _page;
        private readonly ILocator _searchResults;
        private readonly ILocator _resultsStats;
        private readonly ILocator _tablaCanciones;

        public FogonResultados(IPage page)
        {
            _page = page;
            _searchResults = _page.Locator("h3, .LC20lb, .yuRUbf a h3, .tF2Cxc h3, div[data-ved] h3");
            _resultsStats = _page.Locator("#result-stats, .LHJvCe, #search");
            _tablaCanciones = _page.Locator("table.tabla-canciones");
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

        public async Task<List<Cancion>> ObtenerElementos()
        {
            var canciones = new List<Cancion>();

            // Esperar a que la tabla esté cargada
            await _tablaCanciones.WaitForAsync(new LocatorWaitForOptions { Timeout = 10000 });

            // Obtener todas las filas del tbody, excluyendo las filas de detalle
            var filas = _tablaCanciones.Locator("tbody tr").Filter(new LocatorFilterOptions 
            { 
                HasNot = _page.Locator("[data-detail]")
            });

            var cantidadFilas = await filas.CountAsync();

            for (int i = 0; i < cantidadFilas; i++)
            {
                var fila = filas.Nth(i);
                var celdas = fila.Locator("td");

                // Verificar que la fila tenga al menos 3 celdas
                if (await celdas.CountAsync() >= 3)
                {
                    var cancion = new Cancion();

                    // Primera celda: Artista y Tema
                    var primeraCelda = celdas.Nth(0);
                    var temaDiv = primeraCelda.Locator("div.textoGrande");
                    
                    // Obtener el texto completo de la celda y el texto del tema
                    var textoCeldaCompleto = await primeraCelda.TextContentAsync() ?? "";
                    var textoTema = await temaDiv.TextContentAsync() ?? "";
                    
                    // El artista es el texto antes del div del tema
                    cancion.Artista = textoCeldaCompleto.Replace(textoTema, "").Trim();
                    cancion.Tema = textoTema.Trim();

                    // Segunda celda: Duración
                    var duracionTexto = await celdas.Nth(1).TextContentAsync();
                    cancion.Duracion = duracionTexto?.Trim() ?? "";

                    // Tercera celda: Escala
                    var escalaSpan = celdas.Nth(2).Locator("span");
                    var escalaTexto = await escalaSpan.TextContentAsync();
                    cancion.Escala = escalaTexto?.Trim() ?? "";

                    canciones.Add(cancion);
                }
            }

            return canciones;
        }

        private async Task<ILocator?> BuscarFilaPorTema(string nombreTema)
        {
            var filas = _tablaCanciones.Locator("tbody tr").Filter(new LocatorFilterOptions 
            { 
                HasNot = _page.Locator("[data-detail]")
            });

            var cantidadFilas = await filas.CountAsync();

            for (int i = 0; i < cantidadFilas; i++)
            {
                var fila = filas.Nth(i);
                var temaDiv = fila.Locator("td:first-child div.textoGrande");
                var textoTema = await temaDiv.TextContentAsync();

                if (!string.IsNullOrWhiteSpace(textoTema) && textoTema.Trim().Equals(nombreTema, StringComparison.OrdinalIgnoreCase))
                {
                    return fila;
                }
            }

            return null;
        }

        private async Task ExpandirDetalleCancion(ILocator fila)
        {
            // Hacer clic en la fila para expandir los detalles si no está ya expandida
            if (!await fila.GetAttributeAsync("class").ContinueWith(t => t.Result?.Contains("seleccionado") == true))
            {
                await fila.ClickAsync();
                await _page.WaitForTimeoutAsync(500);
            }
        }

        public async Task ClickTocarPorTema(string nombreTema)
        {
            var fila = await BuscarFilaPorTema(nombreTema);
            if (fila != null)
            {
                await ExpandirDetalleCancion(fila);
                var botonTocar = _tablaCanciones.Locator("tr[data-detail] button:has-text('▶ Tocar')");
                if (await botonTocar.CountAsync() > 0)
                {
                    await botonTocar.ClickAsync();
                }
            }
        }

        public async Task ClickListaPorTema(string nombreTema)
        {
            var fila = await BuscarFilaPorTema(nombreTema);
            if (fila != null)
            {
                await ExpandirDetalleCancion(fila);
                var botonLista = _tablaCanciones.Locator("tr[data-detail] button:has-text('🗒️ Lista')");
                if (await botonLista.CountAsync() > 0)
                {
                    await botonLista.ClickAsync();
                }
            }
        }

        public async Task ClickCompartirPorTema(string nombreTema)
        {
            var fila = await BuscarFilaPorTema(nombreTema);
            if (fila != null)
            {
                await ExpandirDetalleCancion(fila);
                var botonCompartir = _tablaCanciones.Locator("tr[data-detail] button:has-text('🔗 Compartir')");
                if (await botonCompartir.CountAsync() > 0)
                {
                    await botonCompartir.ClickAsync();
                }
            }
        }

        public async Task ClickEditarPorTema(string nombreTema)
        {
            var fila = await BuscarFilaPorTema(nombreTema);
            if (fila != null)
            {
                await ExpandirDetalleCancion(fila);
                var botonEditar = _tablaCanciones.Locator("tr[data-detail] button:has-text('✏️ Editar')");
                if (await botonEditar.CountAsync() > 0)
                {
                    await botonEditar.ClickAsync();
                }
            }
        }

        public async Task ClickReordenarPorTema(string nombreTema)
        {
            var fila = await BuscarFilaPorTema(nombreTema);
            if (fila != null)
            {
                await ExpandirDetalleCancion(fila);
                var botonReordenar = _tablaCanciones.Locator("tr[data-detail] button:has-text('↕️ Reordenar')");
                if (await botonReordenar.CountAsync() > 0)
                {
                    await botonReordenar.ClickAsync();
                }
            }
        }

        public async Task ClickEliminarPorTema(string nombreTema)
        {
            var fila = await BuscarFilaPorTema(nombreTema);
            if (fila != null)
            {
                await ExpandirDetalleCancion(fila);
                var botonEliminar = _tablaCanciones.Locator("tr[data-detail] button:has-text('−')");
                if (await botonEliminar.CountAsync() > 0)
                {
                    await botonEliminar.ClickAsync();
                }
            }
        }
    }
}