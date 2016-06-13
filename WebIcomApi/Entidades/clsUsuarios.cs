using DAOicom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebIcomApi.Entidades
{
    public class clsUsuarios
    {
        public int idusuario { get; set; }
        public string nombre { get; set; }
        public string apepaterno { get; set; }
        public string apematerno { get; set; }
        public string telefono { get; set; }
        public string mail { get; set; }
        public int edad { get; set; }
        public int idpuesto { get; set; }
        public string passapp { get; set; }
        public string usuario { get; set; }

        public clsUsuarios(usuarios obj)
        {
            this.idusuario = obj.idusuario;
            this.nombre = obj.nombre;
            this.apepaterno = obj.apepaterno;
            this.apematerno = obj.apematerno;
            this.telefono = obj.telefono;
            this.mail = obj.mail;
            this.edad = (Int32) obj.edad;
            this.idpuesto = (Int32) obj.idpuesto;
            this.passapp = obj.passapp;
            this.usuario = obj.usuario;
        }
    }
}