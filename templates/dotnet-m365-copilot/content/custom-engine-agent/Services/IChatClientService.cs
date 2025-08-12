using OpenAI.Chat;

namespace CustomEngineAgent.Services;

/// <summary>
/// Interface for chat client services that abstracts OpenAI and Azure OpenAI implementations
/// </summary>
public interface IChatClientService
{
    /// <summary>
    /// Completes a chat conversation with streaming response
    /// </summary>
    /// <param name="messages">The conversation messages</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A streaming response</returns>
    IAsyncEnumerable<StreamingChatCompletionUpdate> CompleteChatStreamingAsync(
        IEnumerable<ChatMessage> messages, 
        CancellationToken cancellationToken = default);
}