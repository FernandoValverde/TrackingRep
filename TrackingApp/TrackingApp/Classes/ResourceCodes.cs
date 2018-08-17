using System;
using System.Collections.Generic;
using System.Text;

namespace TrackingApp.Classes
{
    public static class ResourceCodes
    {
        /*Seguridad para Mantenimiento Labor*/
        public static readonly string DciListarLabor = "CAP415";
        public static readonly string DciAgregarLabor = "CAP416";
        public static readonly string DciEliminarLabor = "CAP417";
        public static readonly string DciModificarLabor = "CAP418";

        /*Seguridad para Mantenimiento Actividad*/
        public static readonly string DciListarActividadRecurso = "CAP419";
        public static readonly string DciAgregarActividad = "CAP420";
        public static readonly string DciEliminarActividad = "CAP421";
        public static readonly string DciModificarActividad = "CAP422";

        /*Seguridad para Estado Personal*/
        public static readonly string DciConsultarEstadoPersonal = "CAP423";

        /*Seguridad para Informacion de Tarea*/
        public static readonly string DciConsultarInformacionTarea = "CAP424";

        /*Seguridad para Actualizar Tarea*/
        public static readonly string DciModificarTarea = "CAP211";

        /*Seguridad para Vistas*/
        public static readonly string DciAgregarVista = "CAP425";
        public static readonly string DciEliminarVista = "CAP426";
    }
}
