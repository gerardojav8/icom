using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOicom.Helpers
{
    public class usuariosHelper
    {
        private icomEntities db = new icomEntities();
        public void insertUsuario(usuarios objusuario)
        {
            db.usuarios.Add(objusuario);
            db.SaveChanges();
        }

        public List<usuarios> getTodosUsuarios()
        {
            var query = from u in db.usuarios
                        select u;

            List<usuarios> lstusuarios = new List<usuarios>();
            lstusuarios.AddRange(query.ToList());
            return lstusuarios;
        }

        public usuarios getUsuarioByUsuarioyPassword(String strUsuario, String strPass)
        {
            var query = from u in db.usuarios
                        where u.usuario == strUsuario && u.passapp == strPass
                        select u;

            if (query.Count() > 0)
            {
                return query.First();
            }
            else
            {
                return null;
            }
        }

        public List<usuarios> searchUsuario(String nombre)
        {
            var query = from u in db.usuarios
                        where u.nombre.Contains(nombre) || u.apepaterno.Contains(nombre) || u.apematerno.Contains(nombre)
                        select u;

            if (query.Count() > 0)
            {
                List<usuarios> lstusuarios = new List<usuarios>();
                lstusuarios.AddRange(query.ToList());
                return lstusuarios;
            }
            else
            {
                return null;
            }
        }

        public usuarios getUsuarioByID(int id)
        {
            var query = from u in db.usuarios
                        where u.idusuario == id
                        select u;

            if (query.Count() > 0)
            {
                return query.First();
            }
            else
            {
                return null;
            }
        }

        public String updateUsuario(usuarios obj)
        {
            usuarios objus = (from u in db.usuarios
                              where u.idusuario == obj.idusuario
                              select u).FirstOrDefault();

            objus.nombre = obj.nombre;
            objus.apepaterno = obj.apepaterno;
            objus.apematerno = obj.apematerno;
            objus.telefono = obj.telefono;
            objus.mail = obj.mail;
            objus.edad = obj.edad;
            objus.idpuesto = obj.idpuesto;
            objus.passapp = obj.passapp;
            objus.usuario = obj.usuario;

            try
            {
                db.SaveChanges();
                return "";
            }
            catch (Exception e)
            {
                return e.ToString();
            }

        }
    }
}
