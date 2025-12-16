# Script de configuración inicial para el proyecto Playwright + SpecFlow
Write-Host "🚀 Configurando proyecto Playwright + SpecFlow..." -ForegroundColor Green

# Cambiar al directorio del proyecto
Set-Location "PlaywrightSpecFlowTests"

Write-Host "📦 Restaurando paquetes NuGet..." -ForegroundColor Yellow
dotnet restore

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Paquetes restaurados exitosamente" -ForegroundColor Green
} else {
    Write-Host "❌ Error al restaurar paquetes" -ForegroundColor Red
    exit 1
}

Write-Host "🌐 Compilando el proyecto..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Proyecto compilado exitosamente" -ForegroundColor Green
} else {
    Write-Host "❌ Error al compilar el proyecto" -ForegroundColor Red
    exit 1
}

Write-Host "🎭 Instalando navegadores de Playwright..." -ForegroundColor Yellow
$playwrightPath = "bin/Debug/net8.0/playwright.ps1"

if (Test-Path $playwrightPath) {
    & $playwrightPath install
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Navegadores instalados exitosamente" -ForegroundColor Green
    } else {
        Write-Host "❌ Error al instalar navegadores" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "⚠️  Archivo playwright.ps1 no encontrado. Intenta compilar primero con dotnet build" -ForegroundColor Yellow
    Write-Host "📍 Ruta esperada: $playwrightPath" -ForegroundColor Gray
}

Write-Host ""
Write-Host "🎯 ¡Configuración completada!" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Comandos útiles:" -ForegroundColor Cyan
Write-Host "  • Ejecutar todas las pruebas: dotnet test" -ForegroundColor White
Write-Host "  • Ejecutar pruebas smoke: dotnet test --filter TestCategory=smoke" -ForegroundColor White
Write-Host "  • Ejecutar pruebas de búsqueda: dotnet test --filter TestCategory=search" -ForegroundColor White
Write-Host ""
Write-Host "⚙️  Personalización:" -ForegroundColor Cyan
Write-Host "  • Edita appsettings.json para cambiar el término de búsqueda" -ForegroundColor Gray
Write-Host "  • Cambia SearchTerm por tu palabra deseada" -ForegroundColor Gray
Write-Host ""
Write-Host "🚀 Para ejecutar las pruebas ahora, ejecuta: dotnet test" -ForegroundColor Yellow

# Volver al directorio padre
Set-Location ".."