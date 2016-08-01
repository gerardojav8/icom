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

namespace WebIcomApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("controldeobras")]
    public class controldeobraController : ApiController
    {
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
                    
                    lstmensajes.Add(obj);
                }


                return lstmensajes;

            }

        }

        [Authorize]
        [HttpPost]
        [Route("getListadoAgenda")]
        public Object getListadoAgenda()
        {
            eventosAgendaHelper eahelp = new eventosAgendaHelper();
            List<eventosagenda> lstea = eahelp.getEventosAnioActual();

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
                String strfechaini = fechaini.Year + "-" + fechaini.Month + "-" + fechaini.Day;

                DateTime fechafin = (DateTime)objea.fechafin;
                String strfechafin = fechafin.Year + "-" + fechafin.Month + "-" + fechafin.Day;

                String lapso = strfechaini + " " + objea.horaini.ToString() + " - " + strfechafin + " " + objea.horafin.ToString();

                objearesp.lapso = lapso;
                objearesp.titulo = objea.titulo;
                objearesp.comentario = objea.comentario;

                
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
