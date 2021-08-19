using System;
using System.Collections.Generic;
using System.Diagnostics;
using PubnubApi;

namespace EarnPerCall.Microservices.ChatSessionActorService
{
    public class PubnubClient
    {
        private readonly Pubnub _pubnub;
        private bool _inited = false;

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
            pnConfiguration.SubscribeKey = "sub-c-[get from config]";
            pnConfiguration.PublishKey = "pub-c-[get from config]";
            pnConfiguration.Uuid = "myUniqueUUID";
            _pubnub = new Pubnub(pnConfiguration);

            _pubnub.AddListener(new SubscribeCallbackExt(
                delegate (Pubnub pnObj, PNMessageResult<object> pubMsg)
                {
                    if (pubMsg != null)
                    {
                        Debug.WriteLine("In Example, SubscribeCallback received PNMessageResult");
                        Debug.WriteLine("In Example, SubscribeCallback messsage channel = " + pubMsg.Channel);
                        Debug.WriteLine("In Example, SubscribeCallback messsage channelGroup = " + pubMsg.Subscription);
                        Debug.WriteLine("In Example, SubscribeCallback messsage publishTimetoken = " + pubMsg.Timetoken);
                        Debug.WriteLine("In Example, SubscribeCallback messsage publisher = " + pubMsg.Publisher);
                        string jsonString = pubMsg.Message.ToString();
                        Dictionary<string, string> msg = _pubnub.JsonPluggableLibrary.DeserializeToObject<Dictionary<string, string>>(jsonString);
                        Debug.WriteLine("msg: " + msg["msg"]);
                    }
                },
                delegate (Pubnub pnObj, PNPresenceEventResult presenceEvnt)
                {
                    if (presenceEvnt != null)
                    {
                        Debug.WriteLine("In Example, SubscribeCallback received PNPresenceEventResult");
                        Debug.WriteLine(presenceEvnt.Channel + " " + presenceEvnt.Occupancy + " " + presenceEvnt.Event);
                    }
                },
                delegate (Pubnub pnObj, PNStatus pnStatus)
                {
                    if (pnStatus.Category == PNStatusCategory.PNConnectedCategory)
                    {
                        _inited = true;
                    }
                })
            );
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
