using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOicom.Helpers
{
    public class chatGeneralHelper
    {

        private icomEntities db = new icomEntities();
        public long insertChatGeneral(chat_general objchat_general)
        {
            try
            {
                db.chat_general.Add(objchat_general);
                db.SaveChanges();
                db.Entry(objchat_general).GetDatabaseValues();
                return objchat_general.idmensaje;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return -1;
            }
        }

        public List<chat_general> getTodaschat_general()
        {
            var query = from tf in db.chat_general
                        select tf;

            List<chat_general> lstchat_general = new List<chat_general>();
            lstchat_general.AddRange(query.ToList());
            return lstchat_general;
        }

        public List<chat_general> getMensajesByMenosDias(int dias) {

                DateTime dthoy = DateTime.Today;

                DateTime dtAnterior = dthoy.AddDays(-dias);

                var query = from cg in db.chat_general
                            where cg.fecha >= dtAnterior
                            orderby cg.fecha, cg.hora ascending
                            select cg;


                if (query.Count() > 0)
                {
                    List<chat_general> lstchat_general = new List<chat_general>();
                    lstchat_general.AddRange(query.ToList());
                    return lstchat_general;
                }
                else {
                    return null;
                }
        }

        public chat_general getchat_generalById(long id)
        {
            var query = from t in db.chat_general
                        where t.idmensaje == id
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

        

        public String updatechat_general(chat_general obj)
        {
            chat_general objcg = (from t in db.chat_general
                                where t.idmensaje == obj.idmensaje
                                select t).FirstOrDefault();

            objcg.idusuario = obj.idusuario;
            objcg.fecha = obj.fecha;
            objcg.hora = obj.hora;
            objcg.comentario = obj.comentario;
            objcg.archivo = obj.archivo;
            objcg.nombrearchivo = obj.nombrearchivo;

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
