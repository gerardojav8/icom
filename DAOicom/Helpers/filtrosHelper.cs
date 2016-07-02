using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOicom.Helpers
{
    public class filtrosHelper
    {
        private icomEntities db = new icomEntities();
        public void insertFiltros(filtros objfiltros)
        {
            db.filtros.Add(objfiltros);
            db.SaveChanges();
        }

        public List<filtros> getTodasfiltros()
        {
            var query = from tf in db.filtros
                        select tf;

            List<filtros> lstfiltros = new List<filtros>();
            lstfiltros.AddRange(query.ToList());
            return lstfiltros;
        }

        public filtros getfiltrosById(int id)
        {
            var query = from t in db.filtros
                        where t.idfiltro == id
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

        public String updatefiltros(filtros obj)
        {
            filtros objtf = (from t in db.filtros
                                where t.idfiltro == obj.idfiltro
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
