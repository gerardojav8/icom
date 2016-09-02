using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOicom.Helpers
{
    public class categoriasPlanificadorHelper
    {
        private icomEntities db = new icomEntities();
        public void insertcategoriasPlanificador(categoriasPlanificador objcategoriasPlanificador)
        {
            db.categoriasPlanificador.Add(objcategoriasPlanificador);
            db.SaveChanges();
        }

        public List<categoriasPlanificador> getTodascategoriasPlanificador()
        {
            var query = from tf in db.categoriasPlanificador
                        select tf;

            List<categoriasPlanificador> lstcategoriasPlanificador = new List<categoriasPlanificador>();
            lstcategoriasPlanificador.AddRange(query.ToList());
            return lstcategoriasPlanificador;
        }

        public categoriasPlanificador getcategoriasPlanificadorById(long id)
        {
            var query = from t in db.categoriasPlanificador
                        where t.idcategoria == id
                        select t;

            if (query.Count() > 0)
            {
                return query.First();
            }
            else
            {
                return null;
            }
        }

        public String updatecategoriasPlanificador(categoriasPlanificador obj)
        {
            categoriasPlanificador objtf = (from t in db.categoriasPlanificador
                                        where t.idcategoria == obj.idcategoria
                                        select t).FirstOrDefault();

            objtf.comentario = obj.comentario;
            objtf.nombre = obj.nombre;
            objtf.fecha = obj.fecha;
            objtf.hora = obj.hora;
            objtf.idusuario = obj.idusuario;
           

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
