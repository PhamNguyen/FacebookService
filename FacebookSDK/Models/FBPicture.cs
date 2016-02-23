using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookSDK.Models
{
    public class FBPicture
    {
        public FBPicture()
        {
            PictureData = new FBPictureData();
        }

        [JsonProperty("data")]
        public FBPictureData PictureData { get; set; }
    }
}
