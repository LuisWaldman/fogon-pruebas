# fogon-pruebas

Aplicación de pruebas con Gherkin para testing en lenguaje natural de aplicaciones web con soporte para diferentes entornos (desarrollo y producción).

## Características

- ✅ Pruebas escritas en lenguaje natural usando Gherkin/Cucumber
- ✅ Configuración de URL base para diferentes entornos (desarrollo/producción)
- ✅ Configuración de MongoDB para pruebas de base de datos
- ✅ Step definitions predefinidas para API REST
- ✅ Step definitions predefinidas para operaciones de MongoDB
- ✅ Soporte para español en archivos .feature
- ✅ Reportes HTML y JSON generados automáticamente

## Requisitos Previos

- Node.js (versión 14 o superior)
- MongoDB (local o remoto)
- npm o yarn

## Instalación

1. Clonar el repositorio:
```bash
git clone https://github.com/LuisWaldman/fogon-pruebas.git
cd fogon-pruebas
```

2. Instalar dependencias:
```bash
npm install
```

3. Configurar las variables de entorno:
```bash
cp .env.example .env
```

4. Editar el archivo `.env` con tu configuración:
```env
# Para desarrollo
BASE_URL=http://localhost:3000
MONGODB_URI=mongodb://localhost:27017/fogon_dev

# Para producción (descomentar y ajustar)
# BASE_URL=https://tu-app-produccion.com
# MONGODB_URI=mongodb://usuario:password@tu-servidor:27017/fogon_prod
```

## Estructura del Proyecto

```
fogon-pruebas/
├── features/                    # Archivos de características Gherkin
│   ├── api.feature             # Pruebas de API
│   ├── database.feature        # Pruebas de base de datos
│   ├── step_definitions/       # Implementación de los steps
│   │   ├── api_steps.js       # Steps para API
│   │   └── database_steps.js  # Steps para MongoDB
│   └── support/                # Configuración y utilidades
│       ├── config.js          # Configuración de entornos
│       ├── hooks.js           # Hooks de inicio/fin
│       └── mongoHelper.js     # Helper de MongoDB
├── cucumber.js                 # Configuración de Cucumber
├── package.json
└── .env                       # Variables de entorno (no incluido en git)
```

## Uso

### Ejecutar Todas las Pruebas

```bash
npm test
```

### Ejecutar Pruebas por Entorno

```bash
# Desarrollo
npm run test:dev

# Producción
npm run test:prod
```

### Ejecutar Pruebas Específicas

```bash
# Solo pruebas de API
npm run test:api

# Solo pruebas de base de datos
npm run test:database
```

### Ejecutar una Feature Específica

```bash
npx cucumber-js features/api.feature
```

### Ejecutar un Escenario Específico

```bash
npx cucumber-js features/api.feature:10  # Línea 10
```

## Configuración de Entornos

### Desarrollo
Crea un archivo `.env`:
```env
BASE_URL=http://localhost:3000
MONGODB_URI=mongodb://localhost:27017/fogon_dev
TEST_TIMEOUT=30000
```

### Producción
Crea un archivo `.env.production`:
```env
BASE_URL=https://mi-app-produccion.com
MONGODB_URI=mongodb://usuario:password@servidor.com:27017/fogon_prod
TEST_TIMEOUT=60000
```

Luego ejecuta con:
```bash
NODE_ENV=production npm test
```

## Escribir Nuevas Pruebas

### Ejemplo de Prueba de API

Crea un archivo `.feature` en el directorio `features/`:

```gherkin
# language: es
Característica: Gestión de Usuarios
  
  Escenario: Crear un nuevo usuario
    Dado I set the header "Content-Type" to "application/json"
    Y I set the request body to:
      """
      {
        "nombre": "Juan Pérez",
        "email": "juan@example.com"
      }
      """
    Cuando I send a POST request to "/api/usuarios"
    Entonces the response status code should be 201
    Y the response should have field "id"
```

### Ejemplo de Prueba de Base de Datos

