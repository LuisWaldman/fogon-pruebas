# Guía para Extender el Framework

Esta guía explica cómo agregar tus propios step definitions personalizados para casos de uso específicos.

## Estructura de Step Definitions

Los step definitions se encuentran en `features/step_definitions/` y utilizan la sintaxis de Cucumber.

## Crear Step Definitions Personalizados

### 1. Crear un nuevo archivo

Crea un archivo JavaScript en `features/step_definitions/`, por ejemplo `custom_steps.js`:

```javascript
const { Given, When, Then } = require('@cucumber/cucumber');

// Tus step definitions aquí
```

### 2. Tipos de Steps

#### Given (Dado) - Configuración inicial

```javascript
Given('tengo un usuario con email {string}', function (email) {
  this.userEmail = email;
});

Given('el producto {string} existe en el inventario', async function (productName) {
  // Configurar estado inicial
  global.testContext.product = await findProduct(productName);
});
```

#### When (Cuando) - Acciones

```javascript
When('intento hacer login con mis credenciales', async function () {
  const response = await login(this.userEmail, this.password);
  global.testContext.response = response;
});

When('actualizo el precio del producto a {int}', async function (price) {
  await updateProductPrice(global.testContext.product.id, price);
});
```

#### Then (Entonces) - Verificaciones

```javascript
Then('debería estar autenticado', function () {
  if (!global.testContext.response.authenticated) {
    throw new Error('Usuario no autenticado');
  }
});

Then('el precio debería ser {int}', async function (expectedPrice) {
  const product = await getProduct(global.testContext.product.id);
  if (product.price !== expectedPrice) {
    throw new Error(`Precio esperado: ${expectedPrice}, obtenido: ${product.price}`);
  }
});
```

## Ejemplo Completo: Autenticación

### 1. Crear `features/step_definitions/auth_steps.js`

```javascript
const { Given, When, Then } = require('@cucumber/cucumber');
const axios = require('axios');
const config = require('../support/config');

// Estado para el módulo de autenticación
let authContext = {
  username: null,
  password: null,
  token: null,
  user: null
};

Given('tengo un usuario con username {string} y password {string}', function (username, password) {
  authContext.username = username;
  authContext.password = password;
});

When('hago login', async function () {
  try {
    const response = await axios.post(`${config.baseUrl}/api/auth/login`, {
      username: authContext.username,
      password: authContext.password
    });
    
    authContext.token = response.data.token;
    authContext.user = response.data.user;
    global.testContext.statusCode = response.status;
  } catch (error) {
    global.testContext.error = error;
    global.testContext.statusCode = error.response ? error.response.status : null;
  }
});

Then('debería recibir un token de autenticación', function () {
  if (!authContext.token) {
    throw new Error('No se recibió token de autenticación');
  }
});

Then('el usuario debería tener el rol {string}', function (expectedRole) {
  if (authContext.user.role !== expectedRole) {
    throw new Error(`Rol esperado: ${expectedRole}, obtenido: ${authContext.user.role}`);
  }
});

// Limpiar estado después de cada escenario
const { After } = require('@cucumber/cucumber');
After(function () {
  authContext = {
    username: null,
    password: null,
    token: null,
    user: null
  };
});
```

### 2. Crear `features/autenticacion.feature`

```gherkin
# language: es
Característica: Autenticación de Usuarios

  Escenario: Login exitoso
    Dado tengo un usuario con username "admin" y password "secret123"
    Cuando hago login
    Entonces the response status code should be 200
    Y debería recibir un token de autenticación
    Y el usuario debería tener el rol "administrador"

  Escenario: Login fallido con credenciales incorrectas
    Dado tengo un usuario con username "admin" y password "wrong_password"
    Cuando hago login
    Entonces the response status code should be 401
```

## Ejemplo: Steps para Email

```javascript
const { Given, When, Then } = require('@cucumber/cucumber');
const nodemailer = require('nodemailer');

let emailContext = {
  to: null,
  subject: null,
  body: null,
  sent: false
};

Given('quiero enviar un email a {string}', function (email) {
  emailContext.to = email;
});

Given('el asunto del email es {string}', function (subject) {
  emailContext.subject = subject;
});

When('envío el email con el cuerpo:', function (docString) {
  emailContext.body = docString;
  // Aquí iría la lógica para enviar el email
  emailContext.sent = true;
});

Then('el email debería ser enviado exitosamente', function () {
  if (!emailContext.sent) {
    throw new Error('El email no fue enviado');
  }
});
```

