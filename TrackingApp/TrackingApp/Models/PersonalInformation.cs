using Newtonsoft.Json;
using SQLite;

namespace TrackingApp.Models
{
    public class PersonalInformation
    {
        [PrimaryKey]
        public int UserId { get; set; }

        [JsonProperty(PropertyName = "NombreCompleto")]
        public string FullName { get; set; }

        [JsonProperty(PropertyName = "TelefonoTrabajo")]
        public string OfficePhone { get; set; }

        [JsonProperty(PropertyName = "FechaIngresoString")]
        public string AdmissionDate { get; set; }

        [JsonProperty(PropertyName = "Activo")]
        public bool? State { get; set; }

        [JsonProperty(PropertyName = "Categoria")]
        public Category Category { get; set; }

        [JsonProperty(PropertyName = "Oficina")]
        public Office Office { get; set; }
    }
}

public class Category
{
    [JsonProperty(PropertyName = "Descripcion")]
    public string Description { get; set; }
}

public class Office
{
    [JsonProperty(PropertyName = "Nombre")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "Direccion")]
    public string Direction { get; set; }
}

//public class Fotografia
//{
//    public object ImagenFotografia { get; set; }
//    public object FotografiaImg { get; set; }
//    public string UrlFotografia { get; set; }
//}

