using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrackingApp.Models
{
    public class State
    {
        [JsonProperty(PropertyName = "Codigo")]
        public int StateId { get; set; }

        [JsonProperty(PropertyName = "Descripcion")]
        public string StateName { get; set; }

    }
}
