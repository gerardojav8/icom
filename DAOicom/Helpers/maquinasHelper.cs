using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOicom.Helpers
{
    public class maquinasHelper
    {
        private icomEntities db = new icomEntities();
        public void insertMaquina(maquinas objmaquina)
        {
            db.maquinas.Add(objmaquina);
            db.SaveChanges();
        }

        public List<maquinas> getTodasMaquinas()
        {
            var query = from m in db.maquinas
                        select m;

            List<maquinas> lstmaq = new List<maquinas>();
            lstmaq.AddRange(query.ToList());
            return lstmaq;
        }

        public maquinas getMaquinaByNoSerie(String noserie)
        {
            var query = from m in db.maquinas
                        where m.noserie == noserie
                        select m;

            if (query.Count() > 0)
            {
                return query.First();
            }
            else
            {
                return null;
            }
        }

        public String updateMaquina(maquinas obj)
        {
            maquinas objmaq = (from m in db.maquinas
                              where m.noserie == obj.noserie
                              select m).FirstOrDefault();

            objmaq.noeconomico = obj.noeconomico;
            objmaq.marca = obj.marca;
            objmaq.modelo = obj.modelo;
            objmaq.aniofabricacion = obj.aniofabricacion;
            objmaq.idequipo = obj.idequipo;
            objmaq.imagen = obj.imagen;
            
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
