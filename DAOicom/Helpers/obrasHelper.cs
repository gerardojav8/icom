using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOicom.Helpers
{
    class obrasHelper
    {
        private icomEntities db = new icomEntities();
        public void insertObra(obras objareaobra)
        {
            db.obras.Add(objareaobra);
            db.SaveChanges();
        }

        public List<obras> getTodasobras()
        {
            var query = from a in db.obras
                        select a;

            List<obras> lstobras = new List<obras>();
            lstobras.AddRange(query.ToList());
            return lstobras;
        }

        public obras getobrasById(int id)
        {
            var query = from a in db.obras
                        where a.idobra == id
                        select a;

            if (query.Count() > 0)
            {
                return query.First();
            }
            else
            {
                return null;
            }
        }

        public String updateobras(obras obj)
        {
            obras objtf = (from a in db.obras
                               where a.idobra == obj.idobra
                               select a).FirstOrDefault();

            objtf.nombre = obj.nombre;
            objtf.descripcion = obj.descripcion;
            objtf.idareaobra = obj.idareaobra;

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
