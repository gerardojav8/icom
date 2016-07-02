using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebIcomApi.Entidades
{
    public class clsFichaMaquina
    {
        public string noeco { get; set; }
        public string descripcion { get; set; }
        public string marca { get; set; }
        public string modelo { get; set; }
        public string serie { get; set; }
        public string idobraactual { get; set; }
        public string imagen { get; set; }
        public int equipoaux { get; set; }
        public string  marcaaux { get; set; }
        public string modelaux { get; set; }
        public string serieaux { get; set; }

        public List<clsEstadosFisicos> estadosfisicos { get; set; }
        public List<clsFiltros> filtros { get; set; }




    }
}