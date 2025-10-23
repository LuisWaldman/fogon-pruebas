const { Given, When, Then } = require('@cucumber/cucumber');
const mongoHelper = require('../support/mongoHelper');

/**
 * Database Step Definitions
 * Step definitions for MongoDB database testing
 */

/**
 * Given steps - Set up database state
 */
Given('the database collection {string} is empty', async function (collectionName) {
  try {
    await mongoHelper.clearCollection(collectionName);
    global.testContext.collection = collectionName;
  } catch (error) {
    console.error(`Error clearing collection: ${error.message}`);
    throw error;
  }
});

Given('the database collection {string} contains:', async function (collectionName, docString) {
  try {
    const documents = JSON.parse(docString);
    const documentsArray = Array.isArray(documents) ? documents : [documents];
    
    // Clear collection first
    await mongoHelper.clearCollection(collectionName);
    
    // Insert documents
    for (const doc of documentsArray) {
      await mongoHelper.insertDocument(collectionName, doc);
    }
    
    global.testContext.collection = collectionName;
  } catch (error) {
    console.error(`Error setting up collection: ${error.message}`);
    throw error;
  }
});

Given('I have a document with:', function (docString) {
  global.testContext.document = JSON.parse(docString);
});

/**
 * When steps - Perform database operations
 */
When('I insert the document into collection {string}', async function (collectionName) {
  try {
    const result = await mongoHelper.insertDocument(collectionName, global.testContext.document);
    global.testContext.insertResult = result;
    global.testContext.collection = collectionName;
  } catch (error) {
    console.error(`Error inserting document: ${error.message}`);
    global.testContext.error = error;
  }
});

When('I query collection {string} for:', async function (collectionName, docString) {
  try {
    const query = JSON.parse(docString);
    const documents = await mongoHelper.findDocuments(collectionName, query);
    global.testContext.queryResult = documents;
    global.testContext.collection = collectionName;
  } catch (error) {
    console.error(`Error querying collection: ${error.message}`);
    global.testContext.error = error;
  }
});

When('I query collection {string} for all documents', async function (collectionName) {
  try {
    const documents = await mongoHelper.findDocuments(collectionName, {});
    global.testContext.queryResult = documents;
    global.testContext.collection = collectionName;
  } catch (error) {
    console.error(`Error querying collection: ${error.message}`);
    global.testContext.error = error;
  }
});

When('I update documents in collection {string} matching:', async function (collectionName, docString) {
  try {
    const { query, update } = JSON.parse(docString);
    const result = await mongoHelper.updateDocument(collectionName, query, update);
    global.testContext.updateResult = result;
    global.testContext.collection = collectionName;
  } catch (error) {
    console.error(`Error updating documents: ${error.message}`);
    global.testContext.error = error;
  }
});

When('I delete documents from collection {string} matching:', async function (collectionName, docString) {
  try {
    const query = JSON.parse(docString);
    const result = await mongoHelper.deleteDocuments(collectionName, query);
    global.testContext.deleteResult = result;
    global.testContext.collection = collectionName;
  } catch (error) {
    console.error(`Error deleting documents: ${error.message}`);
    global.testContext.error = error;
  }
});

When('I count documents in collection {string}', async function (collectionName) {
  try {
    const count = await mongoHelper.countDocuments(collectionName);
    global.testContext.documentCount = count;
    global.testContext.collection = collectionName;
  } catch (error) {
    console.error(`Error counting documents: ${error.message}`);
    global.testContext.error = error;
  }
});

/**
 * Then steps - Verify database state
 */
Then('the collection {string} should have {int} document\\(s)', async function (collectionName, expectedCount) {
  const actualCount = await mongoHelper.countDocuments(collectionName);
  if (actualCount !== expectedCount) {
    throw new Error(`Expected ${expectedCount} documents but found ${actualCount}`);
  }
});

Then('the document count should be {int}', function (expectedCount) {
  const actualCount = global.testContext.documentCount;
  if (actualCount !== expectedCount) {
    throw new Error(`Expected ${expectedCount} documents but found ${actualCount}`);
  }
});

Then('the query should return {int} document\\(s)', function (expectedCount) {
  const actualCount = global.testContext.queryResult ? global.testContext.queryResult.length : 0;
  if (actualCount !== expectedCount) {
    throw new Error(`Expected ${expectedCount} documents but found ${actualCount}`);
  }
});

Then('the query result should contain a document with {string} equal to {string}', function (field, expectedValue) {
  const documents = global.testContext.queryResult || [];
  const found = documents.some(doc => {
    const value = doc[field];
    return value !== undefined && value.toString() === expectedValue;
  });
  
  if (!found) {
    throw new Error(`No document found with ${field} equal to "${expectedValue}"`);
  }
});

Then('the insert should succeed', function () {
  if (global.testContext.error) {
    throw new Error(`Insert failed: ${global.testContext.error.message}`);
  }
  if (!global.testContext.insertResult || !global.testContext.insertResult.insertedId) {
    throw new Error('Insert did not return a valid result');
  }
});

Then('the update should modify {int} document\\(s)', function (expectedCount) {
  const actualCount = global.testContext.updateResult ? global.testContext.updateResult.modifiedCount : 0;
  if (actualCount !== expectedCount) {
    throw new Error(`Expected to modify ${expectedCount} documents but modified ${actualCount}`);
  }
});

Then('the delete should remove {int} document\\(s)', function (expectedCount) {
  const actualCount = global.testContext.deleteResult ? global.testContext.deleteResult.deletedCount : 0;
  if (actualCount !== expectedCount) {
    throw new Error(`Expected to delete ${expectedCount} documents but deleted ${actualCount}`);
  }
});
