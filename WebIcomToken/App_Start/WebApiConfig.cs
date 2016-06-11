using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WebIcomToken
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var formatters = GlobalConfiguration.Configuration.Formatters;
            var jsonFormatter = formatters.JsonFormatter;
            var settings = jsonFormatter.SerializerSettings;

            //Opcion para que no importe que tipo de valor se esta serializando
            jsonFormatter.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;

            //Se remueve de la configuracion general el formato de xml para el servicio
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            //Esto hace que el json salga acomdodado con tabs(identado)
            settings.Formatting = Formatting.Indented;

            //Todas las propiedades que tengan valor null no se mandan en el json
            settings.NullValueHandling = NullValueHandling.Ignore;

            //Forza que las clases que estan hechas .net sean notacion camello (primer letra en mayuscula)
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.EnableCors();
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        private static void EnableCrossSiteRequests(HttpConfiguration config)
        {
            var cors = new EnableCorsAttribute(
                    origins: "*",
                    headers: "*",
                    methods: "*");
            config.EnableCors();
        }   
    }
}
