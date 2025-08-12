using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace CustomEngineAgent.Services;

/// <summary>
/// Azure OpenAI implementation of the chat client service
/// </summary>
public class AzureOpenAiChatClientService : IChatClientService
{
    private readonly AzureOpenAIClient _client;
    private readonly string _deploymentName;

    public AzureOpenAiChatClientService(AzureOpenAIClient client, string deploymentName)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _deploymentName = deploymentName ?? throw new ArgumentNullException(nameof(deploymentName));
    }

    public IAsyncEnumerable<StreamingChatCompletionUpdate> CompleteChatStreamingAsync(
        IEnumerable<ChatMessage> messages, 
        CancellationToken cancellationToken = default)
    {
        var chatClient = _client.GetChatClient(_deploymentName);
        return chatClient.CompleteChatStreamingAsync(messages, cancellationToken: cancellationToken);
    }
}