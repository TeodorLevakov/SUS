using SUS.HTTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SUS.MvcFramework
{
    public static class Host
    {
        public static async Task CreateHostAsync(IMvcApplication application, int port = 80) 
        {
            List<Route> routeTable = new List<Route>();

            AutoRegisterStaticFile(routeTable);
            AutoRegisteRoutes(routeTable, application);

            application.ConfigureServices();
            application.Configure(routeTable);

            foreach (var rout in routeTable)
            {
                Console.WriteLine($"{rout.Method} - {rout.Path}");
            }

            IHttpServer server = new HttpServer(routeTable);

            await server.StartAsync(port);
        }

        private static void AutoRegisteRoutes(List<Route> routeTable, IMvcApplication application)
        {
            var controllerTypes = application.GetType().Assembly.GetTypes()
                .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(typeof(Controller)));

            foreach (var controllerT in controllerTypes)
            {
                //Console.WriteLine(controllerT.Name);

                var methods = controllerT.GetMethods()
                    .Where(x => x.IsPublic && !x.IsStatic && x.DeclaringType == controllerT &&
                    !x.IsAbstract && !x.IsConstructor && !x.IsSpecialName);

                foreach (var m in methods)
                {
                    var url = "/" + controllerT.Name.Replace("Controller", string.Empty)
                        + "/" + m.Name;

                    var attribute = m.GetCustomAttributes(false)
                        .Where(x => x.GetType().IsSubclassOf(typeof(BaseHttpAttribute)))
                        .FirstOrDefault() as BaseHttpAttribute;

                    var httpMethod = HttpMethod.Get;

                    if (attribute != null)
                    {
                        httpMethod = attribute.Method;
                    }

                    if (!string.IsNullOrEmpty(attribute?.Url))
                    {
                        url = attribute.Url;
                    }

                    routeTable.Add(new Route(url, httpMethod, (request) => 
                    {
                        var instance = Activator.CreateInstance(controllerT) as Controller;
                        instance.Request = request;

                        var responce = m.Invoke(instance, new object [] { }) as HttpResponse;

                        return responce;
                    }));
                  
                }
            
            }
        }

        private static void AutoRegisterStaticFile(List<Route> routeTable) 
        {
            var staticFiles = Directory.GetFiles("wwwroot", "*", SearchOption.AllDirectories);

            foreach (var item in staticFiles)
            {
                var url = item.Replace("wwwroot", string.Empty)
                    .Replace("\\", "/");

                routeTable.Add(new Route(url, HttpMethod.Get, (request) =>
                {
                    var fileContent = File.ReadAllBytes(item);
                    var fileExt = new FileInfo(item).Extension;
                    var contentType = fileExt switch
                    {
                        ".txt" => "text/plain",
                        ".js" => "text/javascript",
                        ".css" => "text/css",
                        ".jpg" => "image/jpg",
                        ".jpeg" => "image/jpg",
                        ".png" => "image/gif",
                        ".ico" => "image/vnd.microsoft.icon",
                        ".html" => "text/html",
                        _ => "text/plain",
                    };

                    return new HttpResponse(contentType, fileContent, HttpStatusCode.OK);

                }));
            }
        }
    }
}
