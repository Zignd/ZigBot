using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZigBot.Types
{
    [JsonObject(MemberSerialization.OptIn)]
    public class UserOrGroupChat
    {
        #region Common for both User and GroupChat

        [JsonProperty("id")]
        public int Id { get; set; }

        #endregion

        #region User specific

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        #endregion

        #region GroupChat specific

        [JsonProperty("title")]
        public string Title { get; set; }

        #endregion
    }
}
