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
        public string fechaini { get; set; }
        public string fechafin { get; set; }
        public string horaini { get; set; }
        public string horafin { get; set; }
        public List<Dictionary<String, String>> usuarios { get; set; }
    }
}