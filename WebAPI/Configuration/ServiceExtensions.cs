using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Channels;
using WebAPI.Configuration;
using WebAPI.Mediator.Tasks.Commands;
using WebAPI.Services.TaskServices;

namespace WebAPI.Configuration
{
    internal static class ServiceExtensions
    {
        public static IServiceCollection AddWebApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            //builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

            return services.RegisterServices(configuration)
                    .AddCORS()
                    .AddControllers()
                    .AddSwagger(configuration)
                    .AddHttpClients();
        }

        public static void AddHostedBackgroundTaskService(this IServiceCollection services)
        {
            // Register TaskProgressService
            services.AddSingleton<ITaskProgressService, TaskProgressService>();
            services.AddSingleton<IBackgroundMediator, BackgroundMediator>();
            
            // Register the Channel for commands
            services.AddSingleton(Channel.CreateUnbounded<IBackgroundCommand>());

            // Register the Hosted Service (for processing background tasks)
            services.AddHostedService<BackgroundTaskService>();

        }

        static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {

            return services;
        }

        static IServiceCollection AddControllers(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            }).AddControllers(options =>
            {
               // options.Filters.Add<ApiExceptionFilterAttribute>();
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            return services;
        }

        static IServiceCollection AddHttpClients(this IServiceCollection services)
        {
            services.AddHttpClient();
            return services;
        }

        static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services
            .AddApiVersioning(opt =>
            {
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.ReportApiVersions = true;
                opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader());
            })
            .AddVersionedApiExplorer(o =>
            {
                o.GroupNameFormat = "'v'VVV";
                o.SubstituteApiVersionInUrl = true;
            })
            .AddSwaggerGen(options =>
            {
                options.EnableAnnotations();
                options.SwaggerDoc("v1", new OpenApiInfo() { Title = "API v1", Version = "v1" });
                options.UseInlineDefinitionsForEnums();
                options.DescribeAllParametersInCamelCase();

                var assembly = Assembly.GetExecutingAssembly();
                var assemblyPath = Path.GetDirectoryName(assembly.Location);
                try
                {
                    foreach (var filePath in Directory.GetFiles(assemblyPath, $"{assembly.GetName().Name}.xml"))
                        options.IncludeXmlComments(filePath, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            })
            .AddSwaggerGenNewtonsoftSupport();

            return services;
        }

        static IServiceCollection AddCORS(this IServiceCollection services)
        {
            return services.AddCors(opt =>
            {
                opt.AddPolicy(name: "AppConstants_CORS_PolicyName", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }

    }
}
