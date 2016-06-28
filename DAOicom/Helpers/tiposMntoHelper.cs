using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOicom.Helpers
{
    public class tiposMntoHelper
    {
        private icomEntities db = new icomEntities();
        public void insertTipoFalla(tipomantenimientos objtipomnto)
        {
            db.tipomantenimientos.Add(objtipomnto);
            db.SaveChanges();
        }

        public List<tipomantenimientos> getTipoMntos()
        {
            var query = from tm in db.tipomantenimientos
                        select tm;

            List<tipomantenimientos> lsttipomntos = new List<tipomantenimientos>();
            lsttipomntos.AddRange(query.ToList());
            return lsttipomntos;
        }

        public tipomantenimientos getTipoMntoByID(int id)
        {
            var query = from t in db.tipomantenimientos
                        where t.idtipomto == id
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

        public String updateTipoMnto(tipomantenimientos obj)
        {
            tipomantenimientos objtm = (from t in db.tipomantenimientos
                                where t.idtipomto == obj.idtipomto
                                select t).FirstOrDefault();

            objtm.nombre = obj.nombre;
            objtm.descripcion = obj.descripcion;

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
