# Estructura del Proyecto fogon-pruebas

Este documento describe la estructura completa del proyecto y el propósito de cada archivo.

## Estructura de Directorios

```
fogon-pruebas/
├── features/                           # Archivos de características Gherkin
│   ├── api.feature                    # Pruebas de API REST
│   ├── database.feature               # Pruebas de MongoDB
│   ├── ejemplos.feature               # Ejemplos completos de uso
│   ├── step_definitions/              # Implementación de steps
│   │   ├── api_steps.js              # Steps para pruebas de API
│   │   └── database_steps.js         # Steps para pruebas de base de datos
│   └── support/                       # Archivos de soporte
│       ├── config.js                 # Configuración de entornos
│       ├── hooks.js                  # Hooks de inicio/fin de pruebas
│       └── mongoHelper.js            # Utilidades para MongoDB
├── scripts/                           # Scripts de automatización
│   ├── test-dev.sh                   # Ejecutar pruebas en desarrollo
│   └── test-prod.sh                  # Ejecutar pruebas en producción
├── reports/                           # Reportes generados (git ignored)
│   ├── cucumber-report.html          # Reporte HTML visual
│   └── cucumber-report.json          # Reporte JSON para CI/CD
├── .env.example                       # Plantilla de configuración para desarrollo
├── .env.production.example            # Plantilla de configuración para producción
├── .gitignore                         # Archivos a ignorar en git
├── cucumber.js                        # Configuración de Cucumber
├── package.json                       # Dependencias y scripts del proyecto
├── README.md                          # Documentación principal
├── QUICKSTART.md                      # Guía de inicio rápido
├── EXTENDING.md                       # Guía para extender el framework
└── PROJECT_STRUCTURE.md               # Este archivo
```

## Archivos Principales

### Configuración

- **`.env.example`**: Plantilla de configuración para entorno de desarrollo
  - Define BASE_URL, MONGODB_URI y otras variables
  - Debe copiarse a `.env` y personalizarse

- **`.env.production.example`**: Plantilla para entorno de producción
  - Configuración específica para producción
  - Incluye ejemplos de MongoDB Atlas

- **`cucumber.js`**: Configuración de Cucumber
  - Define dónde buscar features y step definitions
  - Configura formatos de reporte
  - Establece opciones de ejecución

- **`package.json`**: Manifesto del proyecto
  - Lista de dependencias npm
  - Scripts de ejecución
  - Metadatos del proyecto

### Features (Pruebas)

- **`features/api.feature`**: Ejemplos de pruebas de API
  - GET, POST, PUT, DELETE requests
  - Manejo de headers
  - Validación de respuestas

- **`features/database.feature`**: Ejemplos de pruebas de base de datos
  - Insertar, consultar, actualizar, eliminar documentos
  - Consultas con filtros
  - Conteo de documentos

- **`features/ejemplos.feature`**: Ejemplos completos y prácticos
  - Casos de uso reales
  - Combinación de API y base de datos
  - Mejores prácticas

### Step Definitions (Implementación)

- **`features/step_definitions/api_steps.js`**: 
  - Implementación de steps para API REST
  - Manejo de requests HTTP con axios
  - Validación de respuestas JSON

- **`features/step_definitions/database_steps.js`**:
  - Implementación de steps para MongoDB
  - Operaciones CRUD completas
  - Consultas y filtros avanzados

### Soporte y Utilidades

- **`features/support/config.js`**:
  - Carga variables de entorno
  - Proporciona configuración a todo el framework
  - Maneja diferentes entornos

- **`features/support/hooks.js`**:
  - BeforeAll: Conecta a MongoDB, muestra configuración
  - Before: Inicializa contexto para cada escenario
  - After: Limpia y reporta resultados
  - AfterAll: Cierra conexiones

- **`features/support/mongoHelper.js`**:
  - Funciones helper para MongoDB
  - connect(), insertDocument(), findDocuments()
  - updateDocument(), deleteDocuments(), etc.
  - Abstrae operaciones comunes

### Scripts

- **`scripts/test-dev.sh`**:
  - Ejecuta pruebas en desarrollo
  - Configura variables de entorno por defecto
  - Muestra información de configuración

- **`scripts/test-prod.sh`**:
  - Ejecuta pruebas en producción
  - Valida que variables requeridas estén configuradas
  - Seguro para entornos productivos

### Documentación

- **`README.md`**: Documentación principal y completa
  - Instalación y configuración
  - Guía de uso
  - Lista de steps disponibles
  - Solución de problemas

- **`QUICKSTART.md`**: Guía de inicio rápido
  - Instalación en 5 minutos
  - Primeros pasos
  - Ejemplos básicos

- **`EXTENDING.md`**: Guía para desarrolladores
  - Cómo crear step definitions personalizados
  - Ejemplos de código
  - Mejores prácticas

- **`PROJECT_STRUCTURE.md`**: Este archivo
  - Descripción completa del proyecto
  - Propósito de cada archivo
  - Referencia rápida

## Flujo de Ejecución

1. **Inicio**: Cucumber lee `cucumber.js` para configuración
2. **Carga**: Lee archivos `.feature` del directorio `features/`
3. **Setup**: Ejecuta hooks `BeforeAll` y `Before`
4. **Ejecución**: 
   - Para cada escenario:
     - Ejecuta steps definidos en `step_definitions/`
     - Usa helpers de `support/` según necesario
5. **Verificación**: Valida que assertions pasen
6. **Cleanup**: Ejecuta hooks `After` y `AfterAll`
7. **Reporte**: Genera reportes en `reports/`

## Variables de Entorno

| Variable | Archivo | Propósito |
|----------|---------|-----------|
| `BASE_URL` | `.env` | URL de la aplicación a probar |
| `MONGODB_URI` | `.env` | Conexión a MongoDB |
| `TEST_TIMEOUT` | `.env` | Timeout de pruebas en ms |
| `NODE_ENV` | `.env` | Entorno (development/production) |

## Scripts npm Disponibles

| Comando | Descripción |
|---------|-------------|
| `npm test` | Ejecuta todas las pruebas |
| `npm run test:dev` | Pruebas en desarrollo |
| `npm run test:prod` | Pruebas en producción |
| `npm run test:api` | Solo pruebas de API |
| `npm run test:database` | Solo pruebas de BD |
| `npm run test:ejemplos` | Ejecuta ejemplos |

## Dependencias

### Producción
- **axios**: Cliente HTTP para pruebas de API
- **dotenv**: Carga variables de entorno
- **mongodb**: Driver oficial de MongoDB

### Desarrollo
- **@cucumber/cucumber**: Framework Gherkin/BDD

## Personalización

Para personalizar el framework para tu proyecto:

1. Copia `.env.example` a `.env` y configura tus valores
2. Crea tus propios archivos `.feature` en `features/`
3. Agrega step definitions personalizados en `step_definitions/`
4. Usa los helpers existentes o crea nuevos en `support/`

## Mantenimiento

- Los reportes en `reports/` se regeneran en cada ejecución
- Las dependencias se gestionan con npm
- `.gitignore` mantiene limpio el repositorio
- Los hooks garantizan limpieza de estado entre tests

## Soporte

Para más información:
- Ver `README.md` para documentación detallada
- Ver `QUICKSTART.md` para empezar rápidamente
- Ver `EXTENDING.md` para personalizar el framework
