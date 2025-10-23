const { Before, After, BeforeAll, AfterAll, setDefaultTimeout } = require('@cucumber/cucumber');
const mongoHelper = require('./mongoHelper');
const config = require('./config');

// Set default timeout for all steps
setDefaultTimeout(config.timeout);

// Global state to share between steps
global.testContext = {};

/**
 * Before all scenarios
 */
BeforeAll(async function () {
  console.log('\n=== Starting Test Suite ===');
  console.log(`Environment: ${config.environment}`);
  console.log(`Base URL: ${config.baseUrl}`);
  console.log(`MongoDB URI: ${config.mongodbUri}`);
  console.log('===========================\n');
  
  // Connect to MongoDB
  try {
    await mongoHelper.connect();
  } catch (error) {
    console.warn('Warning: Could not connect to MongoDB. Database tests will be skipped.');
  }
});

/**
 * Before each scenario
 */
Before(async function (scenario) {
  console.log(`\nStarting scenario: ${scenario.pickle.name}`);
  
  // Reset test context for each scenario
  global.testContext = {
    response: null,
    data: null,
    error: null
  };
});

/**
 * After each scenario
 */
After(async function (scenario) {
  if (scenario.result.status === 'FAILED') {
    console.log(`Scenario FAILED: ${scenario.pickle.name}`);
    console.log(`Error: ${scenario.result.message}`);
  } else {
    console.log(`Scenario ${scenario.result.status}: ${scenario.pickle.name}`);
  }
  
  // Clean up test context
  global.testContext = {};
});

/**
 * After all scenarios
 */
AfterAll(async function () {
  console.log('\n=== Test Suite Completed ===');
  
  // Close MongoDB connection
  await mongoHelper.close();
  
  console.log('===========================\n');
});
