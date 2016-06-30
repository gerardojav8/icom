using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebIcomApi.Entidades;
using DAOicom.Helpers;
using DAOicom;

namespace WebIcomApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("obras")]
    public class obrasController : ApiController
    {

        [Authorize]
        [HttpPost]
        [Route("getAreasObras")]
        public Object getAreaObras()
        {
            areasObraHelper objaohelp = new areasObraHelper();
            List<areasobra> lstareasobras = objaohelp.getTodasAreasObra();

            if (lstareasobras.Count == 0)
            {
                clsError objerr = new clsError();
                objerr.error = "No se han Encontrado Areas de Obras";
                objerr.result = 0;
                return objerr;
            }
            else
            {
                List<clsAreasobra> lst = new List<clsAreasobra>();
                foreach (areasobra a in lstareasobras)
                {
                    clsAreasobra objareaobra = new clsAreasobra();
                    objareaobra.idareaobra = a.idareaobra;
                    objareaobra.nombre = a.nombre;
                    objareaobra.descripcion = a.descripcion;
                    objareaobra.latitud = (Decimal)a.latitud;
                    objareaobra.longitud = (Decimal)a.longitud;
                    lst.Add(objareaobra);
                }

                return lst;
            }
        }
    }
}
