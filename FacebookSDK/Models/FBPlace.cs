namespace FacebookSDK.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides a strongly-typed representation of a Facebook Place as defined by the Graph API.
    /// </summary>
    /// <remarks>
    /// The GraphPlace class represents the most commonly used properties of a Facebook Place object.
    /// </remarks>
    public class FBPlace
    {
        /// <summary>
        /// Initializes a new instance of the GraphPlace class.
        /// </summary>
        public FBPlace()
        {
            ProfilePicture = new FBPicture();
            Location = new FBLocation();
        }
        
        /// <summary>
        /// Gets or sets the ID of the place.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the place.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the location of the place.
        /// </summary>
        [JsonProperty("location")]
        public FBLocation Location { get; set; }

        /// <summary>
        /// Gets or sets the URL of the place's profile picture.
        /// </summary>
        [JsonProperty("picture")]
        public FBPicture ProfilePicture { get; set; }
    }
}
