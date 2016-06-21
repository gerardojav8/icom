using DAOicom;
using DAOicom.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebIcomApi.Entidades;

namespace WebIcomApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("maquinas")]
    public class maquinasController : ApiController
    {
        [Authorize]
        [HttpPost]
        [Route("getTodasMaquinas")]
        public Object getTodasMaquinas()
        {
            maquinasHelper maqhelp = new maquinasHelper();

            List<maquinas> lstemaq = maqhelp.getTodasMaquinas();

            if (lstemaq == null)
            {
                clsError objerr = new clsError();
                objerr.error = "No se han encontrado maquinas";
                objerr.result = 0;
                return objerr;
            }
            else
            {
                if (lstemaq.Count() < 1)
                {
                    clsError objerr = new clsError();
                    objerr.error = "No se han encontrado maquinas";
                    objerr.result = 0;
                    return objerr;
                }
                else
                {
                    List<clsMaquinas> lstmaq = new List<clsMaquinas>();
                    foreach (maquinas item in lstemaq)
                    {
                        clsMaquinas objmaq = new clsMaquinas(item);
                        lstmaq.Add(objmaq);
                    }


                    return lstmaq;
                }

            }


        }

        [Authorize]
        [HttpPost]
        [Route("getListadoMaquinas")]
        public Object getListadoMaquinas()
        {
            maquinasHelper maqhelp = new maquinasHelper();

            List<maquinas> lstemaq = maqhelp.getTodasMaquinas();

            if (lstemaq == null)
            {
                clsError objerr = new clsError();
                objerr.error = "No se han encontrado maquinas";
                objerr.result = 0;
                return objerr;
            }
            else
            {
                if (lstemaq.Count() < 1)
                {
                    clsError objerr = new clsError();
                    objerr.error = "No se han encontrado maquinas";
                    objerr.result = 0;
                    return objerr;
                }
                else
                {
                    reportesHelper objrephelp = new reportesHelper();
                    List<clsListaMaquinas> lstmaq = new List<clsListaMaquinas>();
                    foreach (maquinas item in lstemaq)
                    {
                        clsListaMaquinas objmaq = new clsListaMaquinas(item);
                        objmaq.tieneReporte = objrephelp.tieneMaquinaReporte(objmaq.noserie);
                        lstmaq.Add(objmaq);
                    }


                    return lstmaq;
                }

            }
        }

    }
}
