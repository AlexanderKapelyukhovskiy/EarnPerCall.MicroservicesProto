using Newtonsoft.Json;
using System;

namespace EarnPerCall.Microservices.Api.Models
{

    public class ApiChatSessionPauseModel
    {
        [JsonProperty("pauseTime")]
        public DateTime PauseTime { get; set; }
    }
}
