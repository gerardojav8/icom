using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOicom.Helpers
{
    public class requerimientosSolicitudesHelper
    {
        private icomEntities db = new icomEntities();
        public String insertrequerimientos_solicitudes(requerimientos_solicitudes objareaobra)
        {
            try
            {
                db.requerimientos_solicitudes.Add(objareaobra);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return e.ToString();
            }

            return "";
        }

        public List<requerimientos_solicitudes> getTodasrequerimientos_solicitudes()
        {
            var query = from a in db.requerimientos_solicitudes
                        select a;

            List<requerimientos_solicitudes> lstrequerimientos_solicitudes = new List<requerimientos_solicitudes>();
            lstrequerimientos_solicitudes.AddRange(query.ToList());
            return lstrequerimientos_solicitudes;
        }

        public requerimientos_solicitudes getrequerimientos_solicitudesByFolioAndNo(int folio, int no)
        {
            var query = from a in db.requerimientos_solicitudes
                        where a.foliosolicitud == folio && a.norequerimiento == no
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

        public String updaterequerimientos_solicitudes(requerimientos_solicitudes obj)
        {
            requerimientos_solicitudes objrs = (from a in db.requerimientos_solicitudes
                                         where a.foliosolicitud == obj.foliosolicitud && a.norequerimiento == obj.norequerimiento
                                         select a).FirstOrDefault();

            objrs.equipo = obj.equipo;
            objrs.marca = obj.marca;
            objrs.modelo = obj.modelo;
            

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
