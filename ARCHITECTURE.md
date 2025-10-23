# Arquitectura de fogon-pruebas

Este documento describe la arquitectura del framework de pruebas Gherkin.

## Diagrama de Flujo

```
┌─────────────────────────────────────────────────────────────────┐
│                        Usuario / CI/CD                           │
└────────────┬────────────────────────────────────────────────────┘
             │
             │ npm test / npm run test:dev / npm run test:prod
             ▼
┌─────────────────────────────────────────────────────────────────┐
│                         Cucumber CLI                             │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ cucumber.js (configuración)                              │  │
│  │  - Define ubicación de features                          │  │
│  │  - Define ubicación de step definitions                  │  │
│  │  - Configura formatos de reporte                         │  │
│  └──────────────────────────────────────────────────────────┘  │
└────────────┬────────────────────────────────────────────────────┘
             │
             │ Carga configuración
             ▼
┌─────────────────────────────────────────────────────────────────┐
│                  features/support/config.js                      │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ dotenv.config()                                          │  │
│  │  ↓                                                       │  │
│  │ Lee .env                                                 │  │
│  │  - BASE_URL=http://localhost:3000                       │  │
│  │  - MONGODB_URI=mongodb://localhost:27017/db             │  │
│  │  - TEST_TIMEOUT=30000                                   │  │
│  │  - NODE_ENV=development                                 │  │
│  └──────────────────────────────────────────────────────────┘  │
└────────────┬────────────────────────────────────────────────────┘
             │
             │ Exporta configuración
             ▼
┌─────────────────────────────────────────────────────────────────┐
│                    features/support/hooks.js                     │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ BeforeAll:                                               │  │
│  │  - Conecta a MongoDB usando config.mongodbUri           │  │
│  │  - Muestra configuración en consola                     │  │
│  │                                                          │  │
│  │ Before (cada escenario):                                │  │
│  │  - Inicializa global.testContext = {}                   │  │
│  │                                                          │  │
│  │ After (cada escenario):                                 │  │
│  │  - Limpia global.testContext                            │  │
│  │  - Reporta estado del escenario                         │  │
│  │                                                          │  │
│  │ AfterAll:                                               │  │
│  │  - Cierra conexión a MongoDB                            │  │
│  └──────────────────────────────────────────────────────────┘  │
└────────────┬────────────────────────────────────────────────────┘
             │
             │ Ejecuta escenarios
             ▼
┌─────────────────────────────────────────────────────────────────┐
│                      features/*.feature                          │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ Escenario: Prueba de API                                │  │
│  │   Dado I set the header "Content-Type" to "..."         │  │
│  │   Cuando I send a POST request to "/api/..."            │  │
│  │   Entonces the response status code should be 201       │  │
│  └──────────┬───────────────────────────────────────────────┘  │
└─────────────┼───────────────────────────────────────────────────┘
              │
              │ Busca implementación de steps
              ▼
┌─────────────────────────────────────────────────────────────────┐
│           features/step_definitions/*.js                         │
│                                                                   │
│  ┌────────────────────────┐  ┌───────────────────────────────┐ │
│  │   api_steps.js         │  │   database_steps.js           │ │
│  │                        │  │                               │ │
│  │ Given('I set...')      │  │ Given('collection empty')     │ │
│  │ When('I send GET')     │  │ When('I insert...')           │ │
│  │ When('I send POST')    │  │ When('I query...')            │ │
│  │ Then('status code..') │  │ Then('should have N docs')    │ │
│  └────────┬───────────────┘  └─────────┬─────────────────────┘ │
└───────────┼──────────────────────────────┼───────────────────────┘
            │                              │
            │ Usa helpers                  │ Usa helpers
            ▼                              ▼
┌───────────────────────────┐  ┌──────────────────────────────────┐
│     axios (HTTP client)   │  │ features/support/mongoHelper.js  │
│                           │  │                                  │
│ - axios.get()             │  │ - connect()                      │
│ - axios.post()            │  │ - insertDocument()               │
│ - axios.put()             │  │ - findDocuments()                │
│ - axios.delete()          │  │ - updateDocument()               │
│                           │  │ - deleteDocuments()              │
└───────────┬───────────────┘  └─────────┬────────────────────────┘
            │                            │
            │ Hace request a             │ Opera en
            ▼                            ▼
┌───────────────────────────┐  ┌──────────────────────────────────┐
│   BASE_URL (configurable) │  │  MONGODB_URI (configurable)      │
│                           │  │                                  │
│ Development:              │  │ Development:                     │
│ http://localhost:3000     │  │ mongodb://localhost:27017/dev    │
│                           │  │                                  │
│ Production:               │  │ Production:                      │
│ https://prod-app.com      │  │ mongodb://user:pass@prod/db      │
└───────────────────────────┘  └──────────────────────────────────┘
```

## Flujo de Datos

### 1. Inicialización

```
Usuario ejecuta → npm test
    ↓
Cucumber lee → cucumber.js
    ↓
Carga → features/support/config.js
    ↓
Lee → .env (BASE_URL, MONGODB_URI)
    ↓
Ejecuta → BeforeAll hook
    ↓
Conecta → MongoDB
```

### 2. Ejecución de Escenario

