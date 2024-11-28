using Microsoft.Teams.AI.State;

namespace Custom.Engine.Agent.Models
{
    public class AppState : TurnState
    {
        public AppState()
        {
            ScopeDefaults[CONVERSATION_SCOPE] = new ConversationState();
        }

        public new ConversationState Conversation
        {
            get
            {
                TurnStateEntry scope = GetScope(CONVERSATION_SCOPE);

                if (scope == null)
                {
                    throw new ArgumentException("TurnState hasn't been loaded. Call LoadStateAsync() first.");
                }

                return (ConversationState)scope.Value!;
            }
            set
            {
                TurnStateEntry scope = GetScope(CONVERSATION_SCOPE);

                if (scope == null)
                {
                    throw new ArgumentException("TurnState hasn't been loaded. Call LoadStateAsync() first.");
                }

                scope.Replace(value!);
            }
        }
    }

    public class ConversationState : Record
    {
        // Add additional properties here to store and retrieve from the conversation state.
        // For example:

        // public bool Greeted
        // {
        //     get => Get<bool>("greeted");
        //     set => Set("greeted", value);
        // }

        // public Dictionary<string, IList<string>> Lists
        // {
        //     get => Get<Dictionary<string, IList<string>>>("lists") ?? new();
        //     set => Set("lists", value);
        // }
    }
}