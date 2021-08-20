using EarnPerCall.Microservices.ChatSessionActorService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using System;

namespace EarnPerCall.Microservices.Api.Controllers
{
    public class ChatSessionBaseController : ControllerBase
    {
        protected IChatSessionActorService GetActor(int sessionId)
        {
            return ActorProxy.Create<IChatSessionActorService>(
                new ActorId(sessionId),
                new Uri("fabric:/EarnPerCall.Microservices/ChatSessionActorServiceActorService")
            );
        }
    }
}
