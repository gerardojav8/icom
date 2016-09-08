using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOicom.Helpers
{
    public class solicitudMaquinariaHelper
    {
        private icomEntities db = new icomEntities();
        public String insertsolicitudMaquinaria(solicitudmaquinaria objareaobra)
        {
            try
            {
                db.solicitudmaquinaria.Add(objareaobra);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return e.ToString();
            }

            return "";
        }

        public List<solicitudmaquinaria> getTodassolicitudmaquinaria()
        {
            var query = from a in db.solicitudmaquinaria
                        select a;

            List<solicitudmaquinaria> lstsolicitudmaquinaria = new List<solicitudmaquinaria>();
            lstsolicitudmaquinaria.AddRange(query.ToList());
            return lstsolicitudmaquinaria;
        }

        public solicitudmaquinaria getsolicitudmaquinariaById(int id)
        {
            var query = from a in db.solicitudmaquinaria
                        where a.folio == id
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

        public String getFolioSolicitud()
        {
            try
            {
                DateTime dthoy = DateTime.Today;
                String anioact = dthoy.Year.ToString();
                String mesact = dthoy.Month.ToString();
                if (mesact.Length < 2)
                {
                    mesact = "0" + mesact;
                }

                String strfechaini = anioact + "-" + mesact + "-01";
                DateTime fechaini = DateTime.ParseExact(strfechaini, "yyyy-MM-dd", System.Globalization.CultureInfo.InstalledUICulture);

                int intDiasMes = DateTime.DaysInMonth(Int32.Parse(anioact), Int32.Parse(mesact));
                String diasmeses = intDiasMes.ToString();
                if (diasmeses.Length < 2)
                {
                    diasmeses = "0" + diasmeses;
                }


                String strfechafin = anioact + "-" + mesact + "-" + diasmeses;
                DateTime fechafin = DateTime.ParseExact(strfechafin, "yyyy-MM-dd", System.Globalization.CultureInfo.InstalledUICulture);


                var query = from r in db.solicitudmaquinaria
                            where r.fecha >= fechaini && r.fecha <= fechafin
                            orderby r.folio descending
                            select r;

                String folio = "";

                if (query.Count() <= 0)
                {

                    folio = anioact + mesact + "001";
                }
                else
                {
                    solicitudmaquinaria objrep = query.FirstOrDefault();
                    String folioact = objrep.folio.ToString();

                    String consecutivoact = folioact.Substring(6);
                    int intcons = Int32.Parse(consecutivoact);
                    String strconsec = (intcons + 1).ToString();

                    switch (strconsec.Length)
                    {
                        case 1: strconsec = "00" + strconsec; break;
                        case 2: strconsec = "0" + strconsec; break;
                    }

                    folio = anioact + mesact + strconsec;

                }

                return folio;

            }
            catch (Exception e)
            {
                return "";
            }


        }

        public String updatesolicitudmaquinaria(solicitudmaquinaria obj)
        {
            solicitudmaquinaria objsm = (from a in db.solicitudmaquinaria
                           where a.folio == obj.folio
                           select a).FirstOrDefault();

            objsm.fecha = obj.fecha;
            objsm.time = obj.time;
            objsm.requeridopara = obj.requeridopara;
            objsm.idobra = obj.idobra;
            objsm.idresponsable = obj.idresponsable;

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

        public String GuardaSolicitud(solicitudmaquinaria obj, List<requerimientos_solicitudes> lstreq)
        {
            String resp = insertsolicitudMaquinaria(obj);

            if (resp != "")
            {
                return resp;
            }

            requerimientosSolicitudesHelper rshelp = new requerimientosSolicitudesHelper();
            foreach (requerimientos_solicitudes rs in lstreq)
            {
                resp = rshelp.insertrequerimientos_solicitudes(rs);
                if (resp != "")
                {
                    return "Error al insertar la reparacion " + resp;
                }
            }



            return "";
        }
    }
}
