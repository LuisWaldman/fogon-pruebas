const { MongoClient } = require('mongodb');
const config = require('./config');

let client = null;
let db = null;

/**
 * MongoDB Helper for Gherkin tests
 * Manages database connections and common operations
 */
const mongoHelper = {
  /**
   * Connect to MongoDB
   */
  async connect() {
    if (client && db) {
      return db;
    }
    
    try {
      client = new MongoClient(config.mongodbUri);
      await client.connect();
      
      // Extract database name from URI or use default
      const dbName = config.mongodbUri.split('/').pop().split('?')[0];
      db = client.db(dbName);
      
      console.log(`Connected to MongoDB: ${config.mongodbUri}`);
      return db;
    } catch (error) {
      console.error('Error connecting to MongoDB:', error.message);
      throw error;
    }
  },

  /**
   * Get database instance
   */
  getDb() {
    if (!db) {
      throw new Error('Database not connected. Call connect() first.');
    }
    return db;
  },

  /**
   * Close MongoDB connection
   */
  async close() {
    if (client) {
      await client.close();
      client = null;
      db = null;
      console.log('MongoDB connection closed');
    }
  },

  /**
   * Insert a document into a collection
   */
  async insertDocument(collectionName, document) {
    const collection = db.collection(collectionName);
    const result = await collection.insertOne(document);
    return result;
  },

  /**
   * Find documents in a collection
   */
  async findDocuments(collectionName, query = {}) {
    const collection = db.collection(collectionName);
    return await collection.find(query).toArray();
  },

  /**
   * Find one document in a collection
   */
  async findOneDocument(collectionName, query = {}) {
    const collection = db.collection(collectionName);
    return await collection.findOne(query);
  },

  /**
   * Update a document in a collection
   */
  async updateDocument(collectionName, query, update) {
    const collection = db.collection(collectionName);
    return await collection.updateOne(query, { $set: update });
  },

  /**
   * Delete documents from a collection
   */
  async deleteDocuments(collectionName, query = {}) {
    const collection = db.collection(collectionName);
    return await collection.deleteMany(query);
  },

  /**
   * Clear a collection (delete all documents)
   */
  async clearCollection(collectionName) {
    const collection = db.collection(collectionName);
    return await collection.deleteMany({});
  },

  /**
   * Count documents in a collection
   */
  async countDocuments(collectionName, query = {}) {
    const collection = db.collection(collectionName);
    return await collection.countDocuments(query);
  }
};

module.exports = mongoHelper;
