using System;
using System.Diagnostics;
using PubnubApi;

namespace EarnPerCall.Microservices.ChatSessionActorService
{
    public class PubnubClient
    {
        private readonly Pubnub _pubnub;

        private static PubnubClient _pubnubClientInstance;

        public static PubnubClient GetInstance()
        {
            if (_pubnubClientInstance == null)
            {
                _pubnubClientInstance = new PubnubClient();
            }
            return _pubnubClientInstance;
        }

        public PubnubClient()
        {
            PNConfiguration pnConfiguration = new PNConfiguration();
            pnConfiguration.SubscribeKey = "sub-c-***";
            pnConfiguration.PublishKey = "pub-c-***";
            pnConfiguration.Uuid = "myUniqueUUID";
            _pubnub = new Pubnub(pnConfiguration);
        }

        public void Send(string channel, string message)
        {
            _pubnub.Publish()
            .Channel(channel)
            .Message(message)
            .Execute(new PNPublishResultExt((publishResult, publishStatus) =>
            {
                if (!publishStatus.Error)
                {
                    Debug.WriteLine(string.Format("DateTime {0}, In Publish Example, Timetoken: {1}", DateTime.UtcNow, publishResult.Timetoken));
                }
                else
                {
                    Debug.WriteLine(publishStatus.Error);
                    Debug.WriteLine(publishStatus.ErrorData.Information);
                }
            }));
            
        }
    }
}
