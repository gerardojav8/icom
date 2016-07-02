using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOicom.Helpers
{
    public class medicionesFiltrosHelper
    {
        private icomEntities db = new icomEntities();
        public void insertMedicionesFiltro(medicionesfiltros objmedicionesfiltros)
        {
            db.medicionesfiltros.Add(objmedicionesfiltros);
            db.SaveChanges();
        }

        public List<medicionesfiltros> getTodasmedicionesfiltros()
        {
            var query = from tf in db.medicionesfiltros
                        select tf;

            List<medicionesfiltros> lstmedicionesfiltros = new List<medicionesfiltros>();
            lstmedicionesfiltros.AddRange(query.ToList());
            return lstmedicionesfiltros;
        }

        public List<medicionesfiltros> getTodasmedicionesfiltrosByNoSerie(String noserie)
        {
            var query = from tf in db.medicionesfiltros
                        where tf.noserie == noserie
                        select tf;

            List<medicionesfiltros> lstmedicionesfiltros = new List<medicionesfiltros>();
            lstmedicionesfiltros.AddRange(query.ToList());
            return lstmedicionesfiltros;
        }

        public medicionesfiltros getmedicionesfiltrosById(int id, String noserie)
        {
            var query = from t in db.medicionesfiltros
                        where t.idfiltro == id && t.noserie == noserie
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

        public String updatemedicionesfiltros(medicionesfiltros obj)
        {
            medicionesfiltros objtf = (from t in db.medicionesfiltros
                                where t.idfiltro == obj.idfiltro && t.noserie == obj.noserie
                                select t).FirstOrDefault();

            objtf.medicion = obj.medicion;
            objtf.comentario = obj.comentario;

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
