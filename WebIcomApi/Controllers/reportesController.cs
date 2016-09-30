using DAOicom;
using DAOicom.Helpers;
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
    [RoutePrefix("reportes")]
    public class reportesController : ApiController
    {
        [Authorize]
        [HttpPost]
        [Route("getFolioReporte")]
        public Object getFolioReporte()
        {
            reportesHelper objrephelp = new reportesHelper();
            String folio = objrephelp.getFolioReporte();

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
        [Route("getTipoFallas")]
        public Object getTipoFallas()
        {
            reportesHelper objrephelp = new reportesHelper();
            List<tipofallas> lsttipofallas = objrephelp.getTipoFallas();

            if (lsttipofallas.Count == 0)
            {
                clsError objerr = new clsError();
                objerr.error = "No se han Encontrado tipo de fallas";
                objerr.result = 0;
                return objerr;
            }
            else
            {
                List<clsTipoFallas> lst = new List<clsTipoFallas>();
                foreach (tipofallas t in lsttipofallas)
                {
                    clsTipoFallas objtipofalla = new clsTipoFallas();
                    objtipofalla.idtipofalla = t.idtipofalla;
                    objtipofalla.nombre = t.nombre;
                    objtipofalla.descripcion = t.descripcion;
                    lst.Add(objtipofalla);
                }
                
                return lst;
            }
        }

        [Authorize]
        [HttpPost]
        [Route("getTipoMntos")]
        public Object getTipoMntos()
        {
            tiposMntoHelper objtmhelp = new tiposMntoHelper();
            List<tipomantenimientos> lsttipomntos = objtmhelp.getTipoMntos();

            if (lsttipomntos.Count == 0)
            {
                clsError objerr = new clsError();
                objerr.error = "No se han Encontrado tipos de mantenimiento";
                objerr.result = 0;
                return objerr;
            }
            else
            {
                List<clsTipoMnto> lst = new List<clsTipoMnto>();
                foreach (tipomantenimientos t in lsttipomntos)
                {
                    clsTipoMnto objtipomnto = new clsTipoMnto();
                    objtipomnto.idtipomnto = t.idtipomto;
                    objtipomnto.nombre = t.nombre;
                    objtipomnto.descripcion = t.descripcion;
                    lst.Add(objtipomnto);
                }

                return lst;
            }
        }

        [Authorize]
        [HttpPost]
        [Route("GuardarReporteOperador")]
        public Object GuardarReporteOperador(JObject json)
        {
            reportesHelper objrephelp = new reportesHelper();
            String no_serie = json["noserie"].ToString();
            String kmho = json["kmho"].ToString();
            String modelo = json["modelo"].ToString();
            String idreporto = json["idreporto"].ToString();
            String idtipofalla = json["idtipofalla"].ToString();
            String idatiende = json["idatiende"].ToString();
            String descripcion = json["descripcion"].ToString();            

            reportes objrep = new reportes();            

            objrep.no_serie = no_serie;
            objrep.km_horometro = System.Convert.ToDecimal(kmho, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
            objrep.modelo = Int32.Parse(modelo);
            objrep.idreporto = Int32.Parse(idreporto);
            objrep.idtipofalla = Int32.Parse(idtipofalla);
            objrep.idatiende = Int32.Parse(idatiende);
            objrep.descripcion = descripcion;
            objrep.idstatus = 1;
            objrep.fecha = DateTime.Now;
            objrep.hora = DateTime.Now.TimeOfDay;
            
            String folio = objrephelp.insertaReporte(objrep);

            if (folio == "")
            {
                clsError objerr = new clsError();
                objerr.error = "No ha podido insertar el reporte, notifiquelo a su administrador TI";
                objerr.result = 0;
                return objerr;
            }
            else
            {
                clsInsertaRep objinrep = new clsInsertaRep();
                objinrep.folio = folio;
                return objinrep;
            }
            
        }

        [Authorize]
        [HttpPost]
        [Route("GuardarReporteServicio")]
        public Object GuardarReporteServicio(JObject json)
        {

            reportesHelper objrephelp = new reportesHelper();

            int folio = Int32.Parse(json["folio"].ToString());

            reportes objrep = objrephelp.getReporteByFolio(folio);

            String kmho = json["kmho"].ToString();
            String idrealizo = json["idrealizo"].ToString();
            String tiemporeparacion = json["tiemporeparacion"].ToString();
            String retraso = json["retraso"].ToString();
            String idtipomnto = json["idtipomnto"].ToString();
            String idtipofalla = json["idtipofalla"].ToString();
            String observaciones = json["observaciones"].ToString();


            objrep.km_horometro = System.Convert.ToDecimal(kmho, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
            objrep.idrealizo = Int32.Parse(idrealizo);
            objrep.tiempo_reparacion = Int32.Parse(tiemporeparacion);
            objrep.retraso = Byte.Parse(retraso);
            objrep.idtipomto = Int32.Parse(idtipomnto);
            objrep.observaciones = observaciones;
            objrep.idtipofalla = Int32.Parse(idtipofalla);
            objrep.idstatus = 2;


            JArray jrefs = JArray.Parse(json["refacciones"].ToString());

            List<refacciones_reporte> lstref = new List<refacciones_reporte>();
            int contref = 1;
            foreach (var jref in jrefs)
            {
                JObject jobj = (JObject)jref;
                refacciones_reporte objref = new refacciones_reporte();
                objref.folio_reporte = folio;
                objref.no_refaccion = contref;
                objref.nombre = jobj["nombre_refaccion"].ToString();

                lstref.Add(objref);
                contref++;

            }

            String resp = objrephelp.GuardaReporteServicios(objrep, lstref);

            if (resp != "")
            {
                clsError objerr = new clsError();
                objerr.error = "Error al guardar el reporte " + resp;
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
        [Route("getReporteOperador")]
        public Object getReporteOperador(JObject json)
        {
            String noserie = json["noserie"].ToString();

            reportesHelper objrephelp = new reportesHelper();
            reportes rep = objrephelp.getReportByNoSerie(noserie);

            if (rep == null)
            {
                clsError objerr = new clsError();
                objerr.error = "No ha podido encontrar el reporte para la unidad, verifiquelo con su Administrador de TI";
                objerr.result = 0;
                return objerr;
            }
            else
            {
                usuariosHelper objushelp = new usuariosHelper();
                usuarios atiende = objushelp.getUsuarioByID((int)rep.idatiende);
                usuarios reporto = objushelp.getUsuarioByID((int)rep.idreporto);

                String stratiende = "";
                String strreporto = "";
                if (atiende != null)
                {
                    stratiende = atiende.nombre + " " + atiende.apepaterno + " " + atiende.apematerno;
                }

                if (reporto != null)
                {
                    strreporto = reporto.nombre + " " + reporto.apepaterno + " " + reporto.apematerno;
                }

                maquinasHelper maqhelp = new maquinasHelper();
                maquinas maq = maqhelp.getMaquinaByNoSerie(rep.no_serie);

                if (maq == null)
                {
                    clsError objerr = new clsError();
                    objerr.error = "No ha podido encontrar los datos de la maquina para el reporte, verifiquelo con su Administrador de TI";
                    objerr.result = 0;
                    return objerr;
                }

                tipoFallasHelper tfhelp = new tipoFallasHelper();
                tipofallas tf = tfhelp.getTipoFallasById((int)rep.idtipofalla);


                clsReporteOperador objrepop = new clsReporteOperador();
                objrepop.folio = rep.folio;
                DateTime dtfecha = (DateTime)rep.fecha;
                String fecha = dtfecha.Year + "-" + dtfecha.Month + "-" + dtfecha.Day;
                String hora = rep.hora.ToString();
                objrepop.fechahora = fecha + " " + hora;
                objrepop.equipo = maq.noeconomico.ToString();
                objrepop.noserie = maq.noserie;
                objrepop.kmho = rep.km_horometro.ToString();
                objrepop.modelo = rep.modelo.ToString();
                objrepop.reporto =strreporto;
                objrepop.tipofalla = tf.nombre;
                objrepop.idtipofalla = tf.idtipofalla.ToString();
                objrepop.atiende = stratiende;
                objrepop.descripcion = rep.descripcion;

                return objrepop;


            }

           
        }

        [Authorize]
        [HttpPost]
        [Route("getReporteProduccion")]
        public Object getReporteProduccion(JObject json)
        {
            String folio = json["folio"].ToString();
            String material = json["material"].ToString();
            String strcantidad = json["cantidad"].ToString();
            String unidad = json["unidad"].ToString();
            String cliente = json["cliente"].ToString();
            String strfecha = json["fecha"].ToString();
            String strfechafin = json["fechafin"].ToString();

            DateTime? fecha = !strfecha.Equals("") ? DateTime.ParseExact(strfecha, "yyyy-MM-dd", System.Globalization.CultureInfo.InstalledUICulture) : (DateTime?)null;
            DateTime? fechafin = !strfechafin.Equals("") ? DateTime.ParseExact(strfechafin, "yyyy-MM-dd", System.Globalization.CultureInfo.InstalledUICulture) : (DateTime?)null;
            Decimal cantidad = !strcantidad.Equals("") ? Decimal.Parse(strcantidad): -1;

            produccionHelper phelp = new produccionHelper();
            List<produccion> lstp;

            if(folio.Equals("") && material.Equals("") && strcantidad.Equals("") && unidad.Equals("") && cliente.Equals("") && strfecha.Equals("")){
                lstp = phelp.getTodasproduccion();
            }else{
                lstp = phelp.getProduccionByFiltros(folio, material, unidad, cantidad, cliente, fecha, fechafin);
            }

            if (lstp.Count() <= 0) {
                clsError objerr = new clsError();
                objerr.error = "No se encontraron datos para mostrar";
                objerr.result = 0;
                return objerr;
            }

            List<clsProduccion> lstresp = new List<clsProduccion>();
            foreach (produccion i in lstp) {
                clsProduccion p = new clsProduccion();
                p.folio = i.folio;
                p.material = i.material;
                p.cantidad = i.cantidad.ToString();
                p.undiad = i.unidad;
                p.cliente = i.cliente;

                DateTime fech = (DateTime)i.fecha;
                String mes = fech.Month.ToString().Length < 2 ? "0" + fech.Month.ToString() : fech.Month.ToString();
                String dia = fech.Day.ToString().Length < 2 ? "0" + fech.Day.ToString() : fech.Day.ToString();
                String strFech = fech.Year.ToString() + "-" + mes + "-" + dia;

                p.fecha = strFech;

                lstresp.Add(p);
            }

            return lstresp;


            
        }

        
    }
}
