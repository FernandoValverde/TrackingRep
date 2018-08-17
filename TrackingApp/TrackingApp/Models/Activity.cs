using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrackingApp.Models
{
    public class Activity
    {
        [JsonProperty(PropertyName = "IdActividad")]
        public int ActivityId { get; set; }

        [JsonProperty(PropertyName = "Detalle")]
        public string Detail { get; set; }

        [JsonProperty(PropertyName = "FechaInicio")]
        public DateTime? StartDate { get; set; }

        [JsonProperty(PropertyName = "FechaFin")]
        public DateTime? EndDate { get; set; }

        [JsonProperty(PropertyName = "FechaReversion")]
        public DateTime? ReversionDate { get; set; }
        
        [JsonProperty(PropertyName = "Esfuerzo")]
        public double? EffortTime { get; set; }

        [JsonProperty(PropertyName = "ClasificacionActividad")]
        public ActivityClasification ActivityClasification { get; set; }

        [JsonProperty(PropertyName = "TipoActividad")]
        public ActivityType ActivityType { get; set; }
        
        [JsonProperty(PropertyName = "FechaUltimaModificacion")]
        public DateTime? LastUpDate { get; set; }

        [JsonProperty(PropertyName = "UsuarioUltimaModificacion")]
        public string LastUpdateUser { get; set; }

        [JsonProperty(PropertyName = "ConsecutivoRecurso")]
        public string Action { get; set; }

        [JsonProperty(PropertyName = "Recurso")]
        public User Resource { get; set; }

        [JsonProperty(PropertyName = "ParametroGeneralDias")]
        public int GeneralParameterDays { get; set; }

        [JsonProperty(PropertyName = "Proyecto")]
        public Project Project { get; set; }


    }
}
