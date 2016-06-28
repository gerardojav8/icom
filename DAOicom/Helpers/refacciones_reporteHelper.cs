using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOicom.Helpers
{
    public class refacciones_reporteHelper
    {
        private icomEntities db = new icomEntities();
        public String insertaRefacciones_reporte(refacciones_reporte objrefacciones_reporte)
        {
            try
            {
                db.refacciones_reporte.Add(objrefacciones_reporte);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return e.ToString();
            }

            return "";
        }

        public List<refacciones_reporte> getTodasrefacciones_reporte()
        {
            var query = from rr in db.refacciones_reporte
                        select rr;

            List<refacciones_reporte> lstrefacciones_reporte = new List<refacciones_reporte>();
            lstrefacciones_reporte.AddRange(query.ToList());
            return lstrefacciones_reporte;
        }

        public refacciones_reporte getrefacciones_reporteByFolioyNumero(int foliorep, int num)
        {
            var query = from rr in db.refacciones_reporte
                        where rr.folio_reporte == foliorep && rr.folio_reporte == num
                        select rr;

            if (query.Count() > 0)
            {
                return query.First();
            }
            else
            {
                return null;
            }
        }

        public List<refacciones_reporte> getRefacciones_reporteByFolio(int foliorep)
        {
            var query = from rr in db.refacciones_reporte
                        where rr.folio_reporte == foliorep
                        select rr;

            List<refacciones_reporte> lstrefacciones_reporte = new List<refacciones_reporte>();
            lstrefacciones_reporte.AddRange(query.ToList());
            return lstrefacciones_reporte;
        }

        public String updaterefacciones_reporte(refacciones_reporte obj)
        {
            refacciones_reporte objrr = (from rr in db.refacciones_reporte
                                where rr.folio_reporte == obj.folio_reporte && rr.no_refaccion == obj.no_refaccion
                                select rr).FirstOrDefault();

            objrr.nombre = obj.nombre;            

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
