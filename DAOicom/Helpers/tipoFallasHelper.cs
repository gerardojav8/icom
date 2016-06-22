using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOicom.Helpers
{
    public class tipoFallasHelper
    {
        private icomEntities db = new icomEntities();
        public void insertTipoFalla(tipofallas objtipofallas)
        {
            db.tipofallas.Add(objtipofallas);
            db.SaveChanges();
        }

        public List<tipofallas> getTodasTipoFallas()
        {
            var query = from tf in db.tipofallas
                        select tf;

            List<tipofallas> lsttipofallas = new List<tipofallas>();
            lsttipofallas.AddRange(query.ToList());
            return lsttipofallas;
        }

        public tipofallas getTipoFallasById(int id)
        {
            var query = from t in db.tipofallas
                        where t.idtipofalla == id
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

        public String updateTipoFallas(tipofallas obj)
        {
            tipofallas objtf = (from t in db.tipofallas
                              where t.idtipofalla == obj.idtipofalla
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
