using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;

[assembly: OwinStartup(typeof(Square9.SecurityDisplay.Startup))]
namespace Square9.SecurityDisplay
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            const string rootFolder = @"../";
            var fileSystem = new PhysicalFileSystem(rootFolder);
            var options = new FileServerOptions
            {
                EnableDefaultFiles = true,
                FileSystem = fileSystem
            };

            config.Routes.MapHttpRoute("DefaultAPI",
                "api/{controller}/{action}/{id}",
                new { id = RouteParameter.Optional });
            app.UseFileServer(options);
            app.UseWebApi(config);
            
        }
    }
}
