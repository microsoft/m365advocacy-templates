import * as restify from "restify";
import adapter from "./adapter";
import { SearchApp } from "./searchApp/searchApp";

// Create HTTP server.
const server = restify.createServer();

// Enable the ability to parse incoming content as JSON in req.body
server.use(restify.plugins.bodyParser());

server.listen(process.env.port || process.env.PORT || 3978, () => {
    console.log(`\nBot Started, ${server.name} listening to ${server.url}`);
});

// Create the bot that will handle incoming messages.
const searchApp = new SearchApp();

// Listen for incoming requests.
server.post("/api/messages", async (req, res) => {
  // Route the incoming request to the adapter for processing.
  await adapter.process(req, res, async (context) => {
    // Route to bot's activity processing logic.
    await searchApp.run(context);
  });
});