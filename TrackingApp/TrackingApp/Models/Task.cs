using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TrackingApp.Classes;

namespace TrackingApp.Models
{
    public class Task
    {
        public Task()
        {
            this.Labors = new List<Labor>();
        }
        [JsonProperty(PropertyName = "IdTarea")]
        public int TaskId { get; set; }

        [JsonProperty(PropertyName = "Nombre")]
        public string TaskName { get; set; }

        [JsonProperty(PropertyName = "Descripcion")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "Detalle")]
        public string Detail { get; set; }

        [JsonProperty(PropertyName = "FechaCreacion")]
        public DateTime? CreationDate { get; set; }

        [JsonProperty(PropertyName = "FechaLimite")]
        public DateTime? LimitDate { get; set; }

        [JsonProperty(PropertyName = "TiempoEstimado")]
        public double EstimationTime { get; set; }

        [JsonProperty(PropertyName = "EsfuerzoAcumulado")]
        public double WorkedTime { get; set; }

        [JsonProperty(PropertyName = "IdEstadoTarea")]
        public int StateId { get; set; }

        [JsonProperty(PropertyName = "NombreEstadoTarea")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "IdNuevoEstadoTarea")]
        public byte NewStateId { get; set; }

        [JsonProperty(PropertyName = "IdPrioridad")]
        public int PriorityId { get; set; }

        [JsonProperty(PropertyName = "NombrePrioridad")]
        public string Priority { get; set; }

        [JsonProperty(PropertyName = "Atrasada")]
        public bool Backward { get; set; }

        [JsonProperty(PropertyName = "labor")]
        public List<Labor> Labors { get; set; }

        [JsonProperty(PropertyName = "Proyecto")]
        public Project Project { get; set; }

        [JsonProperty(PropertyName = "Etapa")]
        public Stage Stage { get; set; }

        [JsonProperty(PropertyName = "Recurso")]
        public User Resource { get; set; }

        [JsonProperty(PropertyName = "ConsecutivoRecurso")]
        public string Action { get; set; }

        [JsonProperty(PropertyName = "FechaUltimaModificacion")]
        public DateTime? LastUpDate { get; set; }

        [JsonProperty(PropertyName = "UsuarioUltimaModificacion")]
        public string LastUpdateUser { get; set; }

        [JsonProperty(PropertyName = "EsRecurrente")]
        public bool IsRecurrent { get; set; }

        [JsonProperty(PropertyName = "MensajeBitacora")]
        public string ReasonMessage { get; set; }

        [JsonProperty(PropertyName = "CantidadLabores")]
        public int LaborsCount { get; set; }

        [JsonIgnore]
        public string ColorState
        {
            get
            {
                var color = "";
                switch (this.StateId)
                {
                    case Constant.Abrir:
                        color = "#737373";//Gris
                        break;
                    case Constant.Iniciar:
                        color = "#153184";//Azul
                        break;
                    case Constant.Suspender:
                        color = "#9a1818";//Rojo
                        break;
                    case Constant.Revision:
                        color = "#d8ce0e";//Amarillo
                        break;
                    default:
                        color = "#d8ce0e";//Amarillo
                        break;
                }
                return color;
            }
        }

        [JsonIgnore]
        public string ColorText
        {
            get
            {
                if (IsRecurrent)
                {
                    return "#8a6d3b";
                }
                else
                {
                    if (LimitDate != null && LimitDate > DateTime.Today)
                    {
                        return "Red";//Rojo
                    }
                }
                return "#000000";
            }
        }

        [JsonIgnore]
        public string StimateColorText
        {
            get
            {
                if (StateId != Constant.Revision && !IsRecurrent && EstimationTime < WorkedTime)
                {
                    return "Red";//Rojo
                }
                return "#000000";
            }
        }

    }

    public class Stage
    {
        [JsonProperty(PropertyName = "Descripcion")]
        public string StageName { get; set; }
    }
}
