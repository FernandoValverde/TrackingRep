using System;
using System.Collections.Generic;
using System.Text;

namespace TrackingApp.Classes
{
    /// <summary>
    /// Clase Publica encargada de manejar las constantes de la aplicacion
    /// </summary>
    [Serializable]
    public class Constant
    {
        #region Constantes Estados
        public const byte Abrir = 1;
        public const byte Iniciar = 2;
        public const byte Suspender = 3;
        public const byte Finalizar = 4;
        public const byte Cancelar = 5;
        public const byte Revision = 6;
        public const byte OtroUsuario = 6;
        public const byte Otro = 7;
        #endregion

        #region Constantes Colores
        public const int ColorAmarilloTarea = 6;
        public const int ColorVerde = 4;
        public const int ColorRojo = 3;
        public const int ColorGris = 0;
        public const int ColorAzul = 2;
        public const int ColorNegro = 5;
        public const int ColorAmarilloLabor = 1;
        public const int ImagenActividad = 7;
        #endregion

        #region  Constantes Catalogos
        public const int EstadoTarea = 3;
        public const int EstadoLabor = 4;
        public const int Clasificacion = 5;
        public const int Prioridad = 6;
        #endregion

        #region  Constantes Prioridades
        public const int Alta = 1;
        public const int Media = 2;
        public const int Baja = 3;
        #endregion     

        #region  Constantes ClasificacionActividad
        public const int Personal = 1;
        public const int Administrativa = 2;
        #endregion

        #region  Constantes ParametrosGenerales
        public const int LimiteDias = 6;
        #endregion     
    }

    #region Constantes Proyectos
    public sealed class CodigosEstadosProyectos
    {
        public const int SinIniciar = 1;
        public const int EnEjecucion = 2;
        public const int EnGarantia = 3;
        public const int Suspendido = 4;
        public const int Cancelado = 5;
        public const int Finalizado = 6;
    }
    #endregion
}
