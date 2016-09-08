using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOicom.Helpers
{
    public class obrasHelper
    {
        private icomEntities db = new icomEntities();
        public void insertObra(obras objareaobra)
        {
            db.obras.Add(objareaobra);
            db.SaveChanges();
        }

        public List<obras> getTodasobras()
        {
            var query = from a in db.obras
                        select a;

            List<obras> lstobras = new List<obras>();
            lstobras.AddRange(query.ToList());
            return lstobras;
        }

        public List<obras> getObrasBySearch(String search)
        {
            var query = from a in db.obras
                        where a.nombre.Contains(search) || a.descripcion.Contains(search)
                        select a;

            List<obras> lstobras = new List<obras>();
            lstobras.AddRange(query.ToList());
            return lstobras;
        }

        public obras getobrasById(int id)
        {
            var query = from a in db.obras
                        where a.idobra == id
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

       public double getPorcentajeObra(long idobra){

           var cantidad_tareas_obra = (from o in db.obras
                                      join c in db.categoriasPlanificador on o.idobra equals c.idobra
                                      join t in db.TareasPlanificador on c.idcategoria equals t.idcategoria
                                      where o.idobra == idobra
                                      select t).Count();

           if (cantidad_tareas_obra == 0) {
               return 0d;
           }

           var suma_porcentajes_tareas = (from o in db.obras
                                          join c in db.categoriasPlanificador on o.idobra equals c.idobra
                                          join t in db.TareasPlanificador on c.idcategoria equals t.idcategoria
                                          where o.idobra == idobra
                                          select t.porcentaje).Sum();


           var total_porcentaje_completo = cantidad_tareas_obra * 100;
           var porcentaje_actual = (suma_porcentajes_tareas * 100) / total_porcentaje_completo;


            return (double)porcentaje_actual;
       }

       public string deleteObra(int idobra)
       {
           var query = from a in db.obras
                       where a.idobra == idobra
                       select a;

           if (query.Count() > 0)
           {
               try
               {
                   db.obras.Remove(query.First());
                   db.SaveChanges();
                   return "";
               }
               catch (Exception e) {
                   return e.ToString();
               }
           }
           else
           {
               return "no se ha encontrado el registro a eliminar";
           }
       }

       

        public String updateobras(obras obj)
        {
            obras objtf = (from a in db.obras
                               where a.idobra == obj.idobra
                               select a).FirstOrDefault();

            objtf.nombre = obj.nombre;
            objtf.descripcion = obj.descripcion;
           

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
