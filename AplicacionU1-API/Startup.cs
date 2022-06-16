using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using AplicacionU1_API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AplicacionU1_API
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        private readonly string _MyCors = "MyCors";
        public void ConfigureServices(IServiceCollection services)
        {          
            services.AddDbContext<itesrcne_181g0138Context>(options =>
            options.UseMySql("server=204.93.216.11;user=itesrcne_karla;password=181G0138;database=itesrcne_181g0138",
            Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.3.29-mariadb")));

            services.AddCors(options =>
            {
                options.AddPolicy(name: _MyCors, builder =>
                 {
                     builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                     .AllowAnyHeader().AllowAnyMethod();
                 });
            });

            services.AddMvc();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors(_MyCors);
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});
                endpoints.MapDefaultControllerRoute();
            });

           
        }
    }
}