```
Cucumber lee → features/api.feature
    ↓
Ejecuta → Before hook (inicializa contexto)
    ↓
Para cada step:
    ↓
    Busca implementación → step_definitions/api_steps.js
    ↓
    Ejecuta función → usa config.baseUrl
    ↓
    Guarda resultado → global.testContext
    ↓
    Valida assertions
    ↓
Ejecuta → After hook (limpia contexto)
```

### 3. Reportes

```
Fin de todos los escenarios
    ↓
Ejecuta → AfterAll hook
    ↓
Cierra → Conexión MongoDB
    ↓
Genera reportes → reports/cucumber-report.{html,json}
```

## Componentes Principales

### 1. Configuración (config.js)

**Responsabilidad**: Centralizar configuración de entornos

**Entrada**: Variables de entorno (.env)

**Salida**: Objeto config con:
- baseUrl
- mongodbUri
- timeout
- environment

**Usado por**: 
- hooks.js (conexión MongoDB)
- api_steps.js (requests HTTP)
- database_steps.js (operaciones BD)

### 2. Hooks (hooks.js)

**Responsabilidad**: Gestionar ciclo de vida de pruebas

**BeforeAll**: Setup global (conectar MongoDB)

**Before**: Setup por escenario (inicializar contexto)

**After**: Cleanup por escenario (limpiar contexto)

**AfterAll**: Cleanup global (cerrar conexiones)

### 3. MongoDB Helper (mongoHelper.js)

**Responsabilidad**: Abstraer operaciones de MongoDB

**Funciones**:
- Conexión/desconexión
- CRUD operations
- Queries con filtros
- Conteo de documentos

**Usado por**: database_steps.js y hooks.js

### 4. Step Definitions

#### api_steps.js
**Responsabilidad**: Implementar steps de API

**Given**: Configurar headers, body
**When**: Enviar requests (GET, POST, PUT, DELETE)
**Then**: Validar responses

**Usa**: 
- axios para HTTP
- config.baseUrl para URL
- global.testContext para estado

#### database_steps.js
**Responsabilidad**: Implementar steps de MongoDB

**Given**: Preparar datos, limpiar colecciones
**When**: Operaciones CRUD
**Then**: Validar resultados

**Usa**:
- mongoHelper para operaciones
- global.testContext para estado

## Patrones de Diseño

### 1. Singleton (MongoDB Connection)

```javascript
let client = null;
let db = null;

async connect() {
  if (client && db) return db;
  // ... crear nueva conexión
}
```

### 2. Context Object (Test State)

```javascript
global.testContext = {
  response: null,
  data: null,
  error: null,
  // ... más estado
}
```

### 3. Factory (Config)

```javascript
const config = {
  baseUrl: process.env.BASE_URL || 'default',
  mongodbUri: process.env.MONGODB_URI || 'default',
  // ...
}
```

### 4. Helper/Utility (mongoHelper)

Abstrae operaciones comunes para evitar duplicación.

## Configuración de Entornos

### Desarrollo (.env)

```env
BASE_URL=http://localhost:3000
MONGODB_URI=mongodb://localhost:27017/fogon_dev
TEST_TIMEOUT=30000
NODE_ENV=development
```

**Uso**:
```bash
npm run test:dev
```

### Producción (.env.production)

```env
BASE_URL=https://production-app.com
MONGODB_URI=mongodb://user:pass@prod:27017/fogon_prod
TEST_TIMEOUT=60000
NODE_ENV=production
```

**Uso**:
```bash
NODE_ENV=production npm test
# o
./scripts/test-prod.sh
```

## Extensibilidad

### Agregar Nuevos Steps

1. Crear archivo en `step_definitions/`
2. Importar `{ Given, When, Then }`
3. Implementar functions
4. Usar `global.testContext` para estado
5. Usar `config` para configuración

### Agregar Nuevos Helpers

1. Crear archivo en `support/`
2. Exportar funciones
3. Importar en step definitions
4. Usar según necesidad

### Agregar Nuevas Features

1. Crear archivo `.feature` en `features/`
2. Usar steps existentes o crear nuevos
3. Ejecutar con `npm test`

## Seguridad

- ✅ `.env` excluido de git
- ✅ Credenciales en variables de entorno
- ✅ Sin secrets hardcodeados
- ✅ Validación de dependencias
- ✅ CodeQL analysis

## Performance

- Conexión MongoDB reutilizada (singleton)
- Context limpiado entre escenarios
- Timeouts configurables
- Reportes asíncronos

## Mantenimiento

### Actualizar Dependencias

```bash
npm update
npm audit
```

### Agregar Dependencia

```bash
npm install <package>
# Verificar seguridad
npm audit
```

### Debugging

```bash
# Dry-run (sin ejecutar)
npx cucumber-js --dry-run

# Ejecutar escenario específico
npx cucumber-js features/api.feature:10

# Ejecutar con tags
npx cucumber-js --tags "@api"
```

## Mejores Prácticas

1. **Mantén steps pequeños y reutilizables**
2. **Usa contexto global para compartir estado**
3. **Limpia estado entre escenarios**
4. **Usa helpers para código común**
5. **Documenta steps complejos**
6. **Mantén features legibles**
7. **Usa variables de entorno para config**
8. **Nunca hardcodees secrets**

## Referencias

- [Cucumber.js Documentation](https://github.com/cucumber/cucumber-js)
- [Gherkin Syntax](https://cucumber.io/docs/gherkin/)
- [MongoDB Node Driver](https://mongodb.github.io/node-mongodb-native/)
- [Axios Documentation](https://axios-http.com/)
