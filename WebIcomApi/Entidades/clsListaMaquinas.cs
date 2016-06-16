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
        public int modelo { get; set; }
        public int idtipomaquina { get; set; }

        public clsListaMaquinas(maquinas obj)
        {
            this.noserie = obj.noserie;
            this.noeconomico = (Int32)obj.noeconomico;
            this.marca = obj.marca;
            this.modelo = (Int32)obj.modelo;
            this.idtipomaquina = (Int32)obj.idtipomaquina;
        }
    }
}