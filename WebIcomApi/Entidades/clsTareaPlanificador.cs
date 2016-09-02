using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebIcomApi.Entidades
{
    public class clsTareaPlanificador
    {
        public long idtarea { get; set; }
        public clsCategoriasPlanificador categoria { get; set; }
        public String titulo { get; set; }
        public Boolean todoDia { get; set; }
        public String fechahorainicio { get; set; }        
        public String fechahorafin { get; set; }        
        public double porcentaje { get; set; }
        public double horas { get; set; }
        public String notas { get; set; }
    }
}