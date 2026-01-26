using fogonpruebas.Pages;
using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightSpecFlowTests.Configuration;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using static System.Net.Mime.MediaTypeNames;

namespace fogonpruebas
{
    [Binding]
    public class FogonStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly TestConfiguration _config;
        private Dictionary<string, IPage> _usuarioPages;
        private Dictionary<string, FogonPagina> _usuarioFogonPages;
        private string _fogonId;

        private FogonResultados _fogonResultados;
        public FogonStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _config = (TestConfiguration)scenarioContext["Configuration"];
            _usuarioPages = new Dictionary<string, IPage>();
            _usuarioFogonPages = new Dictionary<string, FogonPagina>();
        }

        [Given(@"""([^""]*)"" accede a la aplicacion")]
        public async Task GivenUsuarioAccedeALaAplicacion(string usuario)
        {
            // Crear una nueva página para este usuario
            var context = (IBrowserContext)_scenarioContext["BrowserContext"];
            var page = await context.NewPageAsync();
            
            _usuarioPages[usuario] = page;
            var fogonPagina = new FogonPagina(page);
            _usuarioFogonPages[usuario] = fogonPagina;
            
            // Navegar a la aplicación
            await fogonPagina.NavigateAsync(_config.TestSettings.BaseUrl);
            
            Console.WriteLine($"{usuario} ha accedido a la aplicación");
        }

        [Given(@"""([^""]*)"" inicia un fogon")]
        public async Task GivenUsuarioIniciaUnFogon(string usuario)
        {
            var fogonPagina = _usuarioFogonPages[usuario];
            await fogonPagina.IniciarFogonAsync();
            
            Console.WriteLine($"{usuario} ha iniciado el fogón con ID: {_fogonId}");
        }

        [Given(@"""([^""]*)"" carga la cancion ""([^""]*)""")]
        public async Task GivenUsuarioCargaLaCancion(string usuario, string cancion)
        {
            var fogonPagina = _usuarioFogonPages[usuario];
            await fogonPagina.CargarCancionAsync(cancion);
            
            
            Console.WriteLine($"{usuario} ha cargado la canción: {cancion}");
        }

        [When(@"""([^""]*)"" se une al fogon de ""([^""]*)""")]
        [Given(@"""([^""]*)"" se une al fogon de ""([^""]*)""")]
        public async Task WhenUsuarioSeUneAlFogonDe(string usuario1, string usuario2)
        {
            var fogonPagina = _usuarioFogonPages[usuario1];
            await fogonPagina.UnirseAlFogonAsync(_fogonId);
            
            Console.WriteLine($"{usuario1} se ha unido al fogón de {usuario2}");
        }

        [When(@"""([^""]*)"" va a tocar")]
        public async void WhenVaATocar(string usuario)
        {
            var fogonPagina = _usuarioFogonPages[usuario];
            await fogonPagina.IrATocar();
        }


        [Then(@"""([^""]*)"" ve la cancion ""([^""]*)"" en reproduccion")]
        public async Task ThenUsuarioVeLaCancionEnReproduccion(string usuario, string cancion)
        {
            var fogonPagina = _usuarioFogonPages[usuario];
            var cancionActual = await fogonPagina.ObtenerCancionEnReproduccionAsync();
            
            Assert.That(cancionActual.ToLower(), Is.EqualTo(cancion.ToLower()), 
                $"{usuario} debería ver '{cancion}' en reproducción, pero ve '{cancionActual}'");
            
            Console.WriteLine($"{usuario} ve correctamente la canción '{cancion}' en reproducción");
        }
    }
}