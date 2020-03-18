﻿// Code generated by Microsoft (R) AutoRest Code Generator 0.16.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Harrison314.PassiveMQ.Client.Models
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;

    public partial class MessageCrateReqDto
    {
        /// <summary>
        /// Initializes a new instance of the MessageCrateReqDto class.
        /// </summary>
        public MessageCrateReqDto() { }

        /// <summary>
        /// Initializes a new instance of the MessageCrateReqDto class.
        /// </summary>
        public MessageCrateReqDto(string content, string label = default(string))
        {
            Label = label;
            Content = content;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            if (Content == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Content");
            }
        }
    }
}