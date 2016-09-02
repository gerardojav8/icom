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
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace WebIcomApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("controldeobras")]
    public class controldeobraController : ApiController
    {
        iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
        iTextSharp.text.Font _TitleFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 15, iTextSharp.text.Font.BOLDITALIC, BaseColor.BLACK);

        [Authorize]
        [HttpPost]
        [Route("exportaPDF")]
        public Object exportaPDF(JObject json)
        {
            String strFechaini = json["fechaini"].ToString();
            String strFechafin = json["fechafin"].ToString();
            String idusuario = json["idusuario"].ToString();


            DateTime dtFechain = DateTime.Parse(strFechaini);
            DateTime dtFechafin = DateTime.Parse(strFechafin);

            tareasPlanificadorHelper tphelp = new tareasPlanificadorHelper();  
            categoriasPlanificadorHelper cphelp = new categoriasPlanificadorHelper();

            List<clsTareaPlanificador> lstTareas = new List<clsTareaPlanificador>();
            DateTime dtFechaAnalizada = dtFechain;

            String pathpdf = "c:/pdf" + idusuario + ".pdf";
            if (!File.Exists(pathpdf)){ File.Delete(pathpdf);}

            Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(pathpdf, FileMode.Create));
            doc.Open();

            iTextSharp.text.Font _FechaFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            float[] columnWidths = new float[] { 25f, 25f, 20f, 20f, 10f, 10f };

            Paragraph titulo = new Paragraph("Reporte de Tareas de " + strFechaini + " a " + strFechafin, _TitleFont);
            titulo.Alignment = Element.ALIGN_CENTER;

            doc.Add(titulo);
            doc.Add(new Paragraph("\n"));

            do
            {
                List<TareasPlanificador> lsttar = tphelp.getTodasTareasPlanificadorbyFecha(dtFechaAnalizada);
                dtFechaAnalizada.AddDays(1);

                if (lsttar == null)
                {
                    continue;
                }

                PdfPTable tblReporte = new PdfPTable(6);
                tblReporte.WidthPercentage = 100;

                PdfPCell clFecha = new PdfPCell(new Phrase(dtFechaAnalizada.ToString("yyyy-MM-dd"), _FechaFont));
                clFecha.BorderWidth = 0;
                clFecha.BorderWidthBottom = 0.75f;

                PdfPCell clClasificacion = new PdfPCell(new Phrase("Clasificacion", _standardFont));
                clClasificacion.BorderWidth = 0;
                clClasificacion.BorderWidthBottom = 0.75f;

                PdfPCell clInicio = new PdfPCell(new Phrase("Inicio", _standardFont));
                clInicio.BorderWidth = 0;
                clInicio.BorderWidthBottom = 0.75f;

                PdfPCell clFin = new PdfPCell(new Phrase("Fin", _standardFont));
                clFin.BorderWidth = 0;
                clFin.BorderWidthBottom = 0.75f;

                PdfPCell clHoras = new PdfPCell(new Phrase("Horas", _standardFont));
                clHoras.BorderWidth = 0;
                clHoras.BorderWidthBottom = 0.75f;

                PdfPCell clPor = new PdfPCell(new Phrase("Porcentaje", _standardFont));
                clPor.BorderWidth = 0;
                clPor.BorderWidthBottom = 0.75f;

                tblReporte.AddCell(clFecha);
                tblReporte.AddCell(clClasificacion);
                tblReporte.AddCell(clInicio);
                tblReporte.AddCell(clFin);
                tblReporte.AddCell(clHoras);
                tblReporte.AddCell(clPor);

                foreach (TareasPlanificador tar in lsttar) {
                    clFecha = new PdfPCell(new Phrase(tar.titulo, _standardFont));
                    clFecha.BorderWidth = 0;
                    clFecha.BorderWidthBottom = 0;

                    clClasificacion = new PdfPCell(new Phrase(tar.categoriasPlanificador.nombre, _standardFont));
                    clClasificacion.BorderWidth = 0;
                    clClasificacion.BorderWidthBottom = 0;
                    DateTime dtini = (DateTime)tar.fechainicio;
                    String strInicio = dtini.Year + "-" + dtini.Month + "-" + dtini.Day + " " + tar.horainicio.ToString();
                    clInicio = new PdfPCell(new Phrase(strInicio, _standardFont));
                    clInicio.BorderWidth = 0;
                    clInicio.BorderWidthBottom = 0;

                    DateTime dtfin = (DateTime)tar.fechafin;
                    String strFin = dtfin.Year + "-" + dtfin.Month + "-" + dtfin.Day + " " + tar.horafin.ToString();
                    clFin = new PdfPCell(new Phrase(strFin, _standardFont));
                    clFin.BorderWidth = 0;
                    clFin.BorderWidthBottom = 0;

                    clHoras = new PdfPCell(new Phrase(tar.horas.ToString(), _standardFont));
                    clHoras.BorderWidth = 0;
                    clHoras.BorderWidthBottom = 0;

                    clPor = new PdfPCell(new Phrase(tar.porcentaje.ToString(), _standardFont));
                    clPor.BorderWidth = 0;
                    clPor.BorderWidthBottom = 0;

                    tblReporte.AddCell(clFecha);
                    tblReporte.AddCell(clClasificacion);
                    tblReporte.AddCell(clInicio);
                    tblReporte.AddCell(clFin);
                    tblReporte.AddCell(clHoras);
                    tblReporte.AddCell(clPor);
                }
                
                tblReporte.SetWidths(columnWidths);
                doc.Add(tblReporte);
                doc.Add(new Paragraph("\n"));
                doc.Add(new Paragraph("\n"));
                               
            } while (dtFechaAnalizada <= dtFechafin);
                                              
            doc.Close();

            Byte[] pdfbytes = File.ReadAllBytes(pathpdf);
            String pdf64 = Convert.ToBase64String(pdfbytes);
            Dictionary<String, String> resp = new Dictionary<string, string>();

            resp.Add("pdf", pdf64);
            //File.Delete(pathpdf);

            return resp;
        }

        [Authorize]
        [HttpPost]
        [Route("exportaGraficaPDF")]
        public Object exportaGraficaPDF(JObject json)
        {
            String strimg64 = json["imagen"].ToString();
            String idusuario = json["idusuario"].ToString();

            Byte[] bytesimg;

            if (strimg64.Equals(""))
            {
                clsError objerr = new clsError();
                objerr.error = "No se ha mandado la imagen de la grafica";
                objerr.result = 0;
                return objerr;
            }

            var bytes = Convert.FromBase64String(strimg64);
            bytesimg = bytes;            

            String pathpdf = "c:/pdfgrafica"+idusuario+".pdf";
            if (!File.Exists(pathpdf))
            {
                File.Delete(pathpdf);
            }

            Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(pathpdf, FileMode.Create));
            doc.Open();

            iTextSharp.text.Image imgpdf = iTextSharp.text.Image.GetInstance(bytesimg);
            imgpdf.BorderWidth = 0;
            imgpdf.Alignment = Element.ALIGN_CENTER;
            float perscala = 0.0f;
            perscala = 300 / imgpdf.Width;
            imgpdf.ScalePercent(perscala * 100);
            doc.Add(imgpdf);

            

            PdfPTable tblClasificaciones = new PdfPTable(3);
            tblClasificaciones.WidthPercentage = 100;

            // Configuramos el título de las columnas de la tabla
            PdfPCell clColor = new PdfPCell(new Phrase("Color", _standardFont));
            clColor.BorderWidth = 0;
            clColor.BorderWidthBottom = 0.75f;

            PdfPCell clClasificacion = new PdfPCell(new Phrase("Clasificacion", _standardFont));
            clClasificacion.BorderWidth = 0;
            clClasificacion.BorderWidthBottom = 0.75f;

            PdfPCell clsHoras = new PdfPCell(new Phrase("Horas", _standardFont));
            clsHoras.BorderWidth = 0;
            clsHoras.BorderWidthBottom = 0.75f;

            // Añadimos las celdas a la tabla
            tblClasificaciones.AddCell(clColor);
            tblClasificaciones.AddCell(clClasificacion);
            tblClasificaciones.AddCell(clsHoras);


            JArray jclasarr = JArray.Parse(json["clasificaciones"].ToString());            
            foreach (var clas in jclasarr)
            {
                JObject jobj = (JObject)clas;
                String strColor = jobj["color"].ToString();
                String strClasificacion = jobj["clasificacion"].ToString();
                String strHoras = jobj["horas"].ToString();

                String[] arrcolor = strColor.Split(',');
                int r = Int32.Parse(arrcolor[0]);
                int g = Int32.Parse(arrcolor[1]);
                int b = Int32.Parse(arrcolor[2]);

                clColor = new PdfPCell(new Phrase("", _standardFont));
                clColor.BackgroundColor = new BaseColor(Color.FromArgb(r, g, b));
                clColor.BorderWidth = 0;

                clClasificacion = new PdfPCell(new Phrase(strClasificacion, _standardFont));
                clClasificacion.BorderWidth = 0;

                clsHoras = new PdfPCell(new Phrase(strHoras, _standardFont));
                clsHoras.BorderWidth = 0;
                
                tblClasificaciones.AddCell(clColor);
                tblClasificaciones.AddCell(clClasificacion);
                tblClasificaciones.AddCell(clsHoras);
                                               
            }

            float[] columnWidths = new float[] { 5f, 30f, 30f };
            tblClasificaciones.SetWidths(columnWidths);

            doc.Add(tblClasificaciones);

            doc.Close();

            if (!File.Exists(pathpdf))
            {
                clsError objer = new clsError();
                objer.error = "No se ha podido crear el pdf";
                objer.result = 0;                
                return objer;
            }

            Byte[] pdfbytes = File.ReadAllBytes(pathpdf);
            String pdf64 = Convert.ToBase64String(pdfbytes);

            Dictionary<String, String> resp = new Dictionary<string, string>();
            resp.Add("pdf", pdf64);
            File.Delete(pathpdf);

            return resp;
        }

        [Authorize]
        [HttpPost]
        [Route("GuardaMensajeconArchivo")]
        public Object GuardaMensajeconArchivo(JObject json)
        {
            String strarchivo = json["archivo"].ToString();
            String strmsg = json["mensaje"].ToString();
            String stridusuario = json["idusuario"].ToString();
            String nombrearchivo = json["nombre"].ToString();


            if (strarchivo.Equals(""))
            {
                clsError objerr = new clsError();
                objerr.error = "Debe de enviar un archivo a guardar";
                objerr.result = 0;
                return objerr;
            }

            
            try
            {
                var bytes = Convert.FromBase64String(strarchivo);
                chatGeneralHelper cghelp = new chatGeneralHelper();
                chat_general objchat = new chat_general();
                objchat.archivo = bytes;
                objchat.comentario = strmsg;
                objchat.idusuario = Int32.Parse(stridusuario);
                objchat.nombrearchivo = nombrearchivo;

                DateTime dtahora = DateTime.Now;
                dtahora = new DateTime(dtahora.Year, dtahora.Month, dtahora.Day, dtahora.Hour, dtahora.Minute, dtahora.Second, dtahora.Kind);

                objchat.fecha = dtahora;
                objchat.hora =  dtahora.TimeOfDay;

                long idmensaje = cghelp.insertChatGeneral(objchat);
                if (idmensaje > -1)
                {
                    
                    Dictionary<String, long> resp = new Dictionary<string, long>();
                    resp.Add("idmensaje", idmensaje);
                    return resp;
                }
                else
                {
                    clsError objerr = new clsError();
                    objerr.error = "No se ha podido guardar el mensaje";
                    objerr.result = 0;
                    return objerr;

                }
            }
            catch (Exception e)
            {
                clsError objerr = new clsError();
                objerr.error = "Error "+ e.ToString();
                objerr.result = 0;
                return objerr;
            }

            
        }

        [Authorize]
        [HttpPost]
        [Route("getArchivodeMensaje")]
        public Object getArchivodeMensaje(JObject json)
        {
            String stridmensaje = json["idmensaje"].ToString();           
           
            try
            {
                chatGeneralHelper cghelp = new chatGeneralHelper();
                chat_general objchat = cghelp.getchat_generalById(Int64.Parse(stridmensaje));
                if (objchat == null)
                {
                    clsError objerr = new clsError();
                    objerr.error = "No se ha encontrado el archivo";
                    objerr.result = 0;
                    return objerr;
                }

                String b64 = Convert.ToBase64String(objchat.archivo);
                

                Dictionary<String, String> resp = new Dictionary<string, string>();
                resp.Add("archivo", b64);
                resp.Add("nombre", objchat.nombrearchivo);
                return resp;
                
            }
            catch (Exception e)
            {
                clsError objerr = new clsError();
                objerr.error = "Error " + e.ToString();
                objerr.result = 0;
                return objerr;
            }


        }

        [Authorize]
        [HttpPost]
        [Route("getMensajesChat")]
        public Object getMensajesChat()
        {
            chatGeneralHelper cghelp = new chatGeneralHelper();
            List<chat_general> cglst = cghelp.getMensajesByMenosDias(10);

            if (cglst == null)
            {
                clsError objerr = new clsError();
                objerr.error = "No se han Encontrado Mensajes";
                objerr.result = 0;
                return objerr;
            }
            else 
            {

                List<clsMensajeChat> lstmensajes = new List<clsMensajeChat>();
                usuariosHelper ushelp = new usuariosHelper();

                foreach(chat_general cg in cglst){

                    clsMensajeChat obj = new clsMensajeChat();

                    obj.idmensaje = cg.idmensaje;
                    obj.idusuario = (int)cg.idusuario;
                    DateTime dtfecha = (DateTime)cg.fecha;
                    obj.fecha = dtfecha.Year + "-" + dtfecha.Month + "-" + dtfecha.Day;
                    String hora = cg.hora.ToString();
                    obj.hora = hora;
                    obj.mensaje = cg.comentario;

                    usuarios objus = ushelp.getUsuarioByID(obj.idusuario);

                    if (objus != null)
                    {
                        obj.nombre = objus.nombre + " " + objus.apepaterno + " " + objus.apematerno;
                        obj.iniciales = objus.nombre.Substring(0, 1) + objus.apepaterno.Substring(0, 1);
                    }
                    else {
                        obj.nombre = "";
                        obj.iniciales = "";
                    }

                    if (cg.archivo != null)
                    {
                        obj.nombrearchivo = cg.nombrearchivo;
                        
                    }
                    else {
                        obj.nombrearchivo = "";
                    }
                    
                    lstmensajes.Add(obj);
                }


                return lstmensajes;

            }

        }

        [Authorize]
        [HttpPost]
        [Route("getListadoAgenda")]
        public Object getListadoAgenda(JObject json)
        {
            String stridusuario = json["idusuario"].ToString();

            eventosAgendaHelper eahelp = new eventosAgendaHelper();
            List<eventosagenda> lstea = eahelp.getEventosAnioActualByIdUsuario(Int32.Parse(stridusuario));

            if (lstea == null)
            {
                clsError objerr = new clsError();
                objerr.error = "No se han Encontrado Eventos";
                objerr.result = 0;
                return objerr;
            }
            else {
                List<clsAgenda> lstagenda = new List<clsAgenda>();

                clsAgenda obj1 = new clsAgenda();
                obj1.mes = 1;

                clsAgenda obj2 = new clsAgenda();
                obj2.mes = 2;

                clsAgenda obj3 = new clsAgenda();
                obj3.mes = 3;

                clsAgenda obj4 = new clsAgenda();
                obj4.mes = 4;

                clsAgenda obj5 = new clsAgenda();
                obj5.mes = 5;

                clsAgenda obj6 = new clsAgenda();
                obj6.mes = 6;

                clsAgenda obj7 = new clsAgenda();
                obj7.mes = 7;

                clsAgenda obj8 = new clsAgenda();
                obj8.mes = 8;

                clsAgenda obj9 = new clsAgenda();
                obj9.mes = 9;

                clsAgenda obj10 = new clsAgenda();
                obj10.mes = 10;
                clsAgenda obj11 = new clsAgenda();
                obj11.mes = 11;

                clsAgenda obj12 = new clsAgenda();
                obj12.mes = 12;

                lstagenda.Add(obj1);
                lstagenda.Add(obj2);
                lstagenda.Add(obj3);
                lstagenda.Add(obj4);
                lstagenda.Add(obj5);
                lstagenda.Add(obj6);
                lstagenda.Add(obj7);
                lstagenda.Add(obj8);
                lstagenda.Add(obj9);
                lstagenda.Add(obj10);
                lstagenda.Add(obj11);
                lstagenda.Add(obj12);

                foreach(eventosagenda ea in lstea){
                    clsEvento objev = new clsEvento();
                    DateTime fechaini = (DateTime)ea.fechainicio;
                    String strfechaini = fechaini.Year + "-" + fechaini.Month + "-" + fechaini.Day; 

                    DateTime fechafin = (DateTime)ea.fechafin;
                    String strfechafin = fechafin.Year + "-" + fechafin.Month + "-" + fechafin.Day;

                    String lapso = strfechaini + " " + ea.horaini.ToString() + " - " + strfechafin + " " + ea.horafin.ToString();

                    objev.idevento = ea.idevento;                    
                    objev.dia = fechaini.Day;
                    objev.titulo = ea.titulo;                    
                    objev.lapso = lapso;
                    
                    int indice = (int)ea.mes -1;
                    lstagenda.ElementAt(indice).eventos.Add(objev);
                   
                }

                return lstagenda;
            }
        }

        [Authorize]
        [HttpPost]
        [Route("getEventoAgenda")]
        public Object getEventoAgenda(JObject json)
        {
            String stridevento = json["idevento"].ToString();

            eventosAgendaHelper eaHelp = new eventosAgendaHelper();
            eventosagenda objea = eaHelp.geteventosagendaById(Int32.Parse(stridevento));

            if (objea == null)
            {
                clsError objerr = new clsError();
                objerr.error = "No se ha Encontrado el evento";
                objerr.result = 0;
                return objerr;
            }
            else {
                clsEventoAgenda objearesp = new clsEventoAgenda();
                int dia = ((DateTime)objea.fechainicio).Day;
                objearesp.dia = dia;
                objearesp.mes = (int)objea.mes;

                DateTime fechaini = (DateTime)objea.fechainicio;
                String diaini = fechaini.Day.ToString();
                if (diaini.Length == 1) {
                    diaini = "0" + diaini;
                }

                String mesini = fechaini.Month.ToString();
                if (mesini.Length == 1) {
                    mesini = "0" + mesini;
                }
                
                String strfechaini = fechaini.Year + "-" + mesini + "-" + diaini;

                DateTime fechafin = (DateTime)objea.fechafin;
                String diafin = fechafin.Day.ToString();
                if (diafin.Length == 1)
                {
                    diafin = "0" + diafin;
                }

                String mesfin = fechafin.Month.ToString();
                if (mesfin.Length == 1)
                {
                    mesfin = "0" + mesfin;
                }

                String strfechafin = fechafin.Year + "-" + mesfin + "-" + diafin;

                String lapso = strfechaini + " " + objea.horaini.ToString() + " - " + strfechafin + " " + objea.horafin.ToString();

                objearesp.lapso = lapso;
                objearesp.titulo = objea.titulo;
                objearesp.comentario = objea.comentario;
                objearesp.fechaini = strfechaini;
                objearesp.fechafin = strfechafin;
                objearesp.horaini = objea.horaini.ToString();
                objearesp.horafin = objea.horafin.ToString();


                
                List<Dictionary<String, String>> lstusuarios = new List<Dictionary<string, string>>();
                foreach (usuarios us in objea.usuarios) {
                    Dictionary<String, String> dicus = new Dictionary<string, string>();
                    dicus.Add("nombre", us.nombre + " " + us.apematerno + " " + us.apematerno);
                    lstusuarios.Add(dicus);

                }

                objearesp.usuarios = lstusuarios;
                return objearesp;
            }            
        }

        [Authorize]
        [HttpPost]
        [Route("getChatEvento")]
        public Object getChatEvento(JObject json)
        {
            String stridevento = json["idevento"].ToString();

            chatEventosHelper cehelp = new chatEventosHelper();
            List<chat_eventos> celst = cehelp.getMensajesByMenosDiasAndIdEvento(10, Int32.Parse(stridevento));

            if (celst == null)
            {
                clsError objerr = new clsError();
                objerr.error = "No se han Encontrado Mensajes";
                objerr.result = 0;
                return objerr;
            }
            else
            {

                List<clsMensajeChat> lstmensajes = new List<clsMensajeChat>();
                usuariosHelper ushelp = new usuariosHelper();

                foreach (chat_eventos ce in celst)
                {
                    clsMensajeChat obj = new clsMensajeChat();

                    obj.idmensaje = ce.idmensaje;
                    obj.idusuario = (int)ce.idusuario;
                    DateTime dtfecha = (DateTime)ce.fecha;
                    obj.fecha = dtfecha.Year + "-" + dtfecha.Month + "-" + dtfecha.Day;
                    String hora = ce.hora.ToString();
                    obj.hora = hora;
                    obj.mensaje = ce.comentario;

                    usuarios objus = ushelp.getUsuarioByID(obj.idusuario);

                    if (objus != null)
                    {
                        obj.nombre = objus.nombre + " " + objus.apepaterno + " " + objus.apematerno;
                        obj.iniciales = objus.nombre.Substring(0, 1) + objus.apepaterno.Substring(0, 1);
                    }
                    else
                    {
                        obj.nombre = "";
                        obj.iniciales = "";
                    }
                    
                    lstmensajes.Add(obj);
                }

                return lstmensajes;

            }

        }

        [Authorize]
        [HttpPost]
        [Route("guardaEventoAgenda")]
        public Object guardaEventoAgenda(JObject json)
        {

            String titulo = json["titulo"].ToString();
            String notas = json["notas"].ToString();

            String strfechaini = json["fechaini"].ToString();
            String strfechafin = json["fechafin"].ToString();
            DateTime dtfechaini = DateTime.ParseExact(strfechaini, "yyyy-MM-dd", System.Globalization.CultureInfo.InstalledUICulture);
            DateTime dtfechafin = DateTime.ParseExact(strfechafin, "yyyy-MM-dd", System.Globalization.CultureInfo.InstalledUICulture);

            String strhoraini = json["horaini"].ToString();
            String strhorafin = json["horafin"].ToString();
            DateTime dthoraini = DateTime.ParseExact(strhoraini, "HH:mm:ss", System.Globalization.CultureInfo.InstalledUICulture);
            DateTime dthorafin = DateTime.ParseExact(strhorafin, "HH:mm:ss", System.Globalization.CultureInfo.InstalledUICulture);

            

            String strdiacompleto = json["diacompleto"].ToString();
            int intDiacompleto = Int32.Parse(strdiacompleto);

            String strnotificaasistentes = json["notificaasistentes"].ToString();
            int intnotificaasistentes = Int32.Parse(strnotificaasistentes);

            /*if (intnotificaasistentes == 1) { 
                //Mandar correo a usuarios
            }*/
           
            JArray jlstasistentes = JArray.Parse(json["asistentes"].ToString());



            List<int> usuarios = new List<int>();
            foreach (var jasis in jlstasistentes)
            {
                JObject jobj = (JObject)jasis;
                int idusuario = Int32.Parse(jobj["idusuario"].ToString());
                usuarios.Add(idusuario);
            }

            eventosagenda objea = new eventosagenda();
            objea.mes = dtfechaini.Month;
            objea.anio = dtfechaini.Year;
            objea.titulo = titulo;
            objea.comentario = notas;
            objea.horaini = dthoraini.TimeOfDay;
            objea.horafin = dthorafin.TimeOfDay;
            objea.fechainicio = dtfechaini;
            objea.fechafin = dtfechafin;
            objea.diacompleto = (byte)intDiacompleto;
                        
            eventosAgendaHelper eaHelp = new eventosAgendaHelper();
            String resp = eaHelp.inserteventosagenda(objea, usuarios);

            if (resp.Equals(""))
            {
                clsError objerr = new clsError();
                objerr.error = "Evento guardado con exito";
                objerr.result = 0;
                return objerr;
            }
            else {
                clsError objerr = new clsError();
                objerr.error = "Error " + resp;
                objerr.result = 1;
                return objerr;
            }
        }
    }
}
