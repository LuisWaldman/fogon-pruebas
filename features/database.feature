# language: es
Característica: Pruebas de Base de Datos MongoDB
  Como desarrollador
  Quiero probar operaciones de base de datos
  Para asegurarme de que los datos se gestionan correctamente

  Escenario: Insertar un documento en la base de datos
    Dado the database collection "productos" is empty
    Y I have a document with:
      """
      {
        "nombre": "Laptop",
        "precio": 1200,
        "stock": 10
      }
      """
    Cuando I insert the document into collection "productos"
    Entonces the insert should succeed
    Y the collection "productos" should have 1 document(s)

  Escenario: Consultar documentos en una colección
    Dado the database collection "productos" contains:
      """
      [
        {"nombre": "Laptop", "precio": 1200, "categoria": "electronica"},
        {"nombre": "Mouse", "precio": 25, "categoria": "electronica"},
        {"nombre": "Teclado", "precio": 75, "categoria": "electronica"}
      ]
      """
    Cuando I query collection "productos" for all documents
    Entonces the query should return 3 document(s)

  Escenario: Consultar documentos con filtro
    Dado the database collection "productos" contains:
      """
      [
        {"nombre": "Laptop", "precio": 1200, "categoria": "electronica"},
        {"nombre": "Mouse", "precio": 25, "categoria": "electronica"},
        {"nombre": "Silla", "precio": 150, "categoria": "muebles"}
      ]
      """
    Cuando I query collection "productos" for:
      """
      {"categoria": "electronica"}
      """
    Entonces the query should return 2 document(s)
    Y the query result should contain a document with "nombre" equal to "Laptop"

  Escenario: Actualizar documentos en la base de datos
    Dado the database collection "productos" contains:
      """
      [
        {"nombre": "Laptop", "precio": 1200, "stock": 10}
      ]
      """
    Cuando I update documents in collection "productos" matching:
      """
      {
        "query": {"nombre": "Laptop"},
        "update": {"precio": 1100}
      }
      """
    Entonces the update should modify 1 document(s)

  Escenario: Eliminar documentos de la base de datos
    Dado the database collection "productos" contains:
      """
      [
        {"nombre": "Producto A", "obsoleto": true},
        {"nombre": "Producto B", "obsoleto": false},
        {"nombre": "Producto C", "obsoleto": true}
      ]
      """
    Cuando I delete documents from collection "productos" matching:
      """
      {"obsoleto": true}
      """
    Entonces the delete should remove 2 document(s)
    Y the collection "productos" should have 1 document(s)

  Escenario: Contar documentos en una colección
    Dado the database collection "usuarios" contains:
      """
      [
        {"nombre": "Usuario 1", "activo": true},
        {"nombre": "Usuario 2", "activo": true},
        {"nombre": "Usuario 3", "activo": false}
      ]
      """
    Cuando I count documents in collection "usuarios"
    Entonces the document count should be 3
