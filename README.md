# Playwright + SpecFlow Test Project

Este proyecto contiene pruebas automatizadas para buscar términos en Google usando Playwright y SpecFlow con C#.

## 🚀 Características

- **Playwright**: Para automatización del navegador
- **SpecFlow**: Para BDD (Behavior Driven Development) 
- **NUnit**: Como framework de testing
- **Configuración flexible**: Términos de búsqueda configurables via `appsettings.json`
- **Múltiples navegadores**: Soporte para Chromium, Firefox y WebKit
- **Page Object Pattern**: Estructura organizada del código

## 📁 Estructura del Proyecto

```
PlaywrightSpecFlowTests/
├── Configuration/
│   └── TestConfiguration.cs          # Configuración del proyecto
├── Features/
│   └── GoogleSearch.feature          # Escenarios de prueba en Gherkin
├── Hooks/
│   ├── BrowserHooks.cs              # Configuración del navegador
│   └── TestHooks.cs                 # Configuración de pruebas
├── Pages/
│   ├── GoogleHomePage.cs            # Page Object para Google Home
│   └── GoogleSearchResultsPage.cs   # Page Object para resultados
├── StepDefinitions/
│   └── GoogleSearchStepDefinitions.cs # Implementación de los steps
├── appsettings.json                  # Configuración de la aplicación
└── PlaywrightSpecFlowTests.csproj   # Archivo del proyecto
```

## ⚙️ Configuración

El archivo `appsettings.json` permite configurar:

```json
{
  "TestSettings": {
    "SearchTerm": "Playwright testing",     // Término de búsqueda por defecto
    "BaseUrl": "https://www.google.com",    // URL base de Google
    "Timeout": 30000,                       // Timeout en milisegundos
    "Headless": false                       // Modo headless del navegador
  },
  "BrowserSettings": {
    "BrowserType": "chromium",              // Tipo de navegador (chromium, firefox, webkit)
    "SlowMo": 100,                         // Velocidad de ejecución (ms entre acciones)
    "ViewportWidth": 1920,                 // Ancho de la ventana
    "ViewportHeight": 1080                 // Alto de la ventana
  }
}
```

## 🏃‍♂️ Ejecución

### 1. Instalar dependencias

```bash
dotnet restore
```

### 2. Instalar navegadores de Playwright

```bash
pwsh bin/Debug/net8.0/playwright.ps1 install
```

### 3. Ejecutar todas las pruebas

```bash
dotnet test
```

### 4. Ejecutar pruebas específicas por tag

```bash
dotnet test --filter "TestCategory=smoke"
dotnet test --filter "TestCategory=search"
```

### 5. Ejecutar con configuración específica

Modifica `appsettings.json` para cambiar el término de búsqueda o configuraciones del navegador antes de ejecutar las pruebas.

## 🎯 Escenarios de Prueba

### Escenario 1: Búsqueda con término configurable
```gherkin
Scenario: Search for a configurable term on Google
    Given I navigate to Google
    When I search for the configured search term
    Then I should see search results
    And the search results should contain the search term
```

### Escenario 2: Búsqueda con término personalizado
```gherkin
Scenario: Search for a custom term on Google
    Given I navigate to Google
    When I search for "SpecFlow BDD testing"
    Then I should see search results
    And the search results should contain "SpecFlow"
```

## 🔧 Personalización

### Cambiar el término de búsqueda
Edita el valor `SearchTerm` en `appsettings.json`:

```json
{
  "TestSettings": {
    "SearchTerm": "Tu término personalizado aquí"
  }
}
```

### Cambiar navegador
Modifica `BrowserType` en `appsettings.json`:
- `chromium` (por defecto)
- `firefox`
- `webkit`

### Modo headless
Para ejecutar sin interfaz gráfica:
```json
{
  "TestSettings": {
    "Headless": true
  }
}
```

## 📊 Reportes

El proyecto usa NUnit como runner, por lo que puedes generar reportes XML:

```bash
dotnet test --logger "trx;LogFileName=TestResults.trx"
```

## 🛠️ Desarrollo

### Agregar nuevos escenarios
1. Edita `Features/GoogleSearch.feature`
2. Implementa los steps en `StepDefinitions/GoogleSearchStepDefinitions.cs`
3. Crea nuevos Page Objects si es necesario

### Agregar nuevas páginas
1. Crea una nueva clase en `Pages/`
2. Implementa el patrón Page Object
3. Úsala en tus Step Definitions

## 🐛 Troubleshooting

### Error: "Playwright browsers not found"
```bash
pwsh bin/Debug/net8.0/playwright.ps1 install
```

### Error: "Element not found"
- Verifica los selectores en los Page Objects
- Aumenta el timeout en `appsettings.json`
- Ejecuta en modo no-headless para debug

### Error de configuración
- Verifica que `appsettings.json` esté marcado como "Copy Always" en el proyecto
- Asegúrate de que la configuración JSON sea válida

## 📝 Logs y Debug

Para habilitar logs detallados de Playwright, configura la variable de entorno:

```bash
$env:DEBUG="pw:*"
dotnet test
```