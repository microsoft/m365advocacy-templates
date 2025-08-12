using CustomEngineAgent.Services;
using Microsoft.Agents.Builder;
using Microsoft.Agents.Builder.App;
using Microsoft.Agents.Builder.State;
using Microsoft.Agents.Core.Models;
using OpenAI.Chat;
using System.Net.Http.Headers;
using System.Text.Json;

namespace CustomEngineAgent;

public class Bot : AgentApplication, IDisposable
{
    private readonly HttpClient _httpClient = new();
    private readonly IChatClientService _chatClientService;

    public Bot(AgentApplicationOptions options, IChatClientService chatClientService) : base(options) => _chatClientService = chatClientService;

    [Route(RouteType = RouteType.Activity, Type = ActivityTypes.Message, Rank = RouteRank.Last, SignInHandlers = "me")]
    protected async Task OnMessageAsync(ITurnContext turnContext, ITurnState turnState, CancellationToken cancellationToken)
    {
        try
        {
            // Queue an informative update to the user
            await turnContext.StreamingResponse.QueueInformativeUpdateAsync("Working on it...", cancellationToken: cancellationToken);

            // Check if user profile is already cached, if not fetch and cache it
            var userProfile = turnState.Conversation.GetCachedUserProfile();
            if (userProfile == null)
            {
                var accessToken = await UserAuthorization.GetTurnTokenAsync(turnContext, UserAuthorization.DefaultHandlerName, cancellationToken: cancellationToken);
                userProfile = await GetUserProfile(accessToken, cancellationToken);
                turnState.Conversation.SetCachedUserProfile(userProfile);
            }

            // Check if system message is already cached, if not create and cache it
            var systemMessageContent = turnState.Conversation.GetCachedSystemMessage();
            if (systemMessageContent == null)
            {
                systemMessageContent = $"""
You are a helpful assistant. Follow these rules strictly:

1. GREETING: Always greet the user by their first name on the first interaction, then omit their name in subsequent messages.

2. LANGUAGE: Always respond in the user's preferred language: {userProfile?.PreferredLanguage ?? "English"}

3. CITATIONS: When you reference ANY information from the user profile, you MUST add a citation marker [1] immediately after the referenced information.

4. CITATION EXAMPLES:
   - "Hi {userProfile?.GivenName}! [1]" (when using their name)
   - "I see you work in {userProfile?.Department} [1]" (when mentioning their department)
   - "Your role as {userProfile?.JobTitle} [1] at {userProfile?.CompanyName} [1]" (when mentioning job details)

5. USER PROFILE DATA (always cite as [1] when referencing):
   - Name: {userProfile?.GivenName}
   - Display Name: {userProfile?.DisplayName}
   - Job Title: {userProfile?.JobTitle}
   - Department: {userProfile?.Department}
   - Company: {userProfile?.CompanyName}
   - Email: {userProfile?.UserPrincipalName}
   - Preferred Language: {userProfile?.PreferredLanguage}

Remember: ANY time you use information from the user profile above, add [1] immediately after that information.
""";
                // Store system message in conversation history
                turnState.Conversation.AddMessageToHistory("system", systemMessageContent);
            }

            // Get conversation history from state
            var conversationHistory = turnState.Conversation.GetConversationHistory();

            // Build messages list starting with system message
            List<ChatMessage> messages = [new SystemChatMessage(systemMessageContent)];

            // Add conversation history to messages (excluding system messages since we already added the current one)
            foreach (var historyMessage in conversationHistory)
            {
                switch (historyMessage.Role.ToLowerInvariant())
                {
                    case "user":
                        messages.Add(new UserChatMessage(historyMessage.Content));
                        break;
                    case "assistant":
                        messages.Add(new AssistantChatMessage(historyMessage.Content));
                        break;
                    default:
                        break;
                }
            }

            // Add current user message
            var currentUserMessage = turnContext.Activity.Text;
            messages.Add(new UserChatMessage(currentUserMessage));

            // Store current user message in conversation history
            turnState.Conversation.AddMessageToHistory("user", currentUserMessage);

            // Start streaming response using the abstracted service
            var response = _chatClientService.CompleteChatStreamingAsync(messages, cancellationToken: cancellationToken);

            // Collect the assistant's response to store in history
            var assistantResponse = new System.Text.StringBuilder();

            // Process the streaming response
            await foreach (var chunk in response.WithCancellation(cancellationToken))
            {
                if (chunk.ContentUpdate.Count > 0)
                {
                    foreach (var contentPart in chunk.ContentUpdate)
                    {
                        if (!string.IsNullOrEmpty(contentPart.Text))
                        {
                            assistantResponse.Append(contentPart.Text);
                            turnContext.StreamingResponse.QueueTextChunk(contentPart.Text);
                        }
                    }
                }
            }

            // Store assistant's response in conversation history
            var assistantResponseText = assistantResponse.ToString();
            if (!string.IsNullOrEmpty(assistantResponseText))
            {
                turnState.Conversation.AddMessageToHistory("assistant", assistantResponseText);
            }

            // Only add citation if there are citation markers in the response
            if (!string.IsNullOrEmpty(assistantResponseText) && assistantResponseText.Contains("[1]"))
            {
                turnContext.StreamingResponse.EnableGeneratedByAILabel = true;
                var citation = new Citation(
                    $"{userProfile.JobTitle}, {userProfile.Department}, {userProfile.CompanyName}",
                    userProfile.DisplayName,
                    userProfile.ProfileUrl
                );
                turnContext.StreamingResponse.AddCitations([citation]);
            }
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            // For now, we'll queue an error message to the user
            turnContext.StreamingResponse.QueueTextChunk($"I encountered an error while processing your request.");
            turnContext.StreamingResponse.QueueTextChunk($"Exception: {ex.Message}");
        }
        finally
        {
            // Always end the streaming response, regardless of success or failure
            await turnContext.StreamingResponse.EndStreamAsync(cancellationToken);
        }
    }

    [Route(RouteType = RouteType.Message, Type = ActivityTypes.Message, Text = "-reset")]
    protected async Task Reset(ITurnContext turnContext, ITurnState turnState, CancellationToken cancellationToken)
    {
        await UserAuthorization.SignOutUserAsync(turnContext, turnState, "me", cancellationToken: cancellationToken);
        turnState.Conversation.ClearConversationHistory();
        turnState.Conversation.ClearCachedUserProfile();
        await turnContext.SendActivityAsync("Reset complete", cancellationToken: cancellationToken);
    }

    private async Task<UserProfile> GetUserProfile(string accessToken, CancellationToken cancellationToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        HttpResponseMessage response = await _httpClient.GetAsync("https://graph.microsoft.com/v1.0/me?$select=department,jobTitle,preferredLanguage,displayName,givenName,companyName,userPrincipalName,id", cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<UserProfile>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public void Dispose() => _httpClient.Dispose();
}