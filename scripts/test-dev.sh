#!/bin/bash

# Script para ejecutar pruebas en entorno de desarrollo

echo "========================================="
echo "Ejecutando Pruebas en Desarrollo"
echo "========================================="

# Configurar variables de entorno para desarrollo
export NODE_ENV=development
export BASE_URL=${BASE_URL:-http://localhost:3000}
export MONGODB_URI=${MONGODB_URI:-mongodb://localhost:27017/fogon_dev}

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
