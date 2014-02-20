using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BitsmackGTAPI.Models
{
    public class TogglData
    {
    }

    public class Task
    {
        [JsonProperty(PropertyName = "start")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Start { get; set; }

        [JsonProperty(PropertyName = "stop")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Stop { get; set; }

        [JsonProperty(PropertyName = "duration")]
        public int Duration { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "project")]
        public Project Project { get; set; }
    }

    public class Project
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "client_project_name")]
        public string Client { get; set; }
    }
}