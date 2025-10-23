const { Given, When, Then } = require('@cucumber/cucumber');
const axios = require('axios');
const config = require('../support/config');

/**
 * API Step Definitions
 * Step definitions for HTTP API testing
 */

/**
 * Given steps - Set up initial state
 */
Given('the application is running', async function () {
  try {
    const response = await axios.get(config.baseUrl, { timeout: 5000 });
    this.applicationRunning = response.status === 200;
  } catch (error) {
    this.applicationRunning = false;
    console.log(`Warning: Could not reach application at ${config.baseUrl}`);
  }
});

Given('I set the header {string} to {string}', function (headerName, headerValue) {
  if (!global.testContext.headers) {
    global.testContext.headers = {};
  }
  global.testContext.headers[headerName] = headerValue;
});

Given('I set the request body to:', function (docString) {
  global.testContext.requestBody = JSON.parse(docString);
});

/**
 * When steps - Perform actions
 */
When('I send a GET request to {string}', async function (endpoint) {
  try {
    const url = `${config.baseUrl}${endpoint}`;
    const response = await axios.get(url, {
      headers: global.testContext.headers || {},
      timeout: config.timeout
    });
    
    global.testContext.response = response;
    global.testContext.data = response.data;
    global.testContext.statusCode = response.status;
  } catch (error) {
    global.testContext.error = error;
    global.testContext.statusCode = error.response ? error.response.status : null;
    global.testContext.data = error.response ? error.response.data : null;
  }
});

When('I send a POST request to {string}', async function (endpoint) {
  try {
    const url = `${config.baseUrl}${endpoint}`;
    const response = await axios.post(url, global.testContext.requestBody || {}, {
      headers: global.testContext.headers || {},
      timeout: config.timeout
    });
    
    global.testContext.response = response;
    global.testContext.data = response.data;
    global.testContext.statusCode = response.status;
  } catch (error) {
    global.testContext.error = error;
    global.testContext.statusCode = error.response ? error.response.status : null;
    global.testContext.data = error.response ? error.response.data : null;
  }
});

When('I send a PUT request to {string}', async function (endpoint) {
  try {
    const url = `${config.baseUrl}${endpoint}`;
    const response = await axios.put(url, global.testContext.requestBody || {}, {
      headers: global.testContext.headers || {},
      timeout: config.timeout
    });
    
    global.testContext.response = response;
    global.testContext.data = response.data;
    global.testContext.statusCode = response.status;
  } catch (error) {
    global.testContext.error = error;
    global.testContext.statusCode = error.response ? error.response.status : null;
    global.testContext.data = error.response ? error.response.data : null;
  }
});

When('I send a DELETE request to {string}', async function (endpoint) {
  try {
    const url = `${config.baseUrl}${endpoint}`;
    const response = await axios.delete(url, {
      headers: global.testContext.headers || {},
      timeout: config.timeout
    });
    
    global.testContext.response = response;
    global.testContext.data = response.data;
    global.testContext.statusCode = response.status;
  } catch (error) {
    global.testContext.error = error;
    global.testContext.statusCode = error.response ? error.response.status : null;
    global.testContext.data = error.response ? error.response.data : null;
  }
});

/**
 * Then steps - Verify outcomes
 */
Then('the response status code should be {int}', function (expectedStatus) {
  const actualStatus = global.testContext.statusCode;
  if (actualStatus !== expectedStatus) {
    throw new Error(`Expected status code ${expectedStatus} but got ${actualStatus}`);
  }
});

Then('the response should contain {string}', function (expectedText) {
  const responseData = JSON.stringify(global.testContext.data);
  if (!responseData.includes(expectedText)) {
    throw new Error(`Response does not contain "${expectedText}". Response: ${responseData}`);
  }
});

Then('the response should be a valid JSON', function () {
  if (typeof global.testContext.data !== 'object') {
    throw new Error('Response is not a valid JSON object');
  }
});

Then('the response field {string} should be {string}', function (field, expectedValue) {
  const actualValue = global.testContext.data[field];
  if (actualValue !== expectedValue) {
    throw new Error(`Expected ${field} to be "${expectedValue}" but got "${actualValue}"`);
  }
});

Then('the response should have field {string}', function (field) {
  if (!(field in global.testContext.data)) {
    throw new Error(`Response does not have field "${field}"`);
  }
});
