using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HPlusSport.API.Classes;
using HPlusSport.API.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HPlusSport.API
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
            var connection = Configuration["DbConnectionString"];
            services.AddDbContext<ShopContext>(options => options.UseSqlServer(connection));
            services.AddControllers();
            services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
            {
                options.Authority = "http://localhost:61884";
                options.RequireHttpsMetadata = false;
                options.Audience = "hps-api";
            });
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                    builder.WithOrigins("https://localhost:44311")
                            .AllowAnyHeader().AllowAnyMethod());
            });
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = new HeaderApiVersionReader("X-API-Version");
            });
            services.AddVersionedApiExplorer(
                options => options.GroupNameFormat = "'v'VVV");
            services.AddTransient<Microsoft.Extensions.Options.IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(options => {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());
                }
            });

        }
    }
}
