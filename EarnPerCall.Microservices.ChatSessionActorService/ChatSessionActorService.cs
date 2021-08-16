using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using EarnPerCall.Microservices.ChatSessionActorService.Interfaces;

namespace EarnPerCall.Microservices.ChatSessionActorService
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class ChatSessionActorService : Actor, IChatSessionActorService, IRemindable
    {
        /// <summary>
        /// Initializes a new instance of ChatSessionActorService
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public ChatSessionActorService(ActorService actorService, ActorId actorId) 
            : base(actorService, actorId)
        {
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization

            return this.StateManager.TryAddStateAsync("count", 0);
        }

        private static string PauseChatSessionReminderName = "PauseChatSessionReminderName";

        public async Task StartSession(int sessionId, int availableTime)
        {
            await RegisterReminderAsync(PauseChatSessionReminderName, null, TimeSpan.FromSeconds(availableTime), TimeSpan.MaxValue);
        }

        public async Task StopSession(int sessionId)
        {
            try
            {
                await UnregisterReminderAsync(GetReminder(PauseChatSessionReminderName));
                ActorEventSource.Current.ActorMessage(this, "UnregisterReminderAsync successfully called");
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorMessage(this, $"Exception during UnregisterReminderAsync call {e.Message}");
                throw;
            }
        }

        public Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            ActorEventSource.Current.ActorMessage(this, $"Send 'Session Paused' event to chat session participants");

            return Task.CompletedTask;
        }

    }
}
