using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOicom.Helpers
{
    public class areasObraHelper
    {
        private icomEntities db = new icomEntities();
        public void insertAreaObra(areasobra objareaobra)
        {
            db.areasobra.Add(objareaobra);
            db.SaveChanges();
        }

        public List<areasobra> getTodasAreasObra()
        {
            var query = from a in db.areasobra
                        select a;

            List<areasobra> lstareasobra = new List<areasobra>();
            lstareasobra.AddRange(query.ToList());
            return lstareasobra;
        }

        public areasobra getareasobraById(int id)
        {
            var query = from a in db.areasobra
                        where a.idareaobra == id
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

        public String updateareasobra(areasobra obj)
        {
            areasobra objtf = (from a in db.areasobra
                                where a.idareaobra == obj.idareaobra
                                select a).FirstOrDefault();

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
