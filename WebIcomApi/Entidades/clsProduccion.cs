using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebIcomApi.Entidades
{
    public class clsProduccion
    {
        public string idproduccion { get; set; }
        public string folio { get; set; }
        public string material { get; set; }
        public string cantidad { get; set; }
        public string unidad { get; set; }
        public string cliente { get; set; }
        public string fecha { get; set; }
    }
}