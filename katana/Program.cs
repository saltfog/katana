using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kantana
{
    using Microsoft.Owin.Hosting;
    using Owin;
    using System.IO;
    using AppFunc = Func<IDictionary<string, object>, Task>; //own using statement

    class Program
    {
        static void Main(string[] args)
        {
            string uri = "http://localhost:8080";

            using (WebApp.Start<Startup>(uri))
            {
                Console.WriteLine("Started!");
                Console.WriteLine("Enter http://localhost:8080/ in your browser to configure OWN");
                Console.ReadKey();
                Console.WriteLine("Stopping!");
            }
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use(async (environment, next) =>
            {
                Console.WriteLine("Requesting : " + environment.Request.Path);

                Console.WriteLine("Response: " + environment.Response.StatusCode);

                foreach (var pair in environment.Environment)
                {
                    Console.WriteLine("{0}:{1}", pair.Key, pair.Value);
                }

                await next();
            });

            //ConfigureWebApi(app);

            app.UseWelcomePage(); //owin welcome page
            app.UseTestComponent();

        }
    }

    public static class AppBuildExtentions
    {
        public static void UseTestComponent(this IAppBuilder app)
        {
            app.Use<TestComponent>();
        }
    }

    //Low Level Component
    public class TestComponent
    {
        //ctor
        AppFunc _next;
        public TestComponent(AppFunc next)
        {
            _next = next;

        }
        public async Task Invoke(IDictionary<string, object> environment)
        {
            var response = environment["owin.ResponseBody"] as Stream;
            using (var writer = new StreamWriter(response))
            {
                await writer.WriteAsync("AppFunc Test");
            }
        }
    }
}

