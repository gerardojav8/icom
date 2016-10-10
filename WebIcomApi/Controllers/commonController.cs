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
    [RoutePrefix("common")]
    public class commonController : ApiController
    {
        [Authorize]
        [HttpPost]
        [Route("getFechaHoraActual")]
        public Object getFechaHoraActual()
        {
            try
            {
                clsfechahora objfechho = new clsfechahora();
                DateTime fechahoraact = DateTime.Now;
                String strhora = fechahoraact.TimeOfDay.ToString();
                String strfecha = fechahoraact.Year.ToString() + "-" + fechahoraact.Month.ToString() + "-" + fechahoraact.Day.ToString();

                objfechho.hora = strhora;
                objfechho.Fecha = strfecha;

                return objfechho;
            }
            catch (Exception e)
            {
                clsError objerr = new clsError();
                objerr.error = "No se ha podido obtener la fecha y hora actual";
                objerr.result = 0;
                return objerr;
            }
        }

        [Authorize]
        [HttpPost]
        [Route("cambiarPass")]
        public Object cambiarPass(JObject json)
        {
            try
            {
                String idusuario = json["idusuario"].ToString();
                String newpass = json["newpass"].ToString();
                String oldpass = json["oldpass"].ToString();

                usuariosHelper ushelp = new usuariosHelper();
                usuarios us = ushelp.getUsuarioByID(Int32.Parse(idusuario));

                if (us == null) {
                    clsError objerr = new clsError();
                    objerr.error = "No se ha podido obtener los datos para el usuario";
                    objerr.result = 1;
                    return objerr;
                }

                if (!us.passapp.ToLower().Equals(oldpass.ToLower())) {
                    clsError objerr = new clsError();
                    objerr.error = "El password anterior no coincide, verifiquelo por favor";
                    objerr.result = 1;
                    return objerr;
                }

                us.passapp = newpass;

                String resp = ushelp.updateUsuario(us);
                if (!resp.Equals("")) {
                    clsError objerr = new clsError();
                    objerr.error = "Ha existido un error al momento de modificar el password";
                    objerr.result = 1;
                    return objerr;
                }

                Dictionary<String, String> dicresp = new Dictionary<string, string>();
                dicresp.Add("respuesta", "cambio de password exitoso");
                return dicresp;
            }
            catch (Exception e)
            {
                clsError objerr = new clsError();
                objerr.error = "No se ha podido obtener la fecha y hora actual";
                objerr.result = 0;
                return objerr;
            }
        }
    }
}
