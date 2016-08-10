using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOicom.Helpers
{
    public class eventosAgendaHelper
    {
        private icomEntities db = new icomEntities();
        public String inserteventosagenda(eventosagenda objeventosagenda, List<int> usuarios)
        {
            try
            {
                db.eventosagenda.Add(objeventosagenda);

                foreach (int idus in usuarios) {
                    var query = from u in db.usuarios
                                where u.idusuario == idus
                                select u;

                    if (query.Count() > 0)
                    {   
                        usuarios us = query.First();
                        objeventosagenda.usuarios.Add(us);
                    }                    
                }

                db.SaveChanges();
                return "";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public List<eventosagenda> getTodaseventosagenda()
        {
            var query = from tf in db.eventosagenda
                        select tf;

            List<eventosagenda> lsteventosagenda = new List<eventosagenda>();
            lsteventosagenda.AddRange(query.ToList());
            return lsteventosagenda;
        }

        public List<eventosagenda> getEventosAnioActualByIdUsuario(int idus)
        {
            DateTime dthoy = DateTime.Today;
            int anioact = dthoy.Year;
            
            usuarios us;
            var queryus = from u in db.usuarios
                        where u.idusuario == idus
                        select u;

            if (queryus.Count() > 0)
            {
                us = queryus.First();
            }
            else
            {
                return null;
            }
                                    
            var query = from ea in db.eventosagenda
                        where ea.anio == anioact
                        orderby ea.fechainicio ascending
                        select ea;

            if (query.Count() > 0)
            {
                List<eventosagenda> lsteventosagenda = new List<eventosagenda>();
                List<eventosagenda> lsteventosagendaresp = new List<eventosagenda>();
                lsteventosagenda.AddRange(query.ToList());
                foreach (eventosagenda evento in lsteventosagenda)
                {
                    if (evento.usuarios.Contains(us)) {
                        lsteventosagendaresp.Add(evento);
                    }
                }
                return lsteventosagendaresp;
            }
            else {
                return null;
            }

        }

        public eventosagenda geteventosagendaById(int id)
        {
            var query = from t in db.eventosagenda
                        where t.idevento == id
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

        public String updateeventosagenda(eventosagenda obj)
        {
            eventosagenda objea = (from t in db.eventosagenda
                                where t.idevento == obj.idevento
                                select t).FirstOrDefault();

            objea.mes = obj.mes;
            objea.anio = obj.anio;
            objea.titulo = obj.titulo;
            objea.comentario = obj.comentario;
            objea.horaini = obj.horaini;
            objea.horafin = obj.horafin;
            objea.fechainicio = obj.fechainicio;
            objea.fechafin = obj.fechafin;
            objea.diacompleto = obj.diacompleto;

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
