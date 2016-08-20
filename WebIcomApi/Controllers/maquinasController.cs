using DAOicom;
using DAOicom.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        [Route("ModificaFichaMaquina")]
        public Object ModificaFichaMaquina(JObject json)
        {
            String noserie = json["serie"].ToString();

            maquinasHelper maqhelp = new maquinasHelper();            
            maquinas objmaq = maqhelp.getMaquinaByNoSerie(noserie);
            
            if(objmaq == null) {
                clsError objerr = new clsError();
                objerr.error = "No se ha encontrado la maquina, imposible modificarla";
                objerr.result = 0;
                return objerr;
            }

            String idobra = json["idobraactual"].ToString();
            String imagen = json["imagen"].ToString();

            objmaq.idobra = Int32.Parse(idobra);

            if (imagen != "")
            {
                var bytes = Convert.FromBase64String(imagen);                
                objmaq.imagen = bytes;
            }

            String idequipoaux = json["idequipoaux"].ToString();
            String marcaaux = json["marcaaux"].ToString();
            String modeloaux = json["modeloaux"].ToString();
            String serieaux = json["serieaux"].ToString();
            String tieneequipoaux = json["equipoaux"].ToString();


            equipoauxiliar objeqaux = new equipoauxiliar();

            if (tieneequipoaux.Equals("0"))
            {
                objeqaux.idequipo = -2;
            }
            else
            {

                if (idequipoaux != "")
                {
                    objeqaux.idequipo = Int32.Parse(idequipoaux);
                }
                else
                {
                    objeqaux.idequipo = -1;
                }


                objeqaux.marca = marcaaux;
                objeqaux.modelo = modeloaux;
                objeqaux.noserie = serieaux;
            }

            JArray jefs = JArray.Parse(json["estadosfisicos"].ToString());

            List<mediciones> lstmed = new List<mediciones>();            
            foreach (var jef in jefs)
            {
                JObject jobj = (JObject)jef;
                mediciones objmed = new mediciones();
                objmed.noserie = noserie;
                objmed.idcomponente = Int32.Parse(jobj["idcomponente"].ToString());
                objmed.marca = jobj["marca"].ToString();
                objmed.tipo = jobj["tipo"].ToString();
                objmed.capacidad = jobj["capacidad"].ToString();
                objmed.calificacion = Int32.Parse(jobj["calificacion"].ToString());
                objmed.comentario = jobj["comentario"].ToString();

                lstmed.Add(objmed);                
            }

            JArray jfs = JArray.Parse(json["filtros"].ToString());

            List<medicionesfiltros> lstfil = new List<medicionesfiltros>();
            foreach (var jf in jfs)
            {
                JObject jobj = (JObject)jf;
                medicionesfiltros objmed = new medicionesfiltros();
                objmed.noserie = noserie;
                objmed.idfiltro = Int32.Parse(jobj["idfiltro"].ToString());
                objmed.medicion = Int32.Parse(jobj["medicion"].ToString());
                objmed.comentario = jobj["comentario"].ToString();

                lstfil.Add(objmed);
            }

            String resp = maqhelp.updateMaquina(objmaq, objeqaux, lstmed, lstfil);

            if (resp != "")
            {
                clsError objerr = new clsError();
                objerr.error = resp;
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

        [Authorize]
        [HttpPost]
        [Route("getFichaMaquina")]
        public Object getFichaMaquina(JObject json)
        {

            String noserie = json["noserie"].ToString();

            maquinasHelper maqhelp = new maquinasHelper();

            maquinas objmaquina = maqhelp.getMaquinaByNoSerie(noserie);

            if (objmaquina == null)
            {
                clsError objerr = new clsError();
                objerr.error = "No se ha encontrado la maquina";
                objerr.result = 0;
                return objerr;
            }


            clsFichaMaquina objficha = new clsFichaMaquina();

            objficha.noeco = objmaquina.noeconomico.ToString();
            objficha.descripcion = objmaquina.descripcion;
            objficha.marca = objmaquina.marca;
            objficha.modelo = objmaquina.modelo.ToString();
            objficha.serie = objmaquina.noserie;

            obrasHelper obhelp = new obrasHelper();
            obras ob = obhelp.getobrasById((int)objmaquina.idobra);
            if (ob != null)
            {
                objficha.idobraactual = objmaquina.idobra.ToString();
                objficha.obraactual = ob.nombre;
            }
            else {
                objficha.idobraactual = "-1";
                objficha.obraactual = "";
            }

            if (objmaquina.imagen.Length > 0)
            {
                objficha.imagen = Convert.ToBase64String(objmaquina.imagen);
            }
            else
            {
                objficha.imagen = "";
            }

            EquipoAuxHelper eahelp = new EquipoAuxHelper();

            if (objmaquina.idequipo != null)
            {

                equipoauxiliar objea = eahelp.getequipoauxiliarById((Int32)objmaquina.idequipo);

                if (objea == null)
                {
                    objficha.idequipoaux = -1;
                    objficha.equipoaux = 0;
                    objficha.marcaaux = "";
                    objficha.modelaux = "";
                    objficha.serieaux = "";
                }
                else
                {
                    objficha.idequipoaux = objea.idequipo;
                    objficha.equipoaux = 1;
                    objficha.marcaaux = objea.marca;
                    objficha.modelaux = objea.modelo.ToString();
                    objficha.serieaux = objea.noserie;
                }
            }
            else
            {
                objficha.equipoaux = 0;
                objficha.marcaaux = "";
                objficha.modelaux = "";
                objficha.serieaux = "";

            }

            EstadosFisicoHelper efhelp = new EstadosFisicoHelper();
            componentesHelper comhelp = new componentesHelper();

            List<mediciones> lstmed = efhelp.getMedicionesByNoSerie(objmaquina.noserie);
            List<clsEstadosFisicos> lsef = new List<clsEstadosFisicos>();

            foreach (mediciones med in lstmed)
            {
                clsEstadosFisicos objef = new clsEstadosFisicos();
                componentes objcomp = comhelp.getcomponentesById(med.idcomponente);
                String nombre = "";
                if (objcomp != null)
                {
                    nombre = objcomp.nombre;
                }

                objef.nombre = nombre;
                objef.idcomponente = med.idcomponente;
                objef.marca = med.marca;
                objef.tipo = med.tipo;
                objef.capacidad = med.capacidad;
                objef.comentario = med.comentario;
                objef.calificacion = med.calificacion.ToString();

                lsef.Add(objef);
                
            }

            objficha.estadosfisicos = lsef;

            medicionesFiltrosHelper mfhelp = new medicionesFiltrosHelper();
            filtrosHelper fhelp = new filtrosHelper();

            List<medicionesfiltros> lstmedfil = mfhelp.getTodasmedicionesfiltrosByNoSerie(objmaquina.noserie);
            List<clsFiltros> lstfil = new List<clsFiltros>();

            foreach (medicionesfiltros medfil in lstmedfil)
            {
                clsFiltros objfil = new clsFiltros();
                filtros fil = fhelp.getfiltrosById(medfil.idfiltro);
                String nombre = "";
                if (fil != null)
                {
                    nombre = fil.nombre;
                }

                objfil.nombre = nombre;
                objfil.idfiltro = medfil.idfiltro;
                objfil.medicion = medfil.medicion.ToString();
                objfil.comentario = medfil.comentario;

                lstfil.Add(objfil);

            }

            objficha.filtros = lstfil;

            return objficha;

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
        [Route("getListadoMaquinasBusqueda")]
        public Object getListadoMaquinasBusqueda(JObject json)
        {
            String strbusqueda = json["busqueda"].ToString();

            maquinasHelper maqhelp = new maquinasHelper();

            List<maquinas> lstemaq = maqhelp.getMaquinasByBusqueda(strbusqueda);

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
                        
            String requeridopara = json["requeridopara"].ToString();
            String idareaobra = json["idareaobra"].ToString();
            String idresponsable = json["idresponsable"].ToString();
            
            DateTime dtrequeridopara = DateTime.ParseExact( requeridopara, "yyyy-MM-dd", System.Globalization.CultureInfo.InstalledUICulture);
            
            objsm.folio = folio;            
            objsm.fecha = DateTime.Now;
            objsm.time = DateTime.Now.TimeOfDay;
            objsm.requeridopara = dtrequeridopara;
            objsm.idareaobra = Int32.Parse(idareaobra);
            objsm.idresponsable = Int32.Parse(idresponsable);


            JArray jlstreq = JArray.Parse(json["requerimientos"].ToString());

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
                objreq.cantidad = Int32.Parse(jobj["cantidad"].ToString());

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
