using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOicom.Helpers
{
    public class componentesHelper
    {
        private icomEntities db = new icomEntities();
        public void insertComponentes(componentes objcomponentes)
        {
            db.componentes.Add(objcomponentes);
            db.SaveChanges();
        }

        public List<componentes> getTodascomponentes()
        {
            var query = from tf in db.componentes
                        select tf;

            List<componentes> lstcomponentes = new List<componentes>();
            lstcomponentes.AddRange(query.ToList());
            return lstcomponentes;
        }

        public componentes getcomponentesById(int id)
        {
            var query = from t in db.componentes
                        where t.idcomponente == id
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

        public String updatecomponentes(componentes obj)
        {
            componentes objtf = (from t in db.componentes
                                where t.idcomponente == obj.idcomponente
                                select t).FirstOrDefault();

            objtf.nombre = obj.nombre;
            objtf.descripcion = obj.descripcion;

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
