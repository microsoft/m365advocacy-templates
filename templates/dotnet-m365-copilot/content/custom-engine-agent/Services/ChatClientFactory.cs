using Azure;
using Azure.AI.OpenAI;
using OpenAI;
using System.ClientModel;

namespace CustomEngineAgent.Services;

/// <summary>
/// Factory for creating chat client services based on configuration
/// </summary>
public class ChatClientFactory
{
    /// <summary>
    /// Creates a chat client service based on the endpoint URL pattern
    /// </summary>
    /// <param name="configuration">Application configuration</param>
    /// <returns>The appropriate chat client service implementation</returns>
    public static IChatClientService CreateChatClientService(IConfiguration configuration)
    {
        var endpoint = configuration["LanguageModel:Endpoint"];
        var apiKey = configuration["LanguageModel:ApiKey"];
        var model = configuration["LanguageModel:Name"];

        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(model))
        {
            throw new InvalidOperationException("LanguageModel configuration is incomplete. Endpoint, ApiKey, and Name are required.");
        }

        // Check if this is an Azure OpenAI endpoint
        if (IsAzureOpenAiEndpoint(endpoint))
        {
            // For Azure OpenAI we use the deployment name
            return CreateAzureOpenAiService(endpoint, apiKey, model);
        }
        else
        {
            return CreateOpenAiService(endpoint, apiKey, model);
        }
    }

    /// <summary>
    /// Determines if the endpoint is an Azure OpenAI endpoint based on URL pattern
    /// </summary>
    /// <param name="endpoint">The endpoint URL</param>
    /// <returns>True if it's an Azure OpenAI endpoint</returns>
    public static bool IsAzureOpenAiEndpoint(string endpoint)
    {
        if (string.IsNullOrEmpty(endpoint))
            return false;

        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var uri))
        {
            return false;
        }

        var host = uri.Host.ToLowerInvariant();

        return host.Contains("cognitiveservices.azure.com");
    }

    /// <summary>
    /// Creates an Azure OpenAI chat client service
    /// </summary>
    private static IChatClientService CreateAzureOpenAiService(string endpoint, string apiKey, string deploymentName)
    {
        var client = new AzureOpenAIClient(
            new Uri(endpoint),
            new AzureKeyCredential(apiKey));
        
        return new AzureOpenAiChatClientService(client, deploymentName);
    }

    /// <summary>
    /// Creates an OpenAI chat client service
    /// </summary>
    private static IChatClientService CreateOpenAiService(string endpoint, string apiKey, string model)
    {
        var credential = new ApiKeyCredential(apiKey);
        var options = new OpenAIClientOptions()
        {
            Endpoint = new Uri(endpoint)
        };
        
        var chatClient = new OpenAI.Chat.ChatClient(model, credential, options);
        return new OpenAiChatClientService(chatClient);
    }
}