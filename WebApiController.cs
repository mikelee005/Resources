using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;

namespace adr.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            /* adrMVC: Attribute routing is in View Controllers are enabled by the MapHttpAttributeRoutes call
              * this line finds the Controllers in the application, then it reads the Route Attributes and 
              * puts the appropriate data in the route table.
              * 
              * this route table is what is searched by the frame work to determine what to end point to use
             
              */
            config.MapHttpAttributeRoutes();

            //adrMVC: this is the direct way to put something in the route table
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            //adr: we remove the xml serilizer to make life easier. 
            MediaTypeHeaderValue appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;

            //adrWebApi: this is what transforms all the Json into camelcase for us
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            //- 1/11/2017 edit
            //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            JsonMediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();
            //optional: set serializer settings here
            //config.Services.Replace(typeof(IContentNegotiator), new JsonContentNegotiator(jsonFormatter));

            MediaTypeFormatterCollection formatters = config.Formatters;
            JsonMediaTypeFormatter jFormatter = formatters.JsonFormatter;
            JsonSerializerSettings settings = jsonFormatter.SerializerSettings;
            // settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
