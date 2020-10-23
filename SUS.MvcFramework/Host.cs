﻿using SUS.HTTP;
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
            IServiceCollection servceCollection = new ServiceCollection();

            AutoRegisterStaticFile(routeTable);

            application.ConfigureServices(servceCollection);
            application.Configure(routeTable);
            
            AutoRegisteRoutes(routeTable, application, servceCollection);

            foreach (var rout in routeTable)
            {
                Console.WriteLine($"{rout.Method} - {rout.Path}");
            }
            Console.WriteLine();
            Console.WriteLine("Requests");

            IHttpServer server = new HttpServer(routeTable);

            await server.StartAsync(port);
        }

        private static void AutoRegisteRoutes(List<Route> routeTable, IMvcApplication application, IServiceCollection serviceCollection)
        {
            var controllerTypes = application.GetType().Assembly.GetTypes()
                .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(typeof(Controller)));

            foreach (var controllerType in controllerTypes)
            {
                //Console.WriteLine(controllerT.Name);

                var methods = controllerType.GetMethods()
                    .Where(x => x.IsPublic && !x.IsStatic && x.DeclaringType == controllerType &&
                    !x.IsAbstract && !x.IsConstructor && !x.IsSpecialName);

                foreach (var method in methods)
                {
                    var url = "/" + controllerType.Name.Replace("Controller", string.Empty)
                        + "/" + method.Name;

                    var attribute = method.GetCustomAttributes(false)
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

                    routeTable.Add(new Route(url, httpMethod, (request) => ExecuteAction(request, controllerType, method, serviceCollection)));
                  
                }
            
            }
        }

        private static HttpResponse ExecuteAction(HttpRequest request, Type controllerType, MethodInfo action, IServiceCollection serviceCollection) 
        {
            var instance = serviceCollection.CreateInstance(controllerType) as Controller;
            instance.Request = request;

            var arguments = new List<object>();
            var parameters = action.GetParameters();

            foreach (var parameter in parameters)
            {
                var httpParamValue = GetParameterFropmRequest(request, parameter.Name);

                var parameterValue = Convert.ChangeType(httpParamValue, parameter.ParameterType);

                if (parameterValue == null && parameter.ParameterType != typeof(string))
                {
                    parameterValue = Activator.CreateInstance(parameter.ParameterType);
                    var properties = parameter.ParameterType.GetProperties();

                    foreach (var property in properties)
                    {
                        var propertyHttpParamValue = GetParameterFropmRequest(request, property.Name);

                        var propertyParameterValue = Convert.ChangeType(propertyHttpParamValue, property.PropertyType);

                        property.SetValue(parameterValue, propertyParameterValue);
                    }
                }

                arguments.Add(parameterValue);

            }

            var responce = action.Invoke(instance, arguments.ToArray()) as HttpResponse;

            return responce;
        }

        private static string GetParameterFropmRequest(HttpRequest request, string parameterName) 
        {
            parameterName = parameterName.ToLower();

            if (request.FormData.Any(x => x.Key.ToLower() == parameterName))
            {
                return request.FormData
                    .FirstOrDefault(x => x.Key.ToLower() == parameterName).Value;
            }

            if (request.QueryData.Any(x => x.Key.ToLower() == parameterName))
            {
                return request.QueryData
                    .FirstOrDefault(x => x.Key.ToLower() == parameterName).Value;
            }
            return null;
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
