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
using System.Net.Mail;

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
            String idusuario = json["idusuario"].ToString();
            String idcategoria = json["idcategoria"].ToString();


            //DateTime dtFechain = DateTime.Parse(strFechaini);
            //DateTime dtFechafin = DateTime.Parse(strFechafin);

            tareasPlanificadorHelper tphelp = new tareasPlanificadorHelper();  
            categoriasPlanificadorHelper cphelp = new categoriasPlanificadorHelper();

            categoriasPlanificador cat = cphelp.getcategoriasPlanificadorById(Int64.Parse(idcategoria));
            
            //DateTime dtFechaAnalizada = dtFechain;

            MemoryStream ms = new MemoryStream();

            Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER);
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);
            doc.Open();

            iTextSharp.text.Font _FechaFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            float[] columnWidths = new float[] { 25f, 25f, 20f, 20f, 10f, 10f };

            

            Paragraph titulo = new Paragraph("Reporte de Tareas de " + cat.nombre, _TitleFont);
            titulo.Alignment = Element.ALIGN_CENTER;

            doc.Add(titulo);
            doc.Add(new Paragraph("\n"));

            List<TareasPlanificador> lsttar = tphelp.getTareasPlanificadorByIdCategoria(Int64.Parse(idcategoria));
            PdfPTable tblReporte = new PdfPTable(6);
            tblReporte.WidthPercentage = 100;

            PdfPCell clFecha = new PdfPCell(new Phrase("Tarea", _FechaFont));
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

            foreach (TareasPlanificador tar in lsttar)
            {
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
                                                         
            doc.Close();

            Byte[] pdfbytes = ms.ToArray();
            String pdf64 = Convert.ToBase64String(pdfbytes);
            Dictionary<String, String> resp = new Dictionary<string, string>();

            resp.Add("pdf", pdf64);

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
            
            MemoryStream ms = new MemoryStream();

            Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER);            
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);
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

            PdfPCell clsHoras = new PdfPCell(new Phrase("Porcentaje", _standardFont));
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
                String strPorcentaje = jobj["porcentaje"].ToString();

                String[] arrcolor = strColor.Split(',');
                int r = Int32.Parse(arrcolor[0]);
                int g = Int32.Parse(arrcolor[1]);
                int b = Int32.Parse(arrcolor[2]);

                
                clColor = new PdfPCell(new Phrase("", _standardFont));
                clColor.BackgroundColor = new BaseColor(Color.FromArgb(r, g, b));
                clColor.BorderWidth = 0;

                clClasificacion = new PdfPCell(new Phrase(strClasificacion, _standardFont));
                clClasificacion.BorderWidth = 0;

                clsHoras = new PdfPCell(new Phrase(strPorcentaje, _standardFont));
                clsHoras.BorderWidth = 0;
                
                tblClasificaciones.AddCell(clColor);
                tblClasificaciones.AddCell(clClasificacion);
                tblClasificaciones.AddCell(clsHoras);
                                               
            }

            float[] columnWidths = new float[] { 5f, 30f, 30f };
            tblClasificaciones.SetWidths(columnWidths);

            doc.Add(tblClasificaciones);

            doc.Close();
                        
            Byte[] pdfbytes = ms.ToArray();
            String pdf64 = Convert.ToBase64String(pdfbytes);

            Dictionary<String, String> resp = new Dictionary<string, string>();
            resp.Add("pdf", pdf64);            

            return resp;
        }

        [Authorize]
        [HttpPost]
        [Route("getCategoriasListado")]
        public Object getCategoriasListado(JObject json)
        {
            try
            {
                String idobra = json["idobra"].ToString();

                categoriasPlanificadorHelper cathelp = new categoriasPlanificadorHelper();
                tareasPlanificadorHelper thelp = new tareasPlanificadorHelper();


                List<categoriasPlanificador> lstcat = cathelp.getCategoriasByIdObra(Int32.Parse(idobra));
                List<Dictionary<String, String>> categorias = new List<Dictionary<string, string>>();

                if (lstcat.Count == 0)
                {
                    clsError objer = new clsError();
                    objer.error = "No se encontraron categorias";
                    objer.result = 0;
                    return objer;
                }

                foreach (categoriasPlanificador cat in lstcat)
                {
                    Dictionary<String, String> respcat = new Dictionary<string, string>();
                    respcat.Add("idcategoria", cat.idcategoria.ToString());
                    respcat.Add("nombre", cat.nombre);
                    int notar = thelp.getNoTareasByIdCategoria(cat.idcategoria);
                    respcat.Add("notareas", notar.ToString());
                    double porcentaje_categoria = cathelp.getPorcentajeCategoria(cat.idcategoria);
                    respcat.Add("porcentaje", Math.Round(porcentaje_categoria, 2).ToString().Replace(',', '.'));
                    categorias.Add(respcat);
                }

                Dictionary<String, List<Dictionary<String, String>>> resp = new Dictionary<string, List<Dictionary<string, string>>>();
                resp.Add("categorias", categorias);

                return resp;
            }
            catch (Exception e)
            {
                clsError objer = new clsError();
                objer.error = "Error al traer las categorias actuales " + e.ToString();
                objer.result = -1;
                return objer;
            }



        }

        [Authorize]
        [HttpPost]
        [Route("getTareasPlanificadorByIdCategoria")]
        public Object getTareasPlanificadorByIdCategoria(JObject json)
        {
            try
            {
                String idcategoria = json["idcategoria"].ToString();                
                tareasPlanificadorHelper thelp = new tareasPlanificadorHelper();


                List<TareasPlanificador> lsttar = thelp.getTareasPlanificadorByIdCategoria(Int64.Parse(idcategoria));
                List<Dictionary<String, String>> tareas = new List<Dictionary<string, string>>();

                if (lsttar.Count == 0)
                {
                    clsError objer = new clsError();
                    objer.error = "No se encontraron tareas";
                    objer.result = 0;
                    return objer;
                }

                foreach (TareasPlanificador tar in lsttar)
                {
                    Dictionary<String, String> resptar = new Dictionary<string, string>();

                    resptar.Add("idtarea", tar.idtarea.ToString());
                    resptar.Add("titulo", tar.titulo);
                    DateTime dtinicio = (DateTime) tar.fechainicio;
                    DateTime dtfin = (DateTime) tar.fechafin;

                    string mesini = dtinicio.Month.ToString().Length < 2 ? "0" + dtinicio.Month.ToString() : dtinicio.Month.ToString();
                    string diaini = dtinicio.Day.ToString().Length < 2 ? "0" + dtinicio.Day.ToString() : dtinicio.Day.ToString();

                    string mesfin = dtfin.Month.ToString().Length < 2 ? "0" + dtfin.Month.ToString() : dtfin.Month.ToString();
                    string diafin = dtfin.Day.ToString().Length < 2 ? "0" + dtfin.Day.ToString() : dtfin.Day.ToString();

                    String strInicio = dtinicio.Year + "-" + mesini + "-" + diaini + " " + tar.horainicio.ToString().Substring(0, 8);
                    String strFin = dtfin.Year + "-" + mesfin + "-" + diafin + " " + tar.horafin.ToString().Substring(0, 8);

                    resptar.Add("horas", tar.horas.ToString().Replace(',', '.'));
                    String lapso = "de " + strInicio + " a " + strFin;
                    resptar.Add("lapso", lapso);
                    resptar.Add("porcentaje", Math.Round((double)tar.porcentaje, 2).ToString().Replace(',', '.'));

                    tareas.Add(resptar);

                }

                Dictionary<String, List<Dictionary<String, String>>> resp = new Dictionary<string, List<Dictionary<string, string>>>();
                resp.Add("tareas", tareas);

                return resp;
            }
            catch (Exception e)
            {
                clsError objer = new clsError();
                objer.error = "Error al traer las categorias actuales " + e.ToString();
                objer.result = -1;
                return objer;
            }



        }

        [Authorize]
        [HttpPost]
        [Route("busquedaTareasPlanificador")]
        public Object busquedaTareasPlanificador(JObject json)
        {
            try
            {
                String idcategoria = json["idcategoria"].ToString();
                String strBusqueda = json["strBusqueda"].ToString();
                tareasPlanificadorHelper thelp = new tareasPlanificadorHelper();


                List<TareasPlanificador> lsttar = thelp.busquedaTareasPlanificador(Int64.Parse(idcategoria), strBusqueda);
                List<Dictionary<String, String>> tareas = new List<Dictionary<string, string>>();

                if (lsttar.Count == 0)
                {
                    clsError objer = new clsError();
                    objer.error = "No se encontraron tareas";
                    objer.result = 0;
                    return objer;
                }

                foreach (TareasPlanificador tar in lsttar)
                {
                    Dictionary<String, String> resptar = new Dictionary<string, string>();

                    resptar.Add("idtarea", tar.idtarea.ToString());
                    resptar.Add("titulo", tar.titulo);
                    DateTime dtinicio = (DateTime)tar.fechainicio;
                    DateTime dtfin = (DateTime)tar.fechafin;

                    string mesini = dtinicio.Month.ToString().Length < 2 ? "0" + dtinicio.Month.ToString() : dtinicio.Month.ToString();
                    string diaini = dtinicio.Day.ToString().Length < 2 ? "0" + dtinicio.Day.ToString() : dtinicio.Day.ToString();

                    string mesfin = dtfin.Month.ToString().Length < 2 ? "0" + dtfin.Month.ToString() : dtfin.Month.ToString();
                    string diafin = dtfin.Day.ToString().Length < 2 ? "0" + dtfin.Day.ToString() : dtfin.Day.ToString();

                    String strInicio = dtinicio.Year + "-" + mesini + "-" + diaini + " " + tar.horainicio.ToString().Substring(0, 8);
                    String strFin = dtfin.Year + "-" + mesfin + "-" + diafin + " " + tar.horafin.ToString().Substring(0, 8);

                    resptar.Add("horas", tar.horas.ToString().Replace(',', '.'));
                    String lapso = "de " + strInicio + " a " + strFin;
                    resptar.Add("lapso", lapso);
                    resptar.Add("porcentaje", Math.Round((double)tar.porcentaje, 2).ToString().Replace(',', '.'));

                    tareas.Add(resptar);

                }

                Dictionary<String, List<Dictionary<String, String>>> resp = new Dictionary<string, List<Dictionary<string, string>>>();
                resp.Add("tareas", tareas);

                return resp;
            }
            catch (Exception e)
            {
                clsError objer = new clsError();
                objer.error = "Error al traer las categorias actuales " + e.ToString();
                objer.result = -1;
                return objer;
            }



        }

        [Authorize]
        [HttpPost]
        [Route("BuscarCategorias")]
        public Object BuscarCategorias(JObject json)
        {
            try
            {
                String strBusqueda = json["strBusqueda"].ToString();
                String idobra = json["idobra"].ToString();

                categoriasPlanificadorHelper cathelp = new categoriasPlanificadorHelper();
                tareasPlanificadorHelper thelp = new tareasPlanificadorHelper();


                List<categoriasPlanificador> lstcat = cathelp.getCategoriasBySearch(Int32.Parse(idobra), strBusqueda);
                List<Dictionary<String, String>> categorias = new List<Dictionary<string, string>>();

                if (lstcat.Count == 0)
                {
                    clsError objer = new clsError();
                    objer.error = "No se encontraron categorias";
                    objer.result = 0;
                    return objer;
                }

                foreach (categoriasPlanificador cat in lstcat)
                {
                    Dictionary<String, String> respcat = new Dictionary<string, string>();
                    respcat.Add("idcategoria", cat.idcategoria.ToString());
                    respcat.Add("nombre", cat.nombre);
                    int notar = thelp.getNoTareasByIdCategoria(cat.idcategoria);
                    respcat.Add("notareas", notar.ToString());
                    double porcentaje_categoria = cathelp.getPorcentajeCategoria(cat.idcategoria);
                    respcat.Add("porcentaje", Math.Round(porcentaje_categoria, 2).ToString().Replace(',', '.'));
                    categorias.Add(respcat);
                }

                Dictionary<String, List<Dictionary<String, String>>> resp = new Dictionary<string, List<Dictionary<string, string>>>();
                resp.Add("categorias", categorias);

                return resp;
            }
            catch (Exception e)
            {
                clsError objer = new clsError();
                objer.error = "Error al traer las categorias actuales " + e.ToString();
                objer.result = -1;
                return objer;
            }



        }

        [Authorize]
        [HttpPost]
        [Route("NuevaTarea")]
        public Object NuevaTarea(JObject json)
        {
            try
            {
                String idcategoria = json["idcategoria"].ToString();
                String titulo = json["titulo"].ToString();
                String todoDia = json["todoDia"].ToString();
                String inicio = json["inicio"].ToString();
                String fin = json["fin"].ToString();
                String porcentaje = json["porcentaje"].ToString();
                String notas = json["notas"].ToString();

                tareasPlanificadorHelper thelp = new tareasPlanificadorHelper();
                TareasPlanificador tar = new TareasPlanificador();

                tar.idcategoria = Int64.Parse(idcategoria);
                tar.titulo = titulo;
                tar.todoDia = (byte)Int32.Parse(todoDia);
                
                DateTime dtFechaHoraIni = DateTime.ParseExact(inicio, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InstalledUICulture);
                DateTime dtFechaHoraFin = DateTime.ParseExact(fin, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InstalledUICulture);

                tar.fechainicio = (DateTime)dtFechaHoraIni;
                tar.horainicio = dtFechaHoraIni.TimeOfDay;
                tar.fechafin = (DateTime)dtFechaHoraFin;
                tar.horafin = dtFechaHoraFin.TimeOfDay;

                TimeSpan ts = dtFechaHoraFin - dtFechaHoraIni;
                int horas = ts.Hours;
                tar.horas = horas;
                tar.porcentaje = Decimal.Parse(porcentaje);
                tar.notas = notas;

                thelp.insertTareasPlanificador(tar);

                clsError objer = new clsError();
                objer.error = "La tarea se ha insertado correctamente";
                objer.result = 1;
                return objer;


                
            }
            catch (Exception e)
            {
                clsError objer = new clsError();
                objer.error = "Error Insertar la tarea " + e.ToString();
                objer.result = 0;
                return objer;
            }
        }

        [Authorize]
        [HttpPost]
        [Route("ModificarTarea")]
        public Object ModificarTarea(JObject json)
        {
            try
            {
                String idtarea = json["idtarea"].ToString();                
                String titulo = json["titulo"].ToString();
                String todoDia = json["todoDia"].ToString();
                String inicio = json["inicio"].ToString();
                String fin = json["fin"].ToString();
                String porcentaje = json["porcentaje"].ToString();
                String notas = json["notas"].ToString();

                tareasPlanificadorHelper thelp = new tareasPlanificadorHelper();
                TareasPlanificador tar = new TareasPlanificador();

                tar.idtarea = Int64.Parse(idtarea);
                tar.titulo = titulo;
                tar.todoDia = (byte)Int32.Parse(todoDia);

                DateTime dtFechaHoraIni = DateTime.ParseExact(inicio, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InstalledUICulture);
                DateTime dtFechaHoraFin = DateTime.ParseExact(fin, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InstalledUICulture);

                tar.fechainicio = (DateTime)dtFechaHoraIni;
                tar.horainicio = dtFechaHoraIni.TimeOfDay;
                tar.fechafin = (DateTime)dtFechaHoraFin;
                tar.horafin = dtFechaHoraFin.TimeOfDay;

                TimeSpan ts = dtFechaHoraFin - dtFechaHoraIni;
                int horas = ts.Hours;
                tar.horas = horas;
                tar.porcentaje = Decimal.Parse(porcentaje);
                tar.notas = notas;

                thelp.updateTareasPlanificador(tar);

                clsError objer = new clsError();
                objer.error = "La tarea se ha modificado correctamente";
                objer.result = 1;
                return objer;



            }
            catch (Exception e)
            {
                clsError objer = new clsError();
                objer.error = "Error al modificar la tarea " + e.ToString();
                objer.result = -1;
                return objer;
            }
        }

        [Authorize]
        [HttpPost]
        [Route("getNombreObraCategoria")]
        public Object getNombreObraCategoria(JObject json)
        {
            try
            {
                String idcategoria = json["idcategoria"].ToString();

                categoriasPlanificadorHelper chelp = new categoriasPlanificadorHelper();

                categoriasPlanificador cat = chelp.getcategoriasPlanificadorById(Int64.Parse(idcategoria));

                if (cat == null) {
                    clsError objer = new clsError();
                    objer.error = "No se ha encontrado los datos de la categoria";
                    objer.result = 0;
                    return objer;
                }

                Dictionary<string, string> resp = new Dictionary<string, string>();
                resp.Add("nombrecategoria", cat.nombre);                


                obrasHelper obhelp = new obrasHelper();
                obras o = obhelp.getobrasById((int)cat.idobra);

                if (o == null)
                {
                    resp.Add("nombreobra", "No encontrada");
                }
                else {
                    resp.Add("nombreobra", o.nombre);
                }

                return resp;
               
            }
            catch (Exception e)
            {
                clsError objer = new clsError();
                objer.error = "Error al tratar de obtener los datos " + e.ToString();
                objer.result = 0;
                return objer;
            }
        }

        [Authorize]
        [HttpPost]
        [Route("getTareaById")]
        public Object getTareaById(JObject json)
        {
            try
            {
                String idtarea = json["idtarea"].ToString();
                clsTareaPlanificador objtar = new clsTareaPlanificador();
                tareasPlanificadorHelper thelp = new tareasPlanificadorHelper();
                obrasHelper ohelp = new obrasHelper();
                categoriasPlanificadorHelper chelp = new categoriasPlanificadorHelper();
                TareasPlanificador tar = thelp.getTareasPlanificadorById(Int32.Parse(idtarea));

                if (tar == null)
                {
                    clsError objer = new clsError();
                    objer.error = "Error no se ha encontrado la tarea requerida";
                    objer.result = 0;
                    return objer;
                }

                objtar.idcategoria = tar.idcategoria.ToString();
                objtar.idtarea = tar.idtarea.ToString();
                objtar.titulo = tar.titulo;
                objtar.todoDia = tar.todoDia.ToString();
                DateTime dtini = (DateTime) tar.fechainicio;
                String mes = dtini.Month.ToString().Length < 2 ? "0" + dtini.Month.ToString() : dtini.Month.ToString();
                String dia = dtini.Day.ToString().Length < 2 ? "0" + dtini.Day.ToString() : dtini.Day.ToString();
                objtar.inicio = dtini.Year + "-" + mes + "-" + dia + " " + tar.horainicio.ToString().Substring(0,8);

                DateTime dtfin = (DateTime)tar.fechafin;
                String mesfin = dtfin.Month.ToString().Length < 2 ? "0" + dtfin.Month.ToString() : dtfin.Month.ToString();
                String diafin = dtfin.Day.ToString().Length < 2 ? "0" + dtfin.Day.ToString() : dtfin.Day.ToString();
                objtar.fin = dtfin.Year + "-" + mesfin + "-" + diafin + " " + tar.horafin.ToString().Substring(0, 8);

                objtar.porcentaje = Math.Round((double)tar.porcentaje, 2).ToString().Replace(',', '.');
                objtar.notas = tar.notas;

                categoriasPlanificador cat = chelp.getcategoriasPlanificadorById((long)tar.idcategoria);
                if (cat != null) {
                    objtar.nombrecategoria = cat.nombre;
                    obras ob = ohelp.getobrasById((int)cat.idobra);
                    if (ob != null) {
                        objtar.idobra = ob.idobra.ToString();
                        objtar.nombreobra = ob.nombre;
                    }

                }

                return objtar;

            }catch (Exception e) {
                clsError objer = new clsError();
                objer.error = "Error al tratar de obtener la tarea " + e.ToString();
                objer.result = 0;
                return objer;
            }
        }

        [Authorize]
        [HttpPost]
        [Route("getObrasListado")]
        public Object getObrasListado()
        {
            try
            {
                obrasHelper obhelper = new obrasHelper();
                categoriasPlanificadorHelper cathelp = new categoriasPlanificadorHelper();


                List<obras> lstob = obhelper.getTodasobras();
                List<Dictionary<String, String>> obras = new List<Dictionary<string, string>>();
                
                if (lstob.Count == 0)
                {
                    clsError objer = new clsError();
                    objer.error = "No se encontraron obras";
                    objer.result = 0;
                    return objer;
                }

                foreach (obras ob in lstob)
                {
                    Dictionary<String, String> respob = new Dictionary<string, string>();
                    respob.Add("idobra", ob.idobra.ToString());
                    respob.Add("nombre", ob.nombre);
                    int nocat = cathelp.getNoCategoriasByObra(ob.idobra);
                    respob.Add("noclasificaciones", nocat.ToString());
                    double porcentaje_obra = obhelper.getPorcentajeObra(ob.idobra);
                    respob.Add("porcentaje", Math.Round(porcentaje_obra, 2).ToString().Replace(',', '.'));
                    obras.Add(respob);
                }

                Dictionary<String, List<Dictionary<String, String>>> resp = new Dictionary<string, List<Dictionary<string, string>>>();
                resp.Add("obras", obras);

                return resp;
            }
            catch (Exception e) {
                clsError objer = new clsError();
                objer.error = "Error al traer las obras actuales " + e.ToString();
                objer.result = -1;
                return objer;
            }
            

            
        }

        [Authorize]
        [HttpPost]
        [Route("BuscaObras")]
        public Object BuscaObras(JObject json)
        {
            try
            {
                String strBusqueda = json["strBusqueda"].ToString();


                obrasHelper obhelper = new obrasHelper();
                categoriasPlanificadorHelper cathelp = new categoriasPlanificadorHelper();


                List<obras> lstob = obhelper.getObrasBySearch(strBusqueda);
                List<Dictionary<String, String>> obras = new List<Dictionary<string, string>>();

                if (lstob.Count == 0) {
                    clsError objer = new clsError();
                    objer.error = "No se encontraron obras";
                    objer.result = 0;
                    return objer;
                }

                foreach (obras ob in lstob)
                {
                    Dictionary<String, String> respob = new Dictionary<string, string>();
                    respob.Add("idobra", ob.idobra.ToString());
                    respob.Add("nombre", ob.nombre);
                    int nocat = cathelp.getNoCategoriasByObra(ob.idobra);
                    respob.Add("noclasificaciones", nocat.ToString());
                    double porcentaje_obra = obhelper.getPorcentajeObra(ob.idobra);
                    respob.Add("porcentaje", Math.Round(porcentaje_obra, 2).ToString().Replace(',', '.'));
                    obras.Add(respob);
                }

                Dictionary<String, List<Dictionary<String, String>>> resp = new Dictionary<string, List<Dictionary<string, string>>>();
                resp.Add("obras", obras);

                return resp;
            }
            catch (Exception e)
            {
                clsError objer = new clsError();
                objer.error = "Error al traer las obras actuales " + e.ToString();
                objer.result = -1;
                return objer;
            }



        }

        [Authorize]
        [HttpPost]
        [Route("getObraById")]
        public Object getObraById(JObject json)
        {
            try
            {
                String idobra = json["idobra"].ToString();
                obrasHelper ohelp = new obrasHelper();

                obras ob = ohelp.getobrasById(Int32.Parse(idobra));

                if (ob != null)
                {
                    clsObras objobra = new clsObras();
                    objobra.idobra = ob.idobra.ToString();
                    objobra.nombre = ob.nombre;
                    objobra.descripcion = ob.descripcion;

                    return objobra;
                }
                else
                {
                    clsError objer = new clsError();
                    objer.error = "No se encontro la obra requerida";
                    objer.result = 0;
                    return objer;
                }
            }
            catch (Exception e) {
                clsError objex = new clsError();
                objex.error = "Error al traer las obras actuales " + e.ToString();
                objex.result = 0;
                return objex;
            }

            
        }

        [Authorize]
        [HttpPost]
        [Route("getCategoriaById")]
        public Object getCategoriaById(JObject json)
        {
            try
            {
                String idcategoria = json["idcategoria"].ToString();
                categoriasPlanificadorHelper cathelp = new categoriasPlanificadorHelper();

                categoriasPlanificador cat = cathelp.getcategoriasPlanificadorById(Int32.Parse(idcategoria));

                if (cat != null)
                {
                    clsCategoriasPlanificador objcat = new clsCategoriasPlanificador();
                    objcat.idcategoria = cat.idcategoria.ToString();
                    objcat.nombre = cat.nombre;
                    objcat.comentario = cat.comentario;
                    DateTime fech = (DateTime) cat.fecha;
                    string mes = fech.Month.ToString().Length < 2 ? "0" + fech.Month.ToString() : fech.Month.ToString();
                    string dia = fech.Day.ToString().Length < 2 ? "0" + fech.Day.ToString() : fech.Day.ToString();
                    objcat.fechahora = fech.Year + "-" + mes + "-" + dia + " " + cat.hora.ToString().Substring(0, 8);
                    objcat.idusuario = (int)cat.idusuario;

                    return objcat;
                }
                else
                {
                    clsError objer = new clsError();
                    objer.error = "No se encontro la categoria requerida";
                    objer.result = 0;
                    return objer;
                }
            }
            catch (Exception e)
            {
                clsError objex = new clsError();
                objex.error = "Error al traer los datos de la categoria " + e.ToString();
                objex.result = 0;
                return objex;
            }


        }

        [Authorize]
        [HttpPost]
        [Route("NuevaObra")]
        public Object NuevaObra(JObject json)
        {
            try
            {

                String nombre = json["nombre"].ToString();
                String descripcion = json["descripcion"].ToString();

                obrasHelper obHelp = new obrasHelper();
                obras ob = new obras();
                ob.nombre = nombre;
                ob.descripcion = descripcion;
                obHelp.insertObra(ob);

                clsError objer = new clsError();
                objer.error = "Se ha insertado la obra";
                objer.result = 1;
                return objer;
            }
            catch (Exception e)
            {
                clsError objex = new clsError();
                objex.error = "Error al traer las obras actuales " + e.ToString();
                objex.result = 0;
                return objex;
            }
        }

        [Authorize]
        [HttpPost]
        [Route("NuevaCategoria")]
        public Object NuevaCategoria(JObject json)
        {
            try
            {

                String nombre = json["nombre"].ToString();
                String comentarios = json["comentarios"].ToString();
                String idobra = json["idobra"].ToString();
                String idusuario = json["idusuario"].ToString();

                categoriasPlanificadorHelper catHelp = new categoriasPlanificadorHelper();
                categoriasPlanificador cat = new categoriasPlanificador();
                cat.nombre = nombre;
                cat.comentario = comentarios;
                cat.idobra = Int32.Parse(idobra);
                cat.idusuario = Int32.Parse(idusuario);
                catHelp.insertcategoriasPlanificador(cat);

                clsError objer = new clsError();
                objer.error = "Se ha insertado la categoria";
                objer.result = 1;
                return objer;
            }
            catch (Exception e)
            {
                clsError objex = new clsError();
                objex.error = "Error insertar la categoria " + e.ToString();
                objex.result = 0;
                return objex;
            }
        }

        [Authorize]
        [HttpPost]
        [Route("ModificaCategoria")]
        public Object ModificaCategoria(JObject json)
        {
            try
            {

                String nombre = json["nombre"].ToString();
                String comentarios = json["comentarios"].ToString();
                String idcategoria = json["idcategoria"].ToString();
                String idusuario = json["idusuario"].ToString();

                categoriasPlanificadorHelper catHelp = new categoriasPlanificadorHelper();
                categoriasPlanificador cat = new categoriasPlanificador();
                cat.nombre = nombre;
                cat.comentario = comentarios;
                cat.idcategoria = Int32.Parse(idcategoria);
                cat.idusuario = Int32.Parse(idusuario);
                catHelp.updatecategoriasPlanificador(cat);

                clsError objer = new clsError();
                objer.error = "Se ha modificado la categoria";
                objer.result = 1;
                return objer;
            }
            catch (Exception e)
            {
                clsError objex = new clsError();
                objex.error = "Error al modificar la categoria " + e.ToString();
                objex.result = 0;
                return objex;
            }
        }

        [Authorize]
        [HttpPost]
        [Route("ModificaObra")]
        public Object ModificaObra(JObject json)
        {
            try
            {
                String idobra = json["idobra"].ToString();
                String nombre = json["nombre"].ToString();
                String descripcion = json["descripcion"].ToString();

                obrasHelper obHelp = new obrasHelper();
                obras ob = new obras();
                ob.idobra = Int32.Parse(idobra);
                ob.nombre = nombre;
                ob.descripcion = descripcion;
                String resp = obHelp.updateobras(ob);

                if (!resp.Equals("")) {
                    clsError objer = new clsError();
                    objer.error = "Error al actualizar los datos " + resp;
                    objer.result = 0;
                    return objer;
                }

                clsError objresp = new clsError();
                objresp.error = "Se ha modificado la obra";
                objresp.result = 1;
                return objresp;
            }
            catch (Exception e)
            {
                clsError objex = new clsError();
                objex.error = "Error al Modificar la obra " + e.ToString();
                objex.result = 0;
                return objex;
            }
        }

        [Authorize]
        [HttpPost]
        [Route("EliminarCategoria")]
        public Object EliminarCategoria(JObject json)
        {
            try
            {
                String idcategoria = json["idcategoria"].ToString();

                categoriasPlanificadorHelper catHelp = new categoriasPlanificadorHelper();
                String resp = catHelp.deleteCategoria(Int64.Parse(idcategoria));

                if (!resp.Equals(""))
                {
                    clsError objer = new clsError();
                    objer.error = "Error al borrar los datos " + resp;
                    objer.result = 0;
                    return objer;
                }

                clsError objresp = new clsError();
                objresp.error = "Se ha eliminado la categoria";
                objresp.result = 1;
                return objresp;
            }
            catch (Exception e)
            {
                clsError objex = new clsError();
                objex.error = "Error al tratar de eliminar la categoria " + e.ToString();
                objex.result = 0;
                return objex;
            }
        }

        [Authorize]
        [HttpPost]
        [Route("EliminarTarea")]
        public Object EliminarTarea(JObject json)
        {
            try
            {
                String idtarea = json["idtarea"].ToString();

                tareasPlanificadorHelper tHelp = new tareasPlanificadorHelper();
                String resp = tHelp.deleteTarea(Int64.Parse(idtarea));

                if (!resp.Equals(""))
                {
                    clsError objer = new clsError();
                    objer.error = "Error al borrar los datos " + resp;
                    objer.result = 0;
                    return objer;
                }

                clsError objresp = new clsError();
                objresp.error = "Se ha eliminado la tarea";
                objresp.result = 1;
                return objresp;
            }
            catch (Exception e)
            {
                clsError objex = new clsError();
                objex.error = "Error al tratar de eliminar la tarea " + e.ToString();
                objex.result = 0;
                return objex;
            }
        }

        [Authorize]
        [HttpPost]
        [Route("EliminarObra")]
        public Object EliminarObra(JObject json)
        {
            try
            {
                String idobra = json["idobra"].ToString();

                obrasHelper obHelp = new obrasHelper();
                String resp = obHelp.deleteObra(Int32.Parse(idobra));

                if (!resp.Equals(""))
                {
                    clsError objer = new clsError();
                    objer.error = "Error al borrar los datos " + resp;
                    objer.result = 2;
                    return objer;
                }

                clsError objresp = new clsError();
                objresp.error = "Se ha eliminado la obra";
                objresp.result = 1;
                return objresp;
            }           
            catch (Exception e)
            {
                clsError objex = new clsError();
                objex.error = "Error al tratar de eliminar la obra " + e.ToString();
                objex.result = 0;
                return objex;
            }
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

            if (!resp.Equals(""))
            {
                clsError objerr = new clsError();
                objerr.error = "Error " + resp;
                objerr.result = 1;
                return objerr;              
            }

            //Envio de correo a usuarios
            usuariosHelper ushelp = new usuariosHelper();
            if (intnotificaasistentes == 1)
            {

                try
                {
                    String titulomsg = "Union a evento " + titulo;
                    String bodymsg = "Usted ha sido registrado para participar en el evento " + titulo + " que tendra lugar del " + strfechaini + " " + strhoraini + " a " + strfechafin + " " + strhorafin + "\n\r\n\r" +
                                      "Notas : " + notas + "\n\r\n\r" +
                                      "Para poder interactuar con dicho evento por favor ingrese a la aplicacion iphone e ingrese a Control de Obra y despues a la opcion Agenda";


                    String strsmtp = "mail.proyextra.com";
                    String emisor = "icom@proyextra.com";                    
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
                    msg.Subject=  titulomsg;
                    msg.Body = bodymsg;

                    foreach (int idus in usuarios)
                    {
                        usuarios us = ushelp.getUsuarioByID(idus);
                        if (us == null) { continue; }
                        if (us.mail.Equals("")) { continue; }
                        msg.To.Add(us.mail);
                        
                    }
                    
                    funciones.sendMail(msg, strsmtp, emisor, pass, puerto, blnssl);

                }
                catch (Exception e)
                {
                    clsError objerr = new clsError();
                    objerr.error = "Se ha guardado el evento sin embago existio un error al mandar el correo a los usuarios " + e.ToString();
                    objerr.result = 0;
                    return objerr;
                }
            }

            clsError objresp = new clsError();
            objresp.error = "Evento guardado con exito";
            objresp.result = 0;
            return objresp;
        }
    }
}
