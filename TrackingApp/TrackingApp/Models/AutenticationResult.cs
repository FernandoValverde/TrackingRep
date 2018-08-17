
using Newtonsoft.Json;

namespace TrackingApp.Models
{
    public class AutenticationResult
    {
        [JsonProperty(PropertyName = "usuarioValido")]
        public bool ValidUser { get; set; }

        [JsonProperty(PropertyName = "mensajeValidacion")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "NombreCompletoUsuario")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "CodigoUsuario")]
        public string UserCode { get; set; }

        [JsonProperty(PropertyName = "Identificacion")]
        public string Identification { get; set; }
    }
}
