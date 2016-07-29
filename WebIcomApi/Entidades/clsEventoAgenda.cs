using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebIcomApi.Entidades
{
    public class clsEventoAgenda
    {
        public int mes { get; set; }
        public int dia { get; set; }
        public string titulo { get; set; }
        public string comentario { get; set; }
        public string lapso { get; set; }
        public List<Dictionary<String, String>> usuarios { get; set; }
    }
}