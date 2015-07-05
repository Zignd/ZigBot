using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZigBot.Types
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ReplyKeyboardMarkup
    {
        [JsonProperty("keyboard")]
        public List<List<string>> Keyboard { get; set; }

        [JsonProperty("resize_keyboard")]
        public bool? ResizeKeyboard { get; set; }

        [JsonProperty("one_time_keyboard")]
        public bool? OneTimeKeyboard { get; set; }

        [JsonProperty("selective")]
        public bool? Selective { get; set; }
    }
}