```gherkin
# language: es
Característica: Gestión de Productos
  
  Escenario: Verificar stock de productos
    Dado the database collection "productos" contains:
      """
      [
        {"nombre": "Producto A", "stock": 5},
        {"nombre": "Producto B", "stock": 0}
      ]
      """
    Cuando I query collection "productos" for:
      """
      {"stock": {"$gt": 0}}
      """
    Entonces the query should return 1 document(s)
```

## Steps Disponibles

### API Steps

**Given (Dado):**
- `the application is running`
- `I set the header "{header}" to "{value}"`
- `I set the request body to:` (con JSON)

**When (Cuando):**
- `I send a GET request to "{endpoint}"`
- `I send a POST request to "{endpoint}"`
- `I send a PUT request to "{endpoint}"`
- `I send a DELETE request to "{endpoint}"`

**Then (Entonces):**
- `the response status code should be {code}`
- `the response should contain "{text}"`
- `the response should be a valid JSON`
- `the response field "{field}" should be "{value}"`
- `the response should have field "{field}"`

### Database Steps

**Given (Dado):**
- `the database collection "{collection}" is empty`
- `the database collection "{collection}" contains:` (con JSON)
- `I have a document with:` (con JSON)

**When (Cuando):**
- `I insert the document into collection "{collection}"`
- `I query collection "{collection}" for:` (con JSON query)
- `I query collection "{collection}" for all documents`
- `I update documents in collection "{collection}" matching:` (con JSON)
- `I delete documents from collection "{collection}" matching:` (con JSON)
- `I count documents in collection "{collection}"`

**Then (Entonces):**
- `the collection "{collection}" should have {count} document(s)`
- `the document count should be {count}`
- `the query should return {count} document(s)`
- `the query result should contain a document with "{field}" equal to "{value}"`
- `the insert should succeed`
- `the update should modify {count} document(s)`
- `the delete should remove {count} document(s)`

## Reportes

Los reportes se generan automáticamente en el directorio `reports/`:

- `cucumber-report.html` - Reporte HTML visual
- `cucumber-report.json` - Reporte JSON para integración con CI/CD

Para ver el reporte HTML, abre el archivo en tu navegador:
```bash
open reports/cucumber-report.html  # macOS
xdg-open reports/cucumber-report.html  # Linux
start reports/cucumber-report.html  # Windows
```

## Variables de Entorno

| Variable | Descripción | Valor por Defecto |
|----------|-------------|-------------------|
| `BASE_URL` | URL base de la aplicación a probar | `http://localhost:3000` |
| `MONGODB_URI` | URI de conexión a MongoDB | `mongodb://localhost:27017/fogon_test` |
| `TEST_TIMEOUT` | Timeout de pruebas en ms | `30000` |
| `NODE_ENV` | Entorno de ejecución | `development` |

## Integración Continua

Ejemplo de configuración para GitHub Actions:

```yaml
name: Tests
on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - uses: actions/setup-node@v2
        with:
          node-version: '16'
      - run: npm install
      - run: npm test
        env:
          BASE_URL: ${{ secrets.BASE_URL }}
          MONGODB_URI: ${{ secrets.MONGODB_URI }}
```

## Solución de Problemas

### Error de conexión a MongoDB

Si las pruebas de base de datos fallan con error de conexión:

1. Verifica que MongoDB esté corriendo:
```bash
# En local
mongosh  # o mongo en versiones anteriores

# O verifica el servicio
sudo systemctl status mongod
```

2. Verifica la URI en `.env`:
```env
MONGODB_URI=mongodb://localhost:27017/fogon_test
```

### Error de conexión a la aplicación

Si las pruebas de API fallan:

1. Verifica que tu aplicación esté corriendo en la URL configurada
2. Verifica la variable `BASE_URL` en `.env`
3. Puedes comentar temporalmente las pruebas de API si solo quieres probar la base de datos

## Contribuir

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/nueva-caracteristica`)
3. Commit tus cambios (`git commit -am 'Añadir nueva característica'`)
4. Push a la rama (`git push origin feature/nueva-caracteristica`)
5. Abre un Pull Request

## Licencia

ISC
