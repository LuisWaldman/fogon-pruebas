#!/bin/bash

# Script para ejecutar pruebas en entorno de producción

echo "========================================="
echo "Ejecutando Pruebas en Producción"
echo "========================================="

# Configurar variables de entorno para producción
export NODE_ENV=production

# Validar que las variables requeridas estén configuradas
if [ -z "$BASE_URL" ]; then
    echo "ERROR: BASE_URL no está configurada"
    echo "Ejemplo: export BASE_URL=https://tu-app.com"
    exit 1
fi

if [ -z "$MONGODB_URI" ]; then
    echo "ERROR: MONGODB_URI no está configurada"
    echo "Ejemplo: export MONGODB_URI=mongodb://user:pass@host:27017/db"
    exit 1
fi

echo "Configuración:"
echo "- Entorno: $NODE_ENV"
echo "- URL Base: $BASE_URL"
echo "- MongoDB: $MONGODB_URI"
echo ""

# Ejecutar las pruebas
npm test

echo ""
echo "========================================="
echo "Pruebas Completadas"
echo "========================================="
