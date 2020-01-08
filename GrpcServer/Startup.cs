using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrpcServer.Repositories;
using GrpcServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Hosting;

namespace GrpcServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddSingleton<PhoneBookRepository>();
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
                endpoints.MapGrpcService<GreeterService>();                
                endpoints.MapGrpcService<PhoneBookService>();
                endpoints.MapGrpcService<BuddyGuyService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });

                endpoints.MapGet("/protos/phonebook.proto", async context =>
                {
                    await context.Response.WriteAsync(System.IO.File.ReadAllText("Protos/phonebook.proto"));
                });

                endpoints.MapGet("/protos/greet.proto", async context =>
                {
                    await context.Response.WriteAsync(System.IO.File.ReadAllText("Protos/greet.proto"));
                });

                endpoints.MapGet("/protos/buddyguy.proto", async context =>
                {
                    await context.Response.WriteAsync(System.IO.File.ReadAllText("Protos/buddyguy.proto"));
                });
            });
        }
    }
}
