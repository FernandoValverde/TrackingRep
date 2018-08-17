using Newtonsoft.Json;

namespace TrackingApp.Models
{
    public class PersonalState
    {
        [JsonProperty(PropertyName = "Clase")]
        public string AttributeClass { get; set; }

        [JsonProperty(PropertyName = "Titulo")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "ToolTip")]
        public string Attribute { get; set; }

        [JsonProperty(PropertyName = "Total")]
        public double Value { get; set; }

        [JsonIgnore]
        public string Color { get; set; }
    }
}
