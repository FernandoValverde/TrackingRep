using System;
using System.Collections.Generic;
using System.Text;

namespace TrackingApp.Models
{
    public class GeneralParameter
    {
        public int Codigo { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaFinVigencia { get; set; }
        public string FechaFinVigenciaString { get; set; }
        public DateTime? FechaInicioVigencia { get; set; }
        public string FechaInicioVigenciaString { get; set; }
        public DateTime? FechaUltimaModificacion { get; set; }
        public string TipoValor { get; set; }
        public string UsuarioUltimaModificacion { get; set; }
        public string Valor { get; set; }
        public DateTime? ValorDate { get; set; }
    }
}
