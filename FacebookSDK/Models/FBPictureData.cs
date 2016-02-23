using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookSDK.Models
{
    public class FBPictureData
    {
        [JsonProperty("url")]
        public Uri Url { get; set; }
    }
}
