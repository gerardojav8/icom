using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOicom.Helpers
{
    public class chatEventosHelper
    {
        private icomEntities db = new icomEntities();
        public void insertTipoFalla(chat_eventos objchat_eventos)
        {
            db.chat_eventos.Add(objchat_eventos);
            db.SaveChanges();
        }

        public List<chat_eventos> getTodaschat_eventos()
        {
            var query = from tf in db.chat_eventos
                        select tf;

            List<chat_eventos> lstchat_eventos = new List<chat_eventos>();
            lstchat_eventos.AddRange(query.ToList());
            return lstchat_eventos;
        }

        public List<chat_eventos> getMensajesByMenosDiasAndIdEvento(int dias, int idevento)
        {

            DateTime dthoy = DateTime.Today;
            DateTime dtAnterior = dthoy.AddDays(-dias);

            var query = from cg in db.chat_eventos
                        where cg.fecha >= dtAnterior && cg.idevento == idevento
                        orderby cg.fecha, cg.hora descending
                        select cg;


            if (query.Count() > 0)
            {
                List<chat_eventos> lstchat_eventos = new List<chat_eventos>();
                lstchat_eventos.AddRange(query.ToList());
                return lstchat_eventos;
            }
            else
            {
                return null;
            }
        }

        public chat_eventos getchat_eventosById(int idevento, int idmensaje)
        {
            var query = from t in db.chat_eventos
                        where t.idevento == idevento && t.idmensaje == idmensaje
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

        public String updatechat_eventos(chat_eventos obj)
        {
            chat_eventos objce = (from t in db.chat_eventos
                                where t.idevento == obj.idevento && t.idmensaje == obj.idmensaje
                                select t).FirstOrDefault();

            objce.fecha = obj.fecha;
            objce.hora = obj.hora;
            objce.comentario = obj.comentario;
            objce.idusuario = obj.idusuario;
            
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
