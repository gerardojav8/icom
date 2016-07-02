using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebIcomApi.Entidades
{
    public class clsEstadosFisicos
    {
        public string nombre { get; set; }
        public int idcomponente { get; set; }
        public String marca { get; set; }
        public String  tipo { get; set; }
        public String capacidad { get; set; }
        public String calificacion { get; set; }
        public String comentario { get; set; }

    }
}