console.log("Ensuring tables and data exist...");

async function ensureTables() {
  try {
    // Import dynamically to handle cases where dependencies might not be installed yet
    const { TableServiceClient } = require("@azure/data-tables");
    
    const storageConnectionString = process.env.AzureWebJobsStorage || "UseDevelopmentStorage=true";
    const tableServiceClient = TableServiceClient.fromConnectionString(storageConnectionString);
    
    // Create required tables if they don't exist
    const tables = ['externalitems', 'state'];
    
    for (const tableName of tables) {
      console.log(`Ensuring table '${tableName}' exists...`);
      try {
        await tableServiceClient.createTable(tableName);
        console.log(`Table '${tableName}' created/verified.`);
      } catch (error) {
        // Table might already exist, which is fine
        if (error.statusCode === 409) {
          console.log(`Table '${tableName}' already exists.`);
        } else {
          throw error;
        }
      }
    }
    
    console.log("Done!");
  } catch (error) {
    if (error.code === 'MODULE_NOT_FOUND') {
      console.error("Error: Required dependencies not found. Please run 'npm install' first.");
    } else {
      console.error(`Error ensuring tables: ${error.message}`);
    }
    process.exit(1);
  }
}

ensureTables();