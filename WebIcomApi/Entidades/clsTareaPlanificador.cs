using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebIcomApi.Entidades
{
    public class clsTareaPlanificador
    {
        public string idtarea { get; set; }
        public string idcategoria { get; set; }
        public string idobra { get; set; }
        public string nombrecategoria { get; set; }
        public string nombreobra { get; set; }
        public String titulo { get; set; }
        public string todoDia { get; set; }
        public String inicio { get; set; }        
        public String fin { get; set; }        
        public string porcentaje { get; set; }        
        public String notas { get; set; }
    }
}