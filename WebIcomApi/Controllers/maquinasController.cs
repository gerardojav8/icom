using DAOicom;
using DAOicom.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        [Route("getFolioSolicitud")]
        public Object getFolioSolicitud()
        {
            solicitudMaquinariaHelper objsmhelp = new solicitudMaquinariaHelper();
            String folio = objsmhelp.getFolioSolicitud();

            if (folio == "")
            {
                clsError objerr = new clsError();
                objerr.error = "No se ha generado el folio";
                objerr.result = 0;
                return objerr;
            }
            else
            {
                clsEntidadFolio objfol = new clsEntidadFolio();
                objfol.folio = folio;
                return objfol;
            }
        }

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

        [Authorize]
        [HttpPost]
        [Route("tieneReporte")]
        public Object tieneReporte(JObject json)
        {
            String noserie = json["noserie"].ToString();
            reportesHelper objrephelp = new reportesHelper();
            int tieneReporte = objrephelp.tieneMaquinaReporte(noserie);

            Dictionary<string, int> resp = new Dictionary<string, int>();
            resp.Add("tieneReporte", tieneReporte);

            return resp;
        }

        [Authorize]
        [HttpPost]
        [Route("GuardarSolicitudMaquinaria")]
        public Object GuardarSolicitudMaquinaria(JObject json)
        {

            solicitudMaquinariaHelper objsmhelp = new solicitudMaquinariaHelper();

            int folio = Int32.Parse(objsmhelp.getFolioSolicitud());

            solicitudmaquinaria objsm = new solicitudmaquinaria();
            
            String fecha = json["fecha"].ToString();
            String time = json["time"].ToString();
            String requeridopara = json["requeridopara"].ToString();
            String idareaobra = json["idareaobra"].ToString();
            String idresponsable = json["idresponsable"].ToString();

            String strFechaHora = fecha + " " + time;
            DateTime fechahora = DateTime.ParseExact( strFechaHora, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InstalledUICulture);
            objsm.folio = folio;
            objsm.fecha = fechahora.Date;
            objsm.time = fechahora.TimeOfDay;
                      

            JArray jlstreq = JArray.Parse(json["requermientos"].ToString());

            List<requerimientos_solicitudes> lstreq = new List<requerimientos_solicitudes>();
            int contreq = 1;
            foreach (var jref in jlstreq)
            {
                JObject jobj = (JObject)jref;
                requerimientos_solicitudes objreq = new requerimientos_solicitudes();
                objreq.foliosolicitud = folio;
                objreq.norequerimiento = contreq;
                objreq.equipo = jobj["equipo"].ToString();
                objreq.marca = jobj["marca"].ToString();
                objreq.modelo = jobj["modelo"].ToString();

                lstreq.Add(objreq);
                contreq++;

            }

            String resp = objsmhelp.GuardaSolicitud(objsm, lstreq);

            if (resp != "")
            {
                clsError objerr = new clsError();
                objerr.error = "Error al guardar la solicitud " + resp;
                objerr.result = 0;
                return objerr;
            }
            else
            {
                Dictionary<String, String> dresp = new Dictionary<string, string>();
                dresp.Add("respuesta", "exito");
                return dresp;
            }

        }

        

    }
}
