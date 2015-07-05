using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZigBot.Types
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Message
    {
        [JsonProperty("message_id")]
        public int MessageId { get; set; }

        [JsonProperty("from")]
        public User From { get; set; }

        // TODO: Fazer com que identifique quando é User e quando é GroupChat
        [JsonProperty("chat")]
        public UserOrGroupChat Chat { get; set; }

        [JsonProperty("date")]
        public int Date { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
