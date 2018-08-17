using Newtonsoft.Json;
using System;
using TrackingApp.Classes;

namespace TrackingApp.Models
{
    public class Labor
    {
        [JsonProperty(PropertyName = "IdLabor")]
        public int LaborId { get; set; }

        [JsonProperty(PropertyName = "Detalle")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "FechaInicio")]
        public DateTime? StartDate { get; set; }

        [JsonProperty(PropertyName = "EsfuerzoEstimado")]
        public double? EstimationTime { get; set; }

        [JsonProperty(PropertyName = "EsfuerzoAcumulado")]
        public double? WorkedTime { get; set; }

        [JsonProperty(PropertyName = "EstadoLabor")]
        public State State { get; set; }

        [JsonProperty(PropertyName = "IdNuevoEstadoLabor")]
        public byte NewStateId { get; set; }

        [JsonProperty(PropertyName = "Tarea")]
        public Task Task { get; set; }

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

        [JsonProperty(PropertyName = "FechaFinal")]
        public DateTime? EndDate { get; set; }

        [JsonIgnore]
        public string ColorState
        {
            get
            {
                var color = "";
                switch (this.State.StateId)
                {
                    case Constant.Abrir:
                        color = "#d8ce0e";//Amarillo
                        break;
                    case Constant.Iniciar:
                        color = "#153184";//Azul
                        break;
                    case Constant.Suspender:
                        color = "#9a1818";//Rojo
                        break;
                    case Constant.Finalizar:
                        color = "#158415";//Verde
                        break;
                    case Constant.Cancelar:
                        color = "#1a1a1a";//Negro
                        break;
                    default:
                        color = "#d8ce0e";//Amarillo
                        break;
                }
                return color;
            }
        }

    }
}
