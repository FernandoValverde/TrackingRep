using Newtonsoft.Json;
using SQLite;

namespace TrackingApp.Models
{
    public class User
    {
        [PrimaryKey]
        [JsonProperty(PropertyName = "IdRecurso")]
        public int UserId { get; set; }

        [JsonProperty(PropertyName = "Identificacion")]
        public string Identification { get; set; }

        public string UserCode { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
        
        public string FullName { get; set; }
        
        public string OfficePhone { get; set; }

        public string AdmissionDate { get; set; }
        
        public bool? State { get; set; }

        public string Category { get; set; }

        public string Office { get; set; }

        public byte[] Photo { get; set; }

        [JsonProperty(PropertyName = "RegistraTracking")]
        public bool TrackingUse { get; set; }

        [JsonIgnore]
        public bool TrackingRegister { get; set; }

        public override int GetHashCode()
        {
            return UserId;
        }
    }

}
