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

        public List<maquinas> getMaquinasByBusqueda(string busqueda)
        {
            int intbus;
            try
            {
                intbus = Int32.Parse(busqueda);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                intbus = -1;
            }

            IOrderedQueryable<maquinas> query;

            if (intbus >= 0)
            {
                query = from m in db.maquinas
                            where m.noserie.Contains(busqueda) || m.noeconomico == intbus ||
                                  m.marca.Contains(busqueda) || m.modelo.Contains(busqueda)
                            orderby m.noserie
                            select m;
            }
            else {
                query = from m in db.maquinas
                            where m.noserie.Contains(busqueda) ||
                                  m.marca.Contains(busqueda) || m.modelo.Contains(busqueda)
                            orderby m.noserie
                            select m;
            }

            if (query.Count() > 0)
            {
                List<maquinas> lstmaq = new List<maquinas>();
                lstmaq.AddRange(query.ToList());
                return lstmaq;
            }
            else 
            {
                return null;            
            }
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
                if (objea.idequipo == -2)
                {
                    idequipoinsert = -2;
                }
                else
                {
                    if (objea.idequipo == -1)
                    {
                        idequipoinsert = eahelp.insertequipoauxiliar(objea);
                    }
                    else
                    {
                        eahelp.updateequipoauxiliar(objea);
                        idequipoinsert = objea.idequipo;
                    }
                }

                if (idequipoinsert == -2)
                {
                    objmaq.idequipo = null;
                }
                else
                {
                    objmaq.idequipo = idequipoinsert;
                }

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
