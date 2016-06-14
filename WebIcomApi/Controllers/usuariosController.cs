using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using DAOicom.Helpers;
using DAOicom;
using Newtonsoft.Json;
using WebIcomApi.Entidades;


namespace WebIcomApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("usuarios")]
    public class usuariosController : ApiController
    {
        [Authorize]
        [HttpPost]
        [Route("getUsuarioById")]
        public String getUsarioById(JObject json)
        {
            usuariosHelper objushelp = new usuariosHelper();
            String id = json["id"].ToString();

            usuarios objus = objushelp.getUsuarioByID(Int32.Parse(id));

            if (objus == null)
            {
                return "usuario no encontrado";
            }
            else
            {
                return "bienvendio " + objus.nombre + " " + objus.apepaterno + " " + objus.apematerno;
            }
        }

        [Authorize]
        [HttpPost]
        [Route("getUsuarioByuserAndpass")]
        public string getUsuarioByuserAndpass(JObject json)
        {
            usuariosHelper objushelp = new usuariosHelper();
            String usuario = json["usuario"].ToString();
            String pass = json["pass"].ToString();

            usuarios objus = objushelp.getUsuarioByUsuarioyPassword(usuario, pass);

            if (objus == null)
            {
                clsError objerr = new clsError();
                objerr.error = "No se ha encontrado el usuario";
                objerr.result = 0;
                return JsonConvert.SerializeObject(objerr);
            }
            else
            {
                clsUsuarios objusuarios = new clsUsuarios(objus);
                return JsonConvert.SerializeObject(objusuarios) ;
            }

        }

        
    }
}
