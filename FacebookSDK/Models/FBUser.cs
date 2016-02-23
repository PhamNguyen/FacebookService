namespace FacebookSDK.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides a strongly-typed representation of a Facebook User as defined by the Graph API.
    /// </summary>
    /// <remarks>
    /// The GraphUser class represents the most commonly used properties of a Facebook User object.
    /// </remarks>
    public class FBUser
    {
        /// <summary>
        /// Initializes a new instance of the GraphUser class.
        /// </summary>
        public FBUser()
        {
            Location = new FBLocation();
            ProfilePicture = new FBPicture();
        }

        /// <summary>
        /// Gets or sets the ID of the user.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the about of the user.
        /// </summary>
        [JsonProperty("about")]
        public string About { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the email of the user.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the Facebook username of the user.
        /// </summary>
        [JsonProperty("username")]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the gender of the user.
        /// </summary>
        [JsonProperty("gender")]
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the is_verified of the user.
        /// </summary>
        [JsonProperty("is_verified")]
        public string IsVerified { get; set; }

        /// <summary>
        /// Gets or sets the verified of the user.
        /// </summary>
        [JsonProperty("verified")]
        public string Verified { get; set; }

        /// <summary>
        /// Gets or sets the quotes of the user.
        /// </summary>
        [JsonProperty("quotes")]
        public string Quotes { get; set; }

        /// <summary>
        /// Gets or sets the middle name of the user.
        /// </summary>
        [JsonProperty("middle_name")]
        public string MiddleName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        [JsonProperty("last_name")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the name_format of the user.
        /// </summary>
        [JsonProperty("name_format")]
        public string NameFormat { get; set; }

        /// <summary>
        /// Gets or sets the bio of the user.
        /// </summary>
        [JsonProperty("bio")]
        public string Bio { get; set; }

        /// <summary>
        /// Gets or sets the birthday of the user.
        /// </summary>
        [JsonProperty("birthday")]
        public string Birthday { get; set; }

        /// <summary>
        /// Gets or sets the Facebook URL of the user.
        /// </summary>
        [JsonProperty("link")]
        public string Link { get; set; }

        /// <summary>
        /// Gets or sets the current city of the user.
        /// </summary>
        [JsonProperty("location")]
        public FBLocation Location { get; set; }

        /// <summary>
        /// Gets or sets the URL of the user's profile picture.
        /// </summary>
        [JsonProperty("picture")]
        public FBPicture ProfilePicture { get; set; }
    }
}
