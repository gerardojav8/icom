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
        public Object getUsuarioByuserAndpass(JObject json)
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
                return objerr;
            }
            else
            {
                clsUsuarios objusuarios = new clsUsuarios(objus);
                return objusuarios ;
            }

        }

        [Authorize]
        [HttpPost]
        [Route("getCmbUsuarios")]
        public Object getCmbUsuarios()
        {
            usuariosHelper objushelp = new usuariosHelper();
           
            List<usuarios> lstusuarios = objushelp.getTodosUsuarios();

            if (lstusuarios.Count == 0)
            {
                clsError objerr = new clsError();
                objerr.error = "No se han encontrado usuarios";
                objerr.result = 0;
                return objerr;
            }
            else
            {
                List<clsCmbUsuarios> lstcmbusuarios = new List<clsCmbUsuarios>();
                foreach (usuarios us in lstusuarios)
                {
                    clsCmbUsuarios objcmbus = new clsCmbUsuarios();
                    objcmbus.idusuario = us.idusuario;
                    objcmbus.nombre = us.nombre;
                    objcmbus.apepaterno = us.apepaterno;
                    objcmbus.apematerno = us.apematerno;

                    lstcmbusuarios.Add(objcmbus);
                }
                return lstcmbusuarios;
            }

        }

        [Authorize]
        [HttpPost]
        [Route("getUsuariosSearch")]
        public Object getUsuariosSearch(JObject json)
        {
            usuariosHelper objushelp = new usuariosHelper();            
            String nombre = json["nombre"].ToString();
            

            List<usuarios> lstusuarios = objushelp.searchUsuario(nombre);

            if (lstusuarios == null)
            {
                clsError objerr = new clsError();
                objerr.error = "No se han encontrado usuarios";
                objerr.result = 0;
                return objerr;
            }
            else
            {
                List<clsCmbUsuarios> lstcmbusuarios = new List<clsCmbUsuarios>();
                foreach (usuarios us in lstusuarios)
                {
                    clsCmbUsuarios objcmbus = new clsCmbUsuarios();
                    objcmbus.idusuario = us.idusuario;
                    objcmbus.nombre = us.nombre;
                    objcmbus.apepaterno = us.apepaterno;
                    objcmbus.apematerno = us.apematerno;

                    lstcmbusuarios.Add(objcmbus);
                }
                return lstcmbusuarios;
            }

        }

        
    }
}
