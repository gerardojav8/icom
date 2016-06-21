using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebIcomApi.Entidades;

namespace WebIcomApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("common")]
    public class commonController : ApiController
    {
        [Authorize]
        [HttpPost]
        [Route("getFechaHoraActual")]
        public Object getFechaHoraActual()
        {
            try
            {
                clsfechahora objfechho = new clsfechahora();
                DateTime fechahoraact = DateTime.Now;
                String strhora = fechahoraact.TimeOfDay.ToString();
                String strfecha = fechahoraact.Year.ToString() + "-" + fechahoraact.Month.ToString() + "-" + fechahoraact.Day.ToString();

                objfechho.hora = strhora;
                objfechho.Fecha = strfecha;

                return objfechho;
            }
            catch (Exception e)
            {
                clsError objerr = new clsError();
                objerr.error = "No se ha podido obtener la fecha y hora actual";
                objerr.result = 0;
                return objerr;
            }
        }
    }
}
