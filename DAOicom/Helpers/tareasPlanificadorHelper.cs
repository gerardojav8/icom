using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOicom.Helpers
{
    public class tareasPlanificadorHelper
    {
        private icomEntities db = new icomEntities();
        public void insertTareasPlanificador(TareasPlanificador objTareasPlanificador)
        {
            db.TareasPlanificador.Add(objTareasPlanificador);
            db.SaveChanges();
        }

        public List<TareasPlanificador> getTodasTareasPlanificador()
        {
            var query = from tf in db.TareasPlanificador
                        select tf;

            List<TareasPlanificador> lstTareasPlanificador = new List<TareasPlanificador>();
            lstTareasPlanificador.AddRange(query.ToList());
            return lstTareasPlanificador;
        }

        public List<TareasPlanificador> getTareasPlanificadorByIdCategoria(long idcategoria)
        {
            var query = from tf in db.TareasPlanificador
                        where tf.idcategoria == idcategoria
                        select tf;

            List<TareasPlanificador> lstTareasPlanificador = new List<TareasPlanificador>();
            lstTareasPlanificador.AddRange(query.ToList());
            return lstTareasPlanificador;
        }

        public List<TareasPlanificador> getTodasTareasPlanificadorbyFecha(DateTime fecha)
        {
            var query = from tf in db.TareasPlanificador
                        where tf.fechainicio == fecha
                        select tf;

            List<TareasPlanificador> lstTareasPlanificador = new List<TareasPlanificador>();
            lstTareasPlanificador.AddRange(query.ToList());

            if (lstTareasPlanificador.Count > 0)
            {
                return lstTareasPlanificador;
            }
            else {
                return null;
            }
        }

        public int getNoTareasByIdCategoria(long idcategoria)
        {
            var query = from t in db.TareasPlanificador where t.idcategoria == idcategoria select t;


            if (query == null)
            {
                return 0;
            }
            else
            {
                return query.Count();
            }
        }

        public TareasPlanificador getTareasPlanificadorById(int id)
        {
            var query = from t in db.TareasPlanificador
                        where t.idtarea == id
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
       

        public String updateTareasPlanificador(TareasPlanificador obj)
        {
            TareasPlanificador objtf = (from t in db.TareasPlanificador
                             where t.idtarea == obj.idtarea
                             select t).FirstOrDefault();
            
            objtf.titulo = obj.titulo;
            objtf.todoDia = obj.todoDia;
            objtf.fechainicio = obj.fechainicio;
            objtf.horainicio = obj.horainicio;
            objtf.fechafin = obj.fechafin;
            objtf.horafin = obj.horafin;
            objtf.porcentaje = obj.porcentaje;
            objtf.horas = obj.horas;
            objtf.notas = obj.notas;

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
        public string deleteTarea(long idtarea)
        {
            var query = from a in db.TareasPlanificador
                        where a.idtarea == idtarea
                        select a;

            if (query.Count() > 0)
            {
                try
                {
                    db.TareasPlanificador.Remove(query.First());
                    db.SaveChanges();
                    return "";

                }catch (Exception e)
                {
                    return e.ToString();
                }
            }
            else
            {
                return "no se ha encontrado el registro a eliminar";
            }
        }
    }
}
