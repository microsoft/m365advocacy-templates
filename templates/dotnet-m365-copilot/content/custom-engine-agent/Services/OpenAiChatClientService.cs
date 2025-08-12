using OpenAI.Chat;

namespace CustomEngineAgent.Services;

/// <summary>
/// OpenAI implementation of the chat client service
/// </summary>
public class OpenAiChatClientService : IChatClientService
{
    private readonly ChatClient _chatClient;

    public OpenAiChatClientService(ChatClient chatClient)
    {
        _chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
    }

    public IAsyncEnumerable<StreamingChatCompletionUpdate> CompleteChatStreamingAsync(
        IEnumerable<ChatMessage> messages, 
        CancellationToken cancellationToken = default)
    {
        return _chatClient.CompleteChatStreamingAsync(messages, cancellationToken: cancellationToken);
    }
}