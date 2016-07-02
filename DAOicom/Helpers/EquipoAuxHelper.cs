using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOicom.Helpers
{
    public class EquipoAuxHelper
    {
        private icomEntities db = new icomEntities();
        public int insertequipoauxiliar(equipoauxiliar objequipoauxiliar)
        {
            db.equipoauxiliar.Add(objequipoauxiliar);
            db.SaveChanges();

            var query = from e in db.equipoauxiliar
                        orderby e.idequipo descending
                        select e;
            if (query.Count() > 0)
            {
                return query.First().idequipo;
            }
            else
            {
                return -1;
            }

        }

        public List<equipoauxiliar> getTodasequipoauxiliar()
        {
            var query = from tf in db.equipoauxiliar
                        select tf;

            List<equipoauxiliar> lstequipoauxiliar = new List<equipoauxiliar>();
            lstequipoauxiliar.AddRange(query.ToList());
            return lstequipoauxiliar;
        }

        public equipoauxiliar getequipoauxiliarById(int id)
        {
            var query = from t in db.equipoauxiliar
                        where t.idequipo == id
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

        public String updateequipoauxiliar(equipoauxiliar obj)
        {
            equipoauxiliar objeq = (from t in db.equipoauxiliar
                                where t.idequipo == obj.idequipo
                                select t).FirstOrDefault();

            objeq.marca = obj.marca;
            objeq.modelo = obj.modelo;
            objeq.noserie = obj.noserie;

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
