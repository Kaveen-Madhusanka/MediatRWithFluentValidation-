using MediatR;
using MediatR.Extensions.FluentValidation.AspNetCore;
using MediatRWithValidation.Persistant;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace MediatRWithValidation
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MediatRWithValidation", Version = "v1" });
            });

            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DevConnection")));
            services.AddMediatR(typeof(Startup).Assembly);
            services.AddFluentValidation(new[] { typeof(Startup).Assembly });
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MediatRWithValidation v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            // custom middlware
            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Use(async (context, next) =>
                {
                    var error = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;
                    if (error?.Error != null)
                    {
                        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                        Console.WriteLine($"--> {contextFeature.Error.GetType()}");

                        if (contextFeature.Error.GetType() == typeof(ArgumentException))
                        {
                            Console.WriteLine("--> this is an Arugumrnt Exp");
                        }
                        else if (contextFeature.Error.GetType() == typeof(FluentValidation.ValidationException)) 
                        {
                            Console.WriteLine("--> this is a fluent validation");

                            var fError = contextFeature.Error as FluentValidation.ValidationException;
                            var errorResponse = new
                            {
                                Message = fError.Message,
                                Errors = fError.Errors.Select(e => $"{e.PropertyName} - {e.ErrorMessage}"),
                                TraceCode = Guid.NewGuid().ToString(),
                                DateTime = DateTime.Now
                            };

                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
                            return;

                        } 
                        
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "application/json";

                        // var response = error.Error.CreateODataError(!env.IsDevelopment());
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(error.Error));
                    }

                    // when no error, do next.
                    else await next();
                });
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