## Mejores Prácticas

### 1. Usar contexto global para compartir estado

```javascript
// Guardar datos en el contexto
global.testContext.myData = { id: 123, name: 'Test' };

// Acceder desde otro step
const myData = global.testContext.myData;
```

### 2. Usar async/await para operaciones asíncronas

```javascript
When('realizo una operación asíncrona', async function () {
  const result = await someAsyncOperation();
  global.testContext.result = result;
});
```

### 3. Manejar errores apropiadamente

```javascript
When('intento una operación que puede fallar', async function () {
  try {
    await riskyOperation();
    global.testContext.error = null;
  } catch (error) {
    global.testContext.error = error;
  }
});

Then('debería haber fallado con el mensaje {string}', function (expectedMessage) {
  if (!global.testContext.error) {
    throw new Error('Se esperaba un error pero no ocurrió');
  }
  
  if (global.testContext.error.message !== expectedMessage) {
    throw new Error(`Mensaje esperado: "${expectedMessage}", obtenido: "${global.testContext.error.message}"`);
  }
});
```

### 4. Limpiar estado después de cada escenario

```javascript
const { After } = require('@cucumber/cucumber');

After(function () {
  // Limpiar cualquier estado específico de tu módulo
  myModuleContext = {};
});
```

### 5. Usar parámetros tipados

```javascript
// Números
Then('el total debería ser {int}', function (expectedTotal) {
  // expectedTotal es un número entero
});

Then('el precio debería ser {float}', function (expectedPrice) {
  // expectedPrice es un número decimal
});

// Strings
Given('el nombre es {string}', function (name) {
  // name es un string
});

// Tablas de datos
Given('los siguientes productos:', function (dataTable) {
  const products = dataTable.hashes();
  // products es un array de objetos
});
```

## Ejemplo con Data Table

```gherkin
Escenario: Crear múltiples productos
  Dado los siguientes productos:
    | nombre    | precio | stock |
    | Laptop    | 1200   | 10    |
    | Mouse     | 25     | 50    |
    | Teclado   | 75     | 30    |
  Cuando los agrego al inventario
  Entonces el inventario debería tener 3 productos
```

```javascript
Given('los siguientes productos:', function (dataTable) {
  const products = dataTable.hashes();
  // products = [
  //   { nombre: 'Laptop', precio: '1200', stock: '10' },
  //   { nombre: 'Mouse', precio: '25', stock: '50' },
  //   { nombre: 'Teclado', precio: '75', stock: '30' }
  // ]
  global.testContext.products = products.map(p => ({
    nombre: p.nombre,
    precio: parseInt(p.precio),
    stock: parseInt(p.stock)
  }));
});

When('los agrego al inventario', async function () {
  for (const product of global.testContext.products) {
    await addToInventory(product);
  }
});
```

## Hooks Personalizados

### Before Hook - Ejecutar antes de cada escenario

```javascript
const { Before } = require('@cucumber/cucumber');

Before(function () {
  console.log('Preparando escenario...');
  // Inicializar estado
});

// Con tags específicos
Before({ tags: '@authentication' }, function () {
  console.log('Preparando autenticación...');
});
```

### After Hook - Ejecutar después de cada escenario

```javascript
const { After } = require('@cucumber/cucumber');

After(function () {
  console.log('Limpiando después del escenario...');
  // Limpiar estado
});

// Con condición
After(function (scenario) {
  if (scenario.result.status === 'FAILED') {
    console.log('Escenario falló, guardando logs...');
  }
});
```

## Testing de tu Código

Ejecuta una prueba específica:

```bash
# Probar un feature específico
npx cucumber-js features/autenticacion.feature

# Probar un escenario específico por línea
npx cucumber-js features/autenticacion.feature:5

# Probar por tags
npx cucumber-js --tags "@authentication"
```

## Recursos Adicionales

- [Documentación oficial de Cucumber](https://cucumber.io/docs/cucumber/)
- [Cucumber Expressions](https://github.com/cucumber/cucumber-expressions)
- [Best Practices](https://cucumber.io/docs/bdd/better-gherkin/)

## Contribuir

Si creas step definitions útiles que otros puedan usar, considera agregarlos al repositorio principal mediante un Pull Request.
