using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebIcomApi.Entidades
{
    public class clsReporteOperador
    {
        public int folio { get; set; }
        public String fechahora { get; set; }
        public String equipo { get; set; }
        public String noserie { get; set; }
        public String kmho { get; set; }
        public String modelo { get; set; }
        public String reporto { get; set; }
        public String tipofalla { get; set; }
        public String atiende { get; set; }
        public String  descripcion { get; set; }
    }
}