using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrackingApp.Models
{
    public class Project
    {
        [JsonProperty(PropertyName = "IdProyecto")]
        [PrimaryKey]
        public int ProjectId { get; set; }

        [JsonProperty(PropertyName = "Nombre")]
        public string ProjectName { get; set; }

        [Ignore]
        [JsonProperty(PropertyName = "EstadoProyecto")]
        public State State { get; set; }

        [Ignore]
        [JsonProperty(PropertyName = "FechaInicio")]
        public DateTime? StartDate { get; set; }

        [Ignore]
        [JsonProperty(PropertyName = "FechaFin")]
        public DateTime? EndDate { get; set; }

        public int? TasksCount{get;set;}

    }
}
