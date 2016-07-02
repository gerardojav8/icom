using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOicom.Helpers
{
    public class EstadosFisicoHelper
    {
        private icomEntities db = new icomEntities();
        public void insertmediciones(mediciones objmediciones)
        {               
            db.mediciones.Add(objmediciones);
            db.SaveChanges();
        }

        public List<mediciones> getTodasmediciones()
        {
            var query = from tf in db.mediciones
                        select tf;

            List<mediciones> lstmediciones = new List<mediciones>();
            lstmediciones.AddRange(query.ToList());
            return lstmediciones;
        }

        public List<mediciones> getMedicionesByNoSerie(String noserie)
        {
            var query = from tf in db.mediciones
                        where tf.noserie == noserie
                        select tf;

            List<mediciones> lstmediciones = new List<mediciones>();
            lstmediciones.AddRange(query.ToList());
            return lstmediciones;
        }

        public mediciones getmedicionesById(int id, String noserie)
        {
            var query = from t in db.mediciones
                        where t.idcomponente == id && t.noserie == noserie
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

        public String updatemediciones(mediciones obj)
        {
            mediciones objeq = (from t in db.mediciones
                                    where t.idcomponente == obj.idcomponente && t.noserie == obj.noserie
                                    select t).FirstOrDefault();

            objeq.marca = obj.marca;
            objeq.tipo = obj.tipo;
            objeq.capacidad = obj.capacidad;
            objeq.calificacion = obj.calificacion;
            objeq.comentario = obj.comentario;

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
