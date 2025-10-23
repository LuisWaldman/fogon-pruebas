# language: es
Característica: Ejemplo Completo de Pruebas
  Como usuario del framework de pruebas
  Quiero ver ejemplos prácticos de uso
  Para entender cómo escribir mis propias pruebas

  @ejemplo @api
  Escenario: Ejemplo básico de prueba de API con headers personalizados
    Dado I set the header "Content-Type" to "application/json"
    Y I set the header "Authorization" to "Bearer token123"
    Y I set the request body to:
      """
      {
        "titulo": "Nueva tarea",
        "descripcion": "Completar la documentación",
        "prioridad": "alta",
        "completada": false
      }
      """
    Cuando I send a POST request to "/api/tareas"
    Entonces the response status code should be 201
    Y the response should be a valid JSON

  @ejemplo @database
  Escenario: Ejemplo de flujo completo con base de datos
    # Primero limpiamos la colección
    Dado the database collection "tareas" is empty
    
    # Preparamos un documento
    Y I have a document with:
      """
      {
        "titulo": "Aprender Gherkin",
        "prioridad": "alta",
        "completada": false,
        "fechaCreacion": "2025-10-23"
      }
      """
    
    # Insertamos el documento
    Cuando I insert the document into collection "tareas"
    Entonces the insert should succeed
    
    # Verificamos que se insertó
    Y the collection "tareas" should have 1 document(s)

  @ejemplo @database-query
  Escenario: Ejemplo de consultas y filtros en base de datos
    # Preparamos datos de prueba
    Dado the database collection "tareas" contains:
      """
      [
        {
          "titulo": "Tarea urgente",
          "prioridad": "alta",
          "completada": false
        },
        {
          "titulo": "Tarea normal",
          "prioridad": "media",
          "completada": true
        },
        {
          "titulo": "Tarea importante",
          "prioridad": "alta",
          "completada": true
        }
      ]
      """
    
    # Consultamos tareas de alta prioridad
    Cuando I query collection "tareas" for:
      """
      {"prioridad": "alta"}
      """
    
    # Verificamos los resultados
    Entonces the query should return 2 document(s)
    Y the query result should contain a document with "titulo" equal to "Tarea urgente"

  @ejemplo @database-operations
  Escenario: Ejemplo de actualización y eliminación
    # Preparamos datos
    Dado the database collection "tareas" contains:
      """
      [
        {
          "titulo": "Revisar código",
          "estado": "pendiente"
        },
        {
          "titulo": "Escribir tests",
          "estado": "pendiente"
        }
      ]
      """
    
    # Actualizamos una tarea
    Cuando I update documents in collection "tareas" matching:
      """
      {
        "query": {"titulo": "Revisar código"},
        "update": {"estado": "completada"}
      }
      """
    Entonces the update should modify 1 document(s)
    
    # Eliminamos tareas pendientes
    Cuando I delete documents from collection "tareas" matching:
      """
      {"estado": "pendiente"}
      """
    Entonces the delete should remove 1 document(s)
    Y the collection "tareas" should have 1 document(s)
