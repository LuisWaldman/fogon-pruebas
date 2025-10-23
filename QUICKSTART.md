# Guía de Inicio Rápido - fogon-pruebas

Esta guía te ayudará a empezar a usar el framework de pruebas Gherkin en menos de 5 minutos.

## 1. Instalación Rápida

```bash
# Clonar el repositorio
git clone https://github.com/LuisWaldman/fogon-pruebas.git
cd fogon-pruebas

# Instalar dependencias
npm install

# Copiar el archivo de configuración
cp .env.example .env
```

## 2. Configurar tu Entorno

Edita el archivo `.env` con tus configuraciones:

```env
# URL de tu aplicación (desarrollo o producción)
BASE_URL=http://localhost:3000

# URI de tu base de datos MongoDB
MONGODB_URI=mongodb://localhost:27017/tu_base_de_datos
```

## 3. Ejecutar tus Primeras Pruebas

### Opción A: Ejecutar todas las pruebas
```bash
npm test
```

### Opción B: Ejecutar solo pruebas de API
```bash
npm run test:api
```

### Opción C: Ejecutar solo pruebas de base de datos
```bash
npm run test:database
```

## 4. Escribir tu Primera Prueba

Crea un nuevo archivo `features/mi-primera-prueba.feature`:

```gherkin
# language: es
Característica: Mi primera prueba

  Escenario: Verificar que la API responde
    Cuando I send a GET request to "/api/health"
    Entonces the response status code should be 200
```

¡Y listo! Ejecuta `npm test` para ver tu prueba en acción.

## 5. Ejemplos Comunes

### Probar un endpoint POST

```gherkin
Escenario: Crear un nuevo recurso
  Dado I set the header "Content-Type" to "application/json"
  Y I set the request body to:
    """
    {
      "nombre": "Mi recurso",
      "valor": 123
    }
    """
  Cuando I send a POST request to "/api/recursos"
  Entonces the response status code should be 201
```

### Verificar datos en MongoDB

```gherkin
Escenario: Verificar datos en la base de datos
  Cuando I query collection "usuarios" for all documents
  Entonces the query should return 5 document(s)
```

## 6. Cambiar entre Desarrollo y Producción

### Para Desarrollo:
```bash
npm run test:dev
```

### Para Producción:
```bash
export BASE_URL=https://mi-app-produccion.com
export MONGODB_URI=mongodb://user:pass@prod-server:27017/prod_db
npm run test:prod
```

## 7. Ver Reportes

Después de ejecutar las pruebas, abre el reporte HTML:

```bash
# El reporte se genera en reports/cucumber-report.html
open reports/cucumber-report.html  # macOS
xdg-open reports/cucumber-report.html  # Linux
start reports/cucumber-report.html  # Windows
```

## ¿Necesitas Ayuda?

Consulta el [README.md](README.md) completo para:
- Lista completa de steps disponibles
- Ejemplos avanzados
- Configuración de CI/CD
- Solución de problemas

## Siguientes Pasos

1. Revisa los ejemplos en `features/ejemplos.feature`
2. Personaliza las pruebas para tu aplicación
3. Agrega tus propios step definitions si es necesario
4. Configura la integración continua

¡Feliz testing! 🎉
