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
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Net.Mail;

namespace WebIcomApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("maquinas")]
    public class maquinasController : ApiController
    {
        iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
        iTextSharp.text.Font _TitleFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 15, iTextSharp.text.Font.BOLDITALIC, BaseColor.BLACK);

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

        [HttpPost]
        [Route("EliminaMaquinaFromWeb")]
        public Object EliminaMaquinaFromWeb(JObject json)
        {
            try
            {
                String noserie = json["noserie"].ToString();                

                maquinasHelper maqhelp = new maquinasHelper();                

                String resp = maqhelp.deleteMaquina(noserie);

                if (resp.Equals(""))
                {

                    clsError objex = new clsError();
                    objex.error = "Se ha eliminado la Maquina";
                    objex.result = 0;
                    return objex;
                }
                else
                {
                    clsError objerr = new clsError();
                    objerr.error = "Error al eliminar datos " + resp;
                    objerr.result = 0;
                    return objerr;
                }

               
            }
            catch (Exception e)
            {
                clsError objerr = new clsError();
                objerr.error = "Error " + e.ToString();
                objerr.result = 1;
                return objerr;
            }
        }
        
        [HttpPost]
        [Route("GuardaFichaMaquinaFromWeb")]
        public Object GuardaFichaMaquinaFromWeb(JObject json)
        {
            try
            {
                String noserie = json["noserie"].ToString();
                String noeconomico = json["noeconomico"].ToString();
                String marca = json["marca"].ToString();
                String modelo = json["modelo"].ToString();
                String anio = json["anio"].ToString();
                String tipomaquina = json["tipomaquina"].ToString();
                String descripcion = json["descripcion"].ToString();
                String imagen = json["imagen"].ToString();

                maquinasHelper maqhelp = new maquinasHelper();
                maquinas objmaq = new maquinas();

                objmaq.noserie = noserie;
                objmaq.noeconomico = Int32.Parse(noeconomico);
                objmaq.marca = marca;
                objmaq.modelo = modelo;
                objmaq.aniofabricacion = Int32.Parse(anio);
                objmaq.idtipomaquina = Int32.Parse(tipomaquina);
                objmaq.descripcion = descripcion;


                if (imagen != "")
                {
                    var bytes = Convert.FromBase64String(imagen);
                    objmaq.imagen = bytes;
                }

                maqhelp.insertMaquina(objmaq);

                clsError objerr = new clsError();
                objerr.error = "Se ha guardaro la Maquina";
                objerr.result = 0;
                return objerr;
            }
            catch (Exception e) {
                clsError objerr = new clsError();
                objerr.error = "Error " + e.ToString();
                objerr.result = 1;
                return objerr;
            }
        }

        [HttpPost]
        [Route("ModificaMaquinaFromWeb")]
        public Object ModificaMaquinaFromWeb(JObject json)
        {
            try
            {
                String noserie = json["noserie"].ToString();
                String noeconomico = json["noeconomico"].ToString();
                String marca = json["marca"].ToString();
                String modelo = json["modelo"].ToString();
                String anio = json["anio"].ToString();
                String tipomaquina = json["tipomaquina"].ToString();
                String descripcion = json["descripcion"].ToString();
                String imagen = json["imagen"].ToString();

                maquinasHelper maqhelp = new maquinasHelper();
                maquinas objmaq = maqhelp.getMaquinaByNoSerie(noserie);

                if (objmaq == null)
                {
                    clsError obje = new clsError();
                    obje.error = "No se ha encontrado la maquina, imposible modificarla";
                    obje.result = 1;
                    return obje;
                }

                objmaq.noserie = noserie;
                objmaq.noeconomico = Int32.Parse(noeconomico);
                objmaq.marca = marca;
                objmaq.modelo = modelo;
                objmaq.aniofabricacion = Int32.Parse(anio);
                objmaq.idtipomaquina = Int32.Parse(tipomaquina);
                objmaq.descripcion = descripcion;


                if (imagen != "")
                {
                    var bytes = Convert.FromBase64String(imagen);
                    objmaq.imagen = bytes;
                }

                String resp = maqhelp.updateMaquina(objmaq);

                if (resp.Equals(""))
                {

                    clsError objex = new clsError();
                    objex.error = "Se ha guardaro la Maquina";
                    objex.result = 0;
                    return objex;
                }
                else {
                    clsError objerr = new clsError();
                    objerr.error = "Error al actualizar datos " + resp;
                    objerr.result = 0;
                    return objerr;
                }
            }
            catch (Exception e)
            {
                clsError objerr = new clsError();
                objerr.error = "Error " + e.ToString();
                objerr.result = 1;
                return objerr;
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

            if (idobra.Equals("-1"))
            {
                objmaq.idobra = null;
            }
            else
            {
                objmaq.idobra = Int32.Parse(idobra);
            }

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

            objficha.noeco = "";
            objficha.descripcion = "";
            objficha.marca = "";
            objficha.modelo = "";
            objficha.serie = "";

            if(objmaquina.noeconomico != null)
                objficha.noeco = objmaquina.noeconomico.ToString();

            if(objmaquina.descripcion != null)
                objficha.descripcion = objmaquina.descripcion;

            if(objmaquina.marca != null)
                objficha.marca = objmaquina.marca;

            if (objmaquina.modelo != null)
                objficha.modelo = objmaquina.modelo.ToString();

            if(objficha.serie != null)
                objficha.serie = objmaquina.noserie;

            obrasHelper obhelp = new obrasHelper();
            if (objmaquina.idobra != null)
            {
                obras ob = obhelp.getobrasById((int)objmaquina.idobra);
                if (ob != null)
                {
                    objficha.idobraactual = objmaquina.idobra.ToString();
                    objficha.obraactual = ob.nombre;
                }
                else
                {
                    objficha.idobraactual = "-1";
                    objficha.obraactual = "";
                }
            }
            else
            {
                objficha.idobraactual = "-1";
                objficha.obraactual = "";
            }

            if (objmaquina.imagen != null) { 
            if (objmaquina.imagen.Length > 0)
            {
                objficha.imagen = Convert.ToBase64String(objmaquina.imagen);
            }
            else
            {
                objficha.imagen = "";
            }
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

        [HttpPost]
        [Route("getFichaMaquinatoWeb")]
        public Object getFichaMaquinatoWeb(JObject json)
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


            
            Dictionary<String, string> ficha = new Dictionary<string, string>();

            String noeco = "";
            String descripcion = "";
            String marca = "";
            String modelo = "";
            String serie = "";
            String imagen = "";
            String tipomaquina = "";
            String anio = "";
            

            if (objmaquina.noeconomico != null)                
                noeco = objmaquina.noeconomico.ToString();

            if (objmaquina.descripcion != null)
                descripcion = objmaquina.descripcion;

            if (objmaquina.marca != null)
                marca = objmaquina.marca;

            if (objmaquina.modelo != null)
                modelo = objmaquina.modelo.ToString();

            if (objmaquina.noserie != null)
                serie = objmaquina.noserie.ToString();

            if (objmaquina.idtipomaquina != null)
                tipomaquina = objmaquina.idtipomaquina.ToString();

            if (objmaquina.aniofabricacion != null)
                anio = objmaquina.aniofabricacion.ToString();
            

            if (objmaquina.imagen != null)
            {
                if (objmaquina.imagen.Length > 0)
                {
                    imagen = Convert.ToBase64String(objmaquina.imagen);
                }
                else
                {
                    imagen = "";
                }
            }
            else
            {
                imagen = "";
            }


            ficha.Add("noecon", noeco);
            ficha.Add("descripcion", descripcion);
            ficha.Add("marca", marca);
            ficha.Add("noserie", serie);
            ficha.Add("modelo", modelo);
            ficha.Add("tipomaquina", tipomaquina);
            ficha.Add("anio", anio);
            ficha.Add("imagen", imagen);

            return ficha;

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

        
        [HttpPost]
        [Route("getListadoMaquinasToWeb")]
        public Object getListadoMaquinasToWeb()
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
                    List<Dictionary<String, String>> lstmaq = new List<Dictionary<String, String>>();
                    foreach (maquinas item in lstemaq)
                    {
                        Dictionary<String, String> dic = new Dictionary<string, string>();                        
                        dic.Add("noserie", item.noserie);
                        dic.Add("descripcion", item.descripcion);                        
                        lstmaq.Add(dic);
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
            String idobra = json["idobra"].ToString();
            String idresponsable = json["idresponsable"].ToString();
            String idsolicitadopor = json["idsolicitadopor"].ToString();
            
            DateTime dtrequeridopara = DateTime.ParseExact( requeridopara, "yyyy-MM-dd", System.Globalization.CultureInfo.InstalledUICulture);
            
            objsm.folio = folio;            
            objsm.fecha = DateTime.Now;
            objsm.time = DateTime.Now.TimeOfDay;
            objsm.requeridopara = dtrequeridopara;
            objsm.idobra = Int32.Parse(idobra);
            objsm.idresponsable = Int32.Parse(idresponsable);
            objsm.idsolicitadopor = Int32.Parse(idsolicitadopor);


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
           
           
            //MandarMail 

            //Se crea pdf a mandar de la solicitud

            MemoryStream ms = new MemoryStream();

            Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER);
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);
            doc.Open();

            iTextSharp.text.Font _FechaFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            float[] columnWidths = new float[] { 25f, 25f };


            Paragraph titulo = new Paragraph("Solicitud de Maquinaria", _TitleFont);
            titulo.Alignment = Element.ALIGN_CENTER;

            doc.Add(titulo);
            doc.Add(new Paragraph("\n"));
            
            PdfPTable tblReporte = new PdfPTable(2);
            tblReporte.WidthPercentage = 100;

            PdfPCell clConcepto = new PdfPCell(new Phrase("Concepto", _standardFont));
            clConcepto.BorderWidth = 0;
            clConcepto.BorderWidthBottom = 0.75f;

            PdfPCell clValor = new PdfPCell(new Phrase("Valor", _standardFont));
            clValor.BorderWidth = 0;
            clValor.BorderWidthBottom = 0.75f;
            

            tblReporte.AddCell(clConcepto);
            tblReporte.AddCell(clValor);

            //Folio----------------------------------------------
            clConcepto = new PdfPCell(new Phrase("Folio", _standardFont));
            clConcepto.BorderWidth = 0;
            clConcepto.BorderWidthBottom = 0;

            clValor = new PdfPCell(new Phrase(folio.ToString(), _standardFont));
            clValor.BorderWidth = 0;
            clValor.BorderWidthBottom = 0;

            tblReporte.AddCell(clConcepto);
            tblReporte.AddCell(clValor);

            //Fecha----------------------------------------------
            clConcepto = new PdfPCell(new Phrase("Fecha", _standardFont));
            clConcepto.BorderWidth = 0;
            clConcepto.BorderWidthBottom = 0;

            clValor = new PdfPCell(new Phrase(objsm.fecha.ToString(), _standardFont));
            clValor.BorderWidth = 0;
            clValor.BorderWidthBottom = 0;

            tblReporte.AddCell(clConcepto);
            tblReporte.AddCell(clValor);

            //Fecha----------------------------------------------
            clConcepto = new PdfPCell(new Phrase("Requerido para", _standardFont));
            clConcepto.BorderWidth = 0;
            clConcepto.BorderWidthBottom = 0;

            clValor = new PdfPCell(new Phrase(requeridopara, _standardFont));
            clValor.BorderWidth = 0;
            clValor.BorderWidthBottom = 0;

            tblReporte.AddCell(clConcepto);
            tblReporte.AddCell(clValor);

            //Obra----------------------------------------------
            clConcepto = new PdfPCell(new Phrase("Obra", _standardFont));
            clConcepto.BorderWidth = 0;
            clConcepto.BorderWidthBottom = 0;

            obrasHelper obhelp = new obrasHelper();
            obras ob = obhelp.getobrasById(Int32.Parse(idobra));
            String nombreobra = "no encontrado";
            if (ob != null) {
                nombreobra = ob.nombre;
            }

            clValor = new PdfPCell(new Phrase(nombreobra, _standardFont));
            clValor.BorderWidth = 0;
            clValor.BorderWidthBottom = 0;

            tblReporte.AddCell(clConcepto);
            tblReporte.AddCell(clValor);

            //Responsable----------------------------------------------
            clConcepto = new PdfPCell(new Phrase("Responsable", _standardFont));
            clConcepto.BorderWidth = 0;
            clConcepto.BorderWidthBottom = 0;

            usuariosHelper ushelp = new usuariosHelper();
            usuarios us = ushelp.getUsuarioByID(Int32.Parse(idresponsable));
            String nombreresponsable = "no encontrado";
            if (us != null) {
                nombreresponsable = us.nombre;
            }

            clValor = new PdfPCell(new Phrase(nombreresponsable, _standardFont));
            clValor.BorderWidth = 0;
            clValor.BorderWidthBottom = 0;

            tblReporte.AddCell(clConcepto);
            tblReporte.AddCell(clValor);

            //Solicitado por----------------------------------------------
            clConcepto = new PdfPCell(new Phrase("Solicitado Por", _standardFont));
            clConcepto.BorderWidth = 0;
            clConcepto.BorderWidthBottom = 0;
           
            usuarios ussol = ushelp.getUsuarioByID(Int32.Parse(idsolicitadopor));
            String nombresolicitadopor = "no encontrado";
            if (ussol != null)
            {
                nombresolicitadopor = ussol.nombre;
            }

            clValor = new PdfPCell(new Phrase(nombresolicitadopor, _standardFont));
            clValor.BorderWidth = 0;
            clValor.BorderWidthBottom = 0;

            tblReporte.AddCell(clConcepto);
            tblReporte.AddCell(clValor);  
                      
            tblReporte.SetWidths(columnWidths);

            doc.Add(tblReporte);

            doc.Add(new Paragraph("\n"));
            Paragraph tituloreq = new Paragraph("Requerimientos", _TitleFont);
            doc.Add(tituloreq);
            doc.Add(new Paragraph("\n"));

            PdfPTable tblrequerimientos = new PdfPTable(4);
            tblrequerimientos.WidthPercentage = 100;

            PdfPCell clsEquipo = new PdfPCell(new Phrase("Equipo", _standardFont));
            clsEquipo.BorderWidth = 0;
            clsEquipo.BorderWidthBottom = 0.75f;

            PdfPCell clMarca = new PdfPCell(new Phrase("Marca", _standardFont));
            clMarca.BorderWidth = 0;
            clMarca.BorderWidthBottom = 0.75f;

            PdfPCell clModelo = new PdfPCell(new Phrase("Modelo", _standardFont));
            clModelo.BorderWidth = 0;
            clModelo.BorderWidthBottom = 0.75f;

            PdfPCell clCantidad = new PdfPCell(new Phrase("Cantidad", _standardFont));
            clCantidad.BorderWidth = 0;
            clCantidad.BorderWidthBottom = 0.75f;

            tblrequerimientos.AddCell(clsEquipo);
            tblrequerimientos.AddCell(clMarca);
            tblrequerimientos.AddCell(clModelo);
            tblrequerimientos.AddCell(clCantidad);

            foreach (var jref in jlstreq)
            {
                JObject jobj = (JObject)jref;                

                clsEquipo = new PdfPCell(new Phrase(jobj["equipo"].ToString(), _standardFont));
                clsEquipo.BorderWidth = 0;
                clsEquipo.BorderWidthBottom = 0;

                clMarca = new PdfPCell(new Phrase(jobj["marca"].ToString(), _standardFont));
                clMarca.BorderWidth = 0;
                clMarca.BorderWidthBottom = 0;

                clModelo = new PdfPCell(new Phrase(jobj["modelo"].ToString(), _standardFont));
                clModelo.BorderWidth = 0;
                clModelo.BorderWidthBottom = 0;

                clCantidad = new PdfPCell(new Phrase(jobj["cantidad"].ToString(), _standardFont));
                clCantidad.BorderWidth = 0;
                clCantidad.BorderWidthBottom = 0;

                tblrequerimientos.AddCell(clsEquipo);
                tblrequerimientos.AddCell(clMarca);
                tblrequerimientos.AddCell(clModelo);
                tblrequerimientos.AddCell(clCantidad);
                
            }

            doc.Add(tblrequerimientos);
            doc.Close();
           
            try
            {
                

                String titulomsg = "Solicitud de Maquinaria " + folio;
                String bodymsg = "Solicitud de Maquinaria " + folio;


                String strsmtp = "mail.proyextra.com";
                String emisor = "icom@proyextra.com";
                String receptor = "jav8586@gmail.com";
                String pass = "ICOM2017*";
                int puerto = 26;
                Boolean blnssl = false;

                /*String strsmtp = "smtp.gmail.com";
                String emisor = "jav8586@gmail.com";
                String receptor = "jav8586@hotmail.com";
                String pass = "toluca";
                int puerto = 587;
                Boolean blnssl = true;*/

                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(emisor);
                msg.Subject = titulomsg;
                msg.Body = bodymsg;
                msg.To.Add(receptor);

                Stream pdfstream = new MemoryStream(ms.ToArray());
                msg.Attachments.Add(new Attachment(pdfstream, "SolicitudMaquinaria.pdf"));
                                   
                funciones.sendMail(msg, strsmtp, emisor, pass, puerto, blnssl);

            }
            catch (Exception e)
            {
                clsError objerr = new clsError();
                objerr.error = "Se ha guardado la solicitud sin embago existio un error al mandar el correo electronico " + e.ToString();
                objerr.result = 0;
                return objerr;
            }
            
            
            Dictionary<String, String> dresp = new Dictionary<string, string>();
            dresp.Add("respuesta", "exito");
            return dresp;
           

        }

        

    }
}
