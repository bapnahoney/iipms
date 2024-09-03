using Abp.AspNetCore;
using Abp.AspNetCore.Mvc.Antiforgery;
using Abp.AspNetCore.SignalR.Hubs;
using Abp.Castle.Logging.Log4Net;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Json;
using Castle.Facilities.Logging;
using HIPMS.Configuration;
using HIPMS.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using static System.Net.WebRequestMethods;

namespace HIPMS.Web.Host.Startup
{
    public class Startup
    {
        private const string _defaultCorsPolicyName = "localhost";

        private const string _apiVersion = "v1";

        private readonly IConfigurationRoot _appConfiguration;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public Startup(IWebHostEnvironment env)
        {
            _hostingEnvironment = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //MVC
            services.AddControllersWithViews(
                options => { options.Filters.Add(new AbpAutoValidateAntiforgeryTokenAttribute()); }
            ).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new AbpMvcContractResolver(IocManager.Instance)
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };
            });

            IdentityRegistrar.Register(services);
            AuthConfigurer.Configure(services, _appConfiguration);

            //services.AddSignalR();

            // Configure CORS for angular2 UI
            services.AddCors(
                options => options.AddPolicy(
                    _defaultCorsPolicyName,
                    builder => builder
                        .WithOrigins(
                            // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
                            _appConfiguration["App:CorsOrigins"]
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                )
            );

            // Swagger - Enable this line and the related lines in Configure method to enable swagger UI
            ConfigureSwagger(services);

            // Configure Abp and Dependency Injection
            services.AddAbpWithoutCreatingServiceProvider<HIPMSWebHostModule>(
                // Configure Log4Net logging
                options => options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseAbpLog4Net().WithConfig(_hostingEnvironment.IsDevelopment()
                        ? "log4net.config"
                        : "log4net.Production.config"
                    )
                )
            );

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
                //options.excludedhosts.add("example.com");
                //options.excludedhosts.add("www.example.com");
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            //ConfigureDbContext(app.ApplicationServices);
            app.UseAbp(options => { options.UseAbpRequestLocalization = false; }); // Initializes ABP framework.

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseExceptionHandler("/Error");
            }//honey

            app.UseCors(_defaultCorsPolicyName); // Enable CORS!

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAbpRequestLocalization();
            //#region Working solution for localhost
            //app.Use(async (context, next) =>
            //{
            //    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self';script-src 'self' https://localhost:44312/libs/jquery/jquery.js https://localhost:44312/libs/abp-web-resources/Abp/Framework/scripts/abp.js https://localhost:44312/libs/spin/spin.js https://localhost:44312/libs/sweetalert/sweetalert.min.js  'unsafe-inline' ;base-uri 'self';form-action 'self';connect-src 'self' wss://localhost:44345/HIPMS.Web.Mvc/ 'unsafe-inline' ;style-src 'self' https://fonts.googleapis.com/css 'unsafe-inline';font-src 'self' https://fonts.gstatic.com/s/sourcesanspro/v22/6xK1dSBYKcSV-LCoeQqfX1RYOo3qPZ7qsDJT9g.woff2 'unsafe-inline' ; img-src * 'self' data:");
            //    await next();
            //});
            //#endregion
            //#region Working solution for ipmsvimqa.hzlmetals.com
            //app.Use(async (context, next) =>
            //{
            //    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self';script-src 'self' https://ipmsvimqa.hzlmetals.com/Account/Login https://ipmsvimqa.hzlmetals.com/libs/jquery/jquery.js https://ipmsvimqa.hzlmetals.com/libs/abp-web-resources/Abp/Framework/scripts/abp.js https://ipmsvimqa.hzlmetals.com/libs/spin/spin.js https://ipmsvimqa.hzlmetals.com/libs/sweetalert/sweetalert.min.js  'unsafe-inline' ;base-uri 'self';form-action 'self';connect-src 'self' wss://ipmsvimqa.hzlmetals.com/HIPMS.Web.Mvc/ 'unsafe-inline' ;style-src 'self' https://fonts.googleapis.com/css 'unsafe-inline';font-src 'self' https://fonts.gstatic.com/s/sourcesanspro/v22/6xK1dSBYKcSV-LCoeQqfX1RYOo3qPZ7qsDJT9g.woff2 'unsafe-inline' ; img-src * 'self' data:");
            //    await next();
            //});
            //#endregion
            app.UseEndpoints(endpoints =>
            {
                //honey comment this signalR
                //endpoints.MapHub<AbpCommonHub>("/signalr");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("defaultWithArea", "{area}/{controller=Home}/{action=Index}/{id?}");
            });
            //Header remove from response
            app.Use(async (context, next) => { context.Response.Headers.Remove("X-Powered-By"); await next.Invoke(); });
            app.Use(async (context, next) => { context.Response.Headers.Remove("Server"); await next.Invoke(); });


            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger(c => { c.RouteTemplate = "swagger/{documentName}/swagger.json"; });

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(options =>
            {
                // specifying the Swagger JSON endpoint.
                options.SwaggerEndpoint($"/swagger/{_apiVersion}/swagger.json", $"HIPMS API {_apiVersion}");
                options.IndexStream = () => Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("HIPMS.Web.Host.wwwroot.swagger.ui.index.html");
                options.DisplayRequestDuration(); // Controls the display of the request duration (in milliseconds) for "Try it out" requests.  
            }); // URL: /swagger
        }

        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(_apiVersion, new OpenApiInfo
                {
                    Version = _apiVersion,
                    Title = "HIPMS API",
                    Description = "HIPMS",
                    // uncomment if needed TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "HIPMS",
                        Email = string.Empty,
                        Url = new Uri("https://twitter.com/aspboilerplate"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://github.com/aspnetboilerplate/aspnetboilerplate/blob/dev/LICENSE"),
                    }
                });
                options.DocInclusionPredicate((docName, description) => true);

                // Define the BearerAuth scheme that's in use
                options.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme()
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                //add summaries to swagger
                bool canShowSummaries = _appConfiguration.GetValue<bool>("Swagger:ShowSummaries");
                if (canShowSummaries)
                {
                    var hostXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var hostXmlPath = Path.Combine(AppContext.BaseDirectory, hostXmlFile);
                    options.IncludeXmlComments(hostXmlPath);

                    var applicationXml = $"HIPMS.Application.xml";
                    var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, applicationXml);
                    options.IncludeXmlComments(applicationXmlPath);

                    var webCoreXmlFile = $"HIPMS.Web.Core.xml";
                    var webCoreXmlPath = Path.Combine(AppContext.BaseDirectory, webCoreXmlFile);
                    options.IncludeXmlComments(webCoreXmlPath);
                }
            });
        }
        //public void ConfigureDbContext(IServiceProvider serviceProvider)
        //{
        //    var builder = new DbContextOptionsBuilder<HIPMSDbContext>();
        //    builder.UseSqlServer(_appConfiguration.GetConnectionString("Default"),
        //    sqlServerOptions =>
        //    {
        //        sqlServerOptions.MigrationsAssembly(typeof(HIPMSDbContext).Assembly.FullName);
        //        sqlServerOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        //    });
        //}
    }
}
