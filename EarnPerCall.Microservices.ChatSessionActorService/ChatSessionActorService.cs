using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using EarnPerCall.Microservices.ChatSessionActorService.Interfaces;

namespace EarnPerCall.Microservices.ChatSessionActorService
{

    [StatePersistence(StatePersistence.Persisted)]
    internal class ChatSessionActorService : Actor, IChatSessionActorService, IRemindable
    {
        public ChatSessionActorService(ActorService actorService, ActorId actorId) 
            : base(actorService, actorId)
        {
        }

        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            return Task.CompletedTask;
        }

        private static string PauseReminderName = "PauseChatSessionReminderName";

        public async Task InitChatSession(int advisorId, int customerId)
        {
            await StateManager.AddStateAsync("advisorId", advisorId);
            await StateManager.AddStateAsync("customerId", customerId);
        }

        public async Task SetPauseReminder(DateTime dateTime)
        {
            var resumeAfter = dateTime - DateTime.UtcNow;

            ActorEventSource.Current.ActorMessage(this, $"Session will pause in {resumeAfter}");

            await RegisterReminderAsync(PauseReminderName, null, resumeAfter, TimeSpan.MaxValue);
        }

        public async Task ClearPauseReminder()
        {
            try
            {
                await UnregisterReminderAsync(GetReminder(PauseReminderName));
                ActorEventSource.Current.ActorMessage(this, "UnregisterReminderAsync successfully called");
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorMessage(this, $"Exception during UnregisterReminderAsync call {e.Message}");
                throw;
            }
        }

        public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            if (reminderName == PauseReminderName)
            {
                await NotifyAboutPause();
            }
        }

        private async Task NotifyAboutPause()
        {
            var advisorId = await StateManager.TryGetStateAsync<int>("advisorId");
            var customerId = await StateManager.TryGetStateAsync<int>("customerId");

            if (!advisorId.HasValue || !customerId.HasValue)
            {
                ActorEventSource.Current.ActorMessage(this, $"Can't Get details for advisorId or customerId");
                return;
            }

            string advisorChannelName = $"App.User.{advisorId.Value}";
            string customerChannelName = $"App.User.{customerId.Value}";
            string message = $"Pause|{Id}";

            var publicClient = PubnubClient.GetInstance();

            publicClient.Send(advisorChannelName, message);
            publicClient.Send(customerChannelName, message);

            ActorEventSource.Current.ActorMessage(this,
                $"Send 'Session Paused' event to {customerChannelName} and {advisorChannelName}" +
                $" for session with sessionId = {Id}");
        }
    }
}