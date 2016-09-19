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
        [Route("getObras")]
        public Object getObras()
        {
            obrasHelper obhelp = new obrasHelper();
            List<obras> lstobras = obhelp.getTodasobras();

            if (lstobras.Count == 0)
            {
                clsError objerr = new clsError();
                objerr.error = "No se han Encontrado obras";
                objerr.result = 0;
                return objerr;
            }
            else
            {
                List<clsObras> lst = new List<clsObras>();
                foreach (obras a in lstobras)
                {
                    clsObras objobra = new clsObras();
                    objobra.idobra = a.idobra.ToString();
                    objobra.nombre = a.nombre;
                    objobra.descripcion = a.descripcion;                    
                    lst.Add(objobra);
                }

                return lst;
            }
        }
    }
}
