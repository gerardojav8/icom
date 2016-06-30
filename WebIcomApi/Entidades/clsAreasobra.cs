using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebIcomApi.Entidades
{
    
    public class clsAreasobra
    {
        public int idareaobra { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public Decimal latitud { get; set; }
        public Decimal longitud { get; set; }
    }
}