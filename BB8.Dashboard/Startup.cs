using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BB8.Dashboard
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_ => new System.Net.Http.HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            });

            services.AddSingleton(sp => GrpcChannel.ForAddress("https://raspberrypi:5001", new GrpcChannelOptions { HttpHandler = sp.GetRequiredService<HttpClientHandler>() }));
            services.AddSingleton(sp => new BB8.Services.RoboDiagnostics.RoboDiagnosticsClient(sp.GetRequiredService<GrpcChannel>()));

            services.AddSignalR();
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "bb8-dashboard-ui/build";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<SignalR.RobotHub>("/robot");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "bb8-dashboard-ui";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
