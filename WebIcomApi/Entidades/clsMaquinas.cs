using DAOicom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebIcomApi.Entidades
{
    public class clsMaquinas
    {
        public string noserie { get; set; }
        public int noeconomico { get; set; }
        public string marca { get; set; }
        public string modelo { get; set; }
        public int aniofabricacion { get; set; }
        public int idequipo { get; set; }
        public String imagen { get; set; }
        public int idtipomaquina { get; set; }

        public clsMaquinas(maquinas obj)
        {
            this.noserie = obj.noserie;
            this.noeconomico = (Int32)obj.noeconomico;
            this.marca = obj.marca;
            this.modelo = obj.modelo;
            this.aniofabricacion = (Int32)obj.aniofabricacion;
            this.idequipo = -1;
            if (obj.idequipo != null)
            {
                this.idequipo = (Int32)obj.idequipo;
            }

            this.imagen = "";
            this.idtipomaquina = (Int32)obj.idtipomaquina;
        }
    }
}