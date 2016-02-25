using System.Collections.Generic;
using Newtonsoft.Json;

namespace FacebookSDK.Models
{
    public class FBFriends
    {
        [JsonProperty("data")]
        public List<FBUser> Friends { get; set; }
    }
}
