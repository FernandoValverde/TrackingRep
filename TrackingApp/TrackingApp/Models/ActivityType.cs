using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrackingApp.Models
{
    public class ActivityType
    {
        [JsonProperty(PropertyName = "IdTipoActividad")]
        public int ActivityTypeId { get; set; }

        [JsonProperty(PropertyName = "Nombre")]
        public string ActivityTypeName { get; set; }
    }
}
