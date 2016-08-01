using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebIcomApi.Entidades
{
    public class clsMensajeChat
    {
        public long idmensaje { get; set; }
        public int idusuario { get; set; }
        public string fecha { get; set; }
        public string hora { get; set; }
        public string mensaje { get; set; }
        public string nombre { get; set; }
        public string iniciales { get; set; }
        
    }
}