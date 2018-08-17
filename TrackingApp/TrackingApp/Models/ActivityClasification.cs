using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrackingApp.Models
{
    public class ActivityClasification
    {
        [JsonProperty(PropertyName = "Codigo")]
        public int ActivityClasificationId { get; set; }

        [JsonProperty(PropertyName = "activityClasificationName")]
        public string ActivityClasificationName { get; set; }
    }
}
