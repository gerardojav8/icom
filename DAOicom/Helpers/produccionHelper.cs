using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace DAOicom.Helpers
{
    public class produccionHelper
    {
        private icomEntities db = new icomEntities();
        public void insertproduccion(produccion objproduccion)
        {
            db.produccion.Add(objproduccion);
            db.SaveChanges();
        }

        public List<produccion> getTodasproduccion()
        {
            var query = from tf in db.produccion
                        select tf;

            List<produccion> lstproduccion = new List<produccion>();
            lstproduccion.AddRange(query.ToList());
            return lstproduccion;
        }

        public List<String> getNombreClientes()
        {
            var query = (from tf in db.produccion
                        select tf.cliente).Distinct();

           
            return query.ToList();
        }

        public string deleteProduccion(String folio)
        {
            var query = from t in db.produccion
                        where t.folio == folio
                        select t;

            if (query.Count() > 0)
            {
                try
                {
                    db.produccion.Remove(query.First());
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

        public List<produccion> getProduccionByFiltros(String folio, String material, String unidad, decimal cantidad, String cliente, DateTime? pFecha, DateTime? pFechafin)
        {
            String strwhere = "";
            List<Object> lstparam = new List<Object>();
            int noparam = 0;
            bool blnTieneWhere = false;

            if (!folio.Equals("")) {
                strwhere += "folio.Contains(@"+ noparam.ToString()+")";
                lstparam.Add(folio);
                noparam++;
                blnTieneWhere = true;
            }

            if (!material.Equals(""))
            {
                if (blnTieneWhere) {
                    strwhere += " AND material.Contains(@" + noparam.ToString() + ")";
                    lstparam.Add(material);
                    noparam++;
                } else {
                    strwhere += "material.Contains(@" + noparam.ToString() + ")";
                    lstparam.Add(material);
                    noparam++;
                    blnTieneWhere = true;
                }
            }

            if (!unidad.Equals(""))
            {
                if (blnTieneWhere)
                {
                    strwhere += " AND unidad.Contains(@" + noparam.ToString() + ")";
                    lstparam.Add(unidad);
                    noparam++;
                }
                else
                {
                    strwhere += "unidad.Contains(@" + noparam.ToString() + ")";
                    lstparam.Add(unidad);
                    noparam++;
                    blnTieneWhere = true;
                }
            }

            if (cantidad > 0)
            {
                if (blnTieneWhere)
                {
                    strwhere += " AND cantidad = @" + noparam.ToString() ;
                    lstparam.Add(cantidad);
                    noparam++;
                }
                else
                {
                    strwhere += "cantidad = @" + noparam.ToString();
                    noparam++;
                    lstparam.Add(cantidad);
                    blnTieneWhere = true;
                }
            }

            if (!cliente.Equals("")) {
                if (blnTieneWhere)
                {
                    strwhere += " AND cliente.Contains(@" + noparam.ToString() + ")";
                    noparam++;
                    lstparam.Add(cliente);
                }
                else
                {
                    strwhere += "cliente.Contains(@" + noparam.ToString() + ")";
                    noparam++;
                    lstparam.Add(cliente);
                    blnTieneWhere = true;
                }
            }

            if (pFecha != null) {
                
                if (blnTieneWhere)
                {
                    strwhere += " AND fecha >= @" + noparam.ToString();
                    lstparam.Add(pFecha);
                    noparam++;
                }
                else
                {
                    strwhere += "fecha >= @" + noparam.ToString();
                    lstparam.Add(pFecha);
                    noparam++;
                    blnTieneWhere = true;
                }
            }

            if (pFechafin != null)
            {                
                if (blnTieneWhere)
                {
                    strwhere += " AND fecha <= @" + noparam.ToString();
                    lstparam.Add(pFechafin);                    
                }
                else
                {
                    strwhere += "fecha <= @" + noparam.ToString();
                    lstparam.Add(pFechafin);                    
                }
            }


            var query = db.produccion.Where(strwhere, lstparam.ToArray());                        
                        

            List<produccion> lstproduccion = new List<produccion>();
            lstproduccion.AddRange(query.ToList());
            return lstproduccion;
        }

        public produccion getproduccionById(string id)
        {
            var query = from t in db.produccion
                        where t.folio == id
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

        public String updateproduccion(produccion obj)
        {
            produccion objtf = (from t in db.produccion
                             where t.folio == obj.folio
                             select t).FirstOrDefault();

            objtf.material = obj.material;
            objtf.cantidad = obj.cantidad;
            objtf.unidad = obj.unidad;
            objtf.cliente = obj.cliente;
            objtf.fecha = objtf.fecha;

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
