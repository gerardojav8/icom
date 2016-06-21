using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOicom.Helpers
{
    public class reportesHelper
    {
        private icomEntities db = new icomEntities();
        public String insertaReporte(reportes objreporte)
        {
            try
            {
                String folio = getFolioReporte();
                objreporte.folio = Int32.Parse(folio);
                db.reportes.Add(objreporte);
                db.SaveChanges();
                return folio;
            }
            catch (Exception e)
            {
                return "";
            }

        }

        public List<tipofallas> getTipoFallas()
        {
            var query = from tf in db.tipofallas
                        select tf;

            List<tipofallas> lsttipofallas = new List<tipofallas>();
            lsttipofallas.AddRange(query.ToList());
            return lsttipofallas;
        }

        public List<reportes> getTodosReportes()
        {
            var query = from r in db.reportes
                        select r;

            List<reportes> lstreportes = new List<reportes>();
            lstreportes.AddRange(query.ToList());
            return lstreportes;
        }

        public reportes getReporteByFolio(int strfolio)
        {
            var query = from r in db.reportes
                        where r.folio == strfolio
                        select r;

            if (query.Count() > 0)
            {
                return query.First();
            }
            else
            {
                return null;
            }
        }

        public String getFolioReporte()
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
                

                String strfechafin = anioact + "-" + mesact + "-"+ diasmeses;
                DateTime fechafin = DateTime.ParseExact(strfechafin, "yyyy-MM-dd", System.Globalization.CultureInfo.InstalledUICulture);


                var query = from r in db.reportes
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
                    reportes objrep = query.FirstOrDefault();
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

        public int tieneMaquinaReporte(string strnoserie)
        {
            var query = from r in db.reportes
                        where r.no_serie == strnoserie && r.idstatus < 3
                        select r;

            if (query.Count() > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public String updateReporte(reportes obj)
        {
            reportes objreporte = (from r in db.reportes
                               where r.folio == obj.folio
                               select r).FirstOrDefault();

            objreporte.fecha = obj.fecha;
            objreporte.hora = obj.hora;
            objreporte.no_serie = obj.no_serie;
            objreporte.km_horometro = obj.km_horometro;
            objreporte.modelo = obj.modelo;
            objreporte.idreporto = obj.idreporto;
            objreporte.idtipofalla = obj.idtipofalla;
            objreporte.idatiende = obj.idatiende;
            objreporte.idrealizo = obj.idrealizo;
            objreporte.tiempo_reparacion = obj.tiempo_reparacion;
            objreporte.retraso = obj.retraso;
            objreporte.idtipomto = obj.idtipomto;
            objreporte.descripcion = obj.descripcion;
            objreporte.observaciones = obj.observaciones;
            objreporte.idstatus = obj.idstatus;

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
