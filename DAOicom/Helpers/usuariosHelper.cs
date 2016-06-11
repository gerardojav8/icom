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
