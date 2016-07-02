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
                        orderby m.noserie
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

        public String updateMaquina(maquinas obj, equipoauxiliar objea, List<mediciones> lstmed, List<medicionesfiltros> lstmedfil)
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
            objmaq.descripcion = obj.descripcion;
            
            try
            {
                

                EquipoAuxHelper eahelp = new EquipoAuxHelper();

                int idequipoinsert = -1;
                if (objea.idequipo == 0)
                {
                   idequipoinsert = eahelp.insertequipoauxiliar(objea);
                }
                else
                {
                    eahelp.updateequipoauxiliar(objea);
                }

                objmaq.idequipo = idequipoinsert;

                db.SaveChanges();

                EstadosFisicoHelper efhelp = new EstadosFisicoHelper();

                foreach (mediciones med in lstmed)
                {
                    mediciones medact = efhelp.getmedicionesById(med.idcomponente, med.noserie);
                    if (medact == null)
                    {
                        efhelp.insertmediciones(med);
                    }
                    else
                    {
                        efhelp.updatemediciones(med);
                    }

                }

                medicionesFiltrosHelper mfhelp = new medicionesFiltrosHelper();

                foreach (medicionesfiltros medfil in lstmedfil)
                {
                    medicionesfiltros medfilact = mfhelp.getmedicionesfiltrosById(medfil.idfiltro, medfil.noserie);
                    if (medfilact == null)
                    {
                        mfhelp.insertMedicionesFiltro(medfil);
                    }
                    else
                    {
                        mfhelp.updatemedicionesfiltros(medfil);
                    }
                }

                return "";
            }
            catch (Exception e)
            {
                return e.ToString();
            }

        }
    }
}
