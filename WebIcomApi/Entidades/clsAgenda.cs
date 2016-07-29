using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebIcomApi.Entidades;

namespace WebIcomApi.Controllers
{
    public class clsAgenda
    {
        public int mes { get; set; }
        public string comentario { get; set; }
        public List<clsEvento> eventos { get; set; }

        public clsAgenda() { eventos = new List<clsEvento>(); }
    }
}