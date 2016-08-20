using DAOicom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebIcomApi.Entidades
{
    public class clsListaMaquinas
    {
        public string noserie { get; set; }
        public int noeconomico { get; set; }
        public string marca { get; set; }
        public string modelo { get; set; }
        public int idtipomaquina { get; set; }

        public int tieneReporte { get; set; }

        public clsListaMaquinas(maquinas obj)
        {
            this.noserie = obj.noserie;
            this.noeconomico = (Int32)obj.noeconomico;
            this.marca = obj.marca;
            this.modelo = obj.modelo;
            this.idtipomaquina = (Int32)obj.idtipomaquina;

        }
    }
}