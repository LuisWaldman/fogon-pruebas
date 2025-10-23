const dotenv = require('dotenv');
const path = require('path');

// Load environment variables from .env file
dotenv.config({ path: path.join(__dirname, '../../.env') });

/**
 * Configuration module for Gherkin tests
 * Provides configurable settings for different environments (development/production)
 */
const config = {
  // Base URL for the application under test
  baseUrl: process.env.BASE_URL || 'http://localhost:3000',
  
  // MongoDB connection URI
  mongodbUri: process.env.MONGODB_URI || 'mongodb://localhost:27017/fogon_test',
  
  // Test timeout in milliseconds
  timeout: parseInt(process.env.TEST_TIMEOUT || '30000', 10),
  
  // Current environment
  environment: process.env.NODE_ENV || 'development'
};

module.exports = config;
