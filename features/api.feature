# language: es
Característica: Pruebas de API
  Como desarrollador
  Quiero probar mi API en diferentes entornos
  Para asegurarme de que funciona correctamente

  Escenario: Verificar que la aplicación está corriendo
    Dado the application is running
    Cuando I send a GET request to "/"
    Entonces the response status code should be 200

  Escenario: Crear un nuevo recurso mediante POST
    Dado I set the header "Content-Type" to "application/json"
    Y I set the request body to:
      """
      {
        "nombre": "Producto de prueba",
        "precio": 100,
        "categoria": "test"
      }
      """
    Cuando I send a POST request to "/api/productos"
    Entonces the response status code should be 201
    Y the response should be a valid JSON

  Escenario: Obtener un recurso específico
    Cuando I send a GET request to "/api/productos/123"
    Entonces the response status code should be 200
    Y the response should be a valid JSON
    Y the response should have field "id"

  Escenario: Actualizar un recurso existente
    Dado I set the header "Content-Type" to "application/json"
    Y I set the request body to:
      """
      {
        "nombre": "Producto actualizado",
        "precio": 150
      }
      """
    Cuando I send a PUT request to "/api/productos/123"
    Entonces the response status code should be 200

  Escenario: Eliminar un recurso
    Cuando I send a DELETE request to "/api/productos/123"
    Entonces the response status code should be 204
