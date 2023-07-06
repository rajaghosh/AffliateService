using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telemedicine.Data;

namespace AffliateService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        readonly string CORSPolicyProd = "_CORSPolicyProd";
        readonly string CORSPolicyDev = "_CORSPolicyDev";
        readonly string CORSPolicyTesting = "_CORSPolicyProd";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen();


            //This will allow production based URLs to communicate
            services.AddCors(options =>
            {
                options.AddPolicy(name: CORSPolicyProd,
                                  builder =>
                                  {
                                      builder.WithOrigins("https://localhost:5001",
                                                          "https://localhost:5000",
                                                          "https://telemedicine.frontlinemds.com",
                                                          "http://ec2-54-89-28-147.compute-1.amazonaws.com")
                                                          .AllowAnyHeader()
                                                          .AllowAnyMethod();
                                  }
                                );
            });

            //This will allow dev based URLs to communicate
            services.AddCors(options =>
            {
                options.AddPolicy(name: CORSPolicyDev,
                                  builder =>
                                  {
                                      builder.WithOrigins("http://localhost:4200",
                                                          "http://localhost:5200")
                                                          .AllowAnyHeader()
                                                          .AllowAnyMethod();
                                  }
                                );
            });

            //This will allow any URL to communicate (This is for testing)
            services.AddCors(options =>
            {
                options.AddPolicy(name: CORSPolicyTesting,
                                  builder =>
                                  {
                                      builder.AllowAnyOrigin()
                                             .AllowAnyHeader()
                                             .AllowAnyMethod();
                                  }
                                );
            });


            services.AddScoped<IDapper, AppDapper>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Test1 Api v1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            if (env.IsDevelopment())
            {
                app.UseCors(CORSPolicyDev);
            }
            else
            {
                app.UseCors(CORSPolicyProd);
            }

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }
    }
}
