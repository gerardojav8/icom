using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOicom.Helpers
{
    public class categoriasPlanificadorHelper
    {
        private icomEntities db = new icomEntities();
        public void insertcategoriasPlanificador(categoriasPlanificador objcategoriasPlanificador)
        {
            objcategoriasPlanificador.fecha = DateTime.Now;
            objcategoriasPlanificador.hora = DateTime.Now.TimeOfDay;
            db.categoriasPlanificador.Add(objcategoriasPlanificador);
            db.SaveChanges();
        }

        public List<categoriasPlanificador> getTodascategoriasPlanificador()
        {
            var query = from tf in db.categoriasPlanificador
                        select tf;

            List<categoriasPlanificador> lstcategoriasPlanificador = new List<categoriasPlanificador>();
            lstcategoriasPlanificador.AddRange(query.ToList());
            return lstcategoriasPlanificador;
        }

        public List<categoriasPlanificador> getCategoriasBySearch(String strBusqueda)
        {
            var query = from cat in db.categoriasPlanificador
                        where cat.nombre.Contains(strBusqueda)
                        select cat;

            List<categoriasPlanificador> lstcategoriasPlanificador = new List<categoriasPlanificador>();
            lstcategoriasPlanificador.AddRange(query.ToList());
            return lstcategoriasPlanificador;
        }

        public int getNoCategoriasByObra(long idobra) {
            var query = from cat in db.categoriasPlanificador where cat.idobra == idobra select cat;
            
            
            if (query == null)
            {
                return 0;
            }
            else
            {
                return query.Count();
            }
        }

        public double getPorcentajeCategoria(long idcategoria)
        {

            var cantidad_tareas_categoria = (from c in db.categoriasPlanificador 
                                            join t in db.TareasPlanificador on c.idcategoria equals t.idcategoria
                                            where c.idcategoria == idcategoria
                                            select t).Count();

            if (cantidad_tareas_categoria == 0)
            {
                return 0d;
            }

            var suma_porcentajes_tareas = (from c in db.categoriasPlanificador 
                                           join t in db.TareasPlanificador on c.idcategoria equals t.idcategoria
                                           where c.idcategoria == idcategoria
                                           select t.porcentaje).Sum();


            var total_porcentaje_completo = cantidad_tareas_categoria * 100;
            var porcentaje_actual = (suma_porcentajes_tareas * 100) / total_porcentaje_completo;


            return (double)porcentaje_actual;
        }

        public categoriasPlanificador getcategoriasPlanificadorById(long id)
        {
            var query = from t in db.categoriasPlanificador
                        where t.idcategoria == id
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

        public String updatecategoriasPlanificador(categoriasPlanificador obj)
        {
            categoriasPlanificador objtf = (from t in db.categoriasPlanificador
                                        where t.idcategoria == obj.idcategoria
                                        select t).FirstOrDefault();

            objtf.comentario = obj.comentario;
            objtf.nombre = obj.nombre;
            objtf.fecha = obj.fecha;
            objtf.hora = obj.hora;
            objtf.idusuario = obj.idusuario;
           

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

        public string deleteCategoria(long idcategoria)
        {
            var query = from a in db.categoriasPlanificador
                        where a.idcategoria == idcategoria
                        select a;

            if (query.Count() > 0)
            {
                try
                {
                    db.categoriasPlanificador.Remove(query.First());
                    db.SaveChanges();
                    return "";
                }
                catch (Exception e)
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
