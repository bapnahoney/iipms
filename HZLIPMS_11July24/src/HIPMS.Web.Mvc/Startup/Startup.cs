using Abp.AspNetCore;
using Abp.AspNetCore.Mvc.Antiforgery;
using Abp.AspNetCore.SignalR.Hubs;
using Abp.Castle.Logging.Log4Net;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Json;
using Castle.Facilities.Logging;
using DocumentFormat.OpenXml.InkML;
using HIPMS.Authentication.JwtBearer;
using HIPMS.Authorization.NCR;
using HIPMS.Authorization.PO;
using HIPMS.Configuration;
using HIPMS.Dapper;
using HIPMS.Dashboard;
using HIPMS.DC;
using HIPMS.EntityFrameworkCore;
using HIPMS.File;
using HIPMS.IC;
using HIPMS.Identity;
using HIPMS.Options;
using HIPMS.Services;
using HIPMS.Services.SendEmail;
using HIPMS.SRFI;
using HIPMS.Web.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.WebEncoders;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using ITfoxtec.Identity.Saml2.MvcCore.Configuration;
//using System.Linq;

namespace HIPMS.Web.Startup
{
    public class Startup
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfigurationRoot _appConfiguration;
        //private const string _defaultCorsPolicyName = "localhost";
        public Startup(IWebHostEnvironment env)
        {
            _hostingEnvironment = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        [Obsolete]
        public void ConfigureServices(IServiceCollection services)
        {
            //added by honey
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.MaxAge = TimeSpan.FromMinutes(30);
            });

            //services.AddIdentity<ApplicationUser, IdentityRole>(options => {

            //    options.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromHours(1);

            //});


            // MVC
            services.AddControllersWithViews(
                    options =>
                    {
                        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                        options.Filters.Add(new AbpAutoValidateAntiforgeryTokenAttribute());
                    }
                )
                .AddRazorRuntimeCompilation()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new AbpMvcContractResolver(IocManager.Instance)
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    };
                });

            IdentityRegistrar.Register(services);
            AuthConfigurer.Configure(services, _appConfiguration);

            services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });

            var Configuration = _appConfiguration;

            var sapSettingsSection = Configuration.GetSection(SAPSettingsOptions.SAPSettings);
            var sapBackgroundSettings = sapSettingsSection.Get<SAPSettingsOptions>();

            var emailSettingsSection = Configuration.GetSection(EmailNotificationSettingsOptions.EmailNotificationSettings);
            var emaiBackgroundSettings = sapSettingsSection.Get<EmailNotificationSettingsOptions>();

            services.AddScoped<IWebResourceManager, WebResourceManager>();
            services.AddScoped<ISendEmail, SendEmail>();
            services.AddScoped<IDapperRepository, DapperRepository>();
            services.AddScoped<IInspectionClearanceService, InspectionClearanceService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IPOService, POService>();
            services.AddScoped<IDispatchClearanceService, DispatchClearanceService>();
            services.AddScoped<IRFIAppService, RFIAppService>();
            services.AddScoped<INCRService, NCRService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IEmailTriggerService, EmailTriggerService>();
            services.Configure<SAPSettingsOptions>(Configuration.GetSection(SAPSettingsOptions.SAPSettings));
            services.Configure<EmailNotificationSettingsOptions>(Configuration.GetSection(EmailNotificationSettingsOptions.EmailNotificationSettings));
            //services.AddSignalR();


            // Configure CORS for angular2 UI
            //services.AddCors(
            //    options => options.AddPolicy(
            //        _defaultCorsPolicyName,
            //        builder => builder
            //            .WithOrigins(
            //                // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
            //                _appConfiguration["App:CorsOrigins"]
            //                    .Split(",", StringSplitOptions.RemoveEmptyEntries)
            //                    .Select(o => o.RemovePostFix("/"))
            //                    .ToArray()
            //            )
            //            .AllowAnyHeader()
            //            .AllowAnyMethod()
            //            .AllowCredentials()
            //    )
            //);

            //#region Saml
            services.Configure<Saml2Configuration>(Configuration.GetSection("Saml2"));

            services.Configure<Saml2Configuration>(saml2Configuration =>
            {
                saml2Configuration.AllowedAudienceUris.Add(saml2Configuration.Issuer);

                var entityDescriptor = new EntityDescriptor();
                entityDescriptor.ReadIdPSsoDescriptorFromUrl(new Uri(_appConfiguration["Saml2:IdentityProvider:MetadataLocation"]));
                //entityDescriptor.ReadIdPSsoDescriptorFromUrl(new Uri("https://localhost:44312/metadata/metadata.xml"));
                if (entityDescriptor.IdPSsoDescriptor != null)
                {
                    saml2Configuration.SingleSignOnDestination = entityDescriptor.IdPSsoDescriptor.SingleSignOnServices.First().Location;
                    saml2Configuration.SignatureValidationCertificates.AddRange(entityDescriptor.IdPSsoDescriptor.SigningCertificates);
                }
                else
                {
                    throw new Exception("IdPSsoDescriptor not loaded from metadata.");
                }
            });

            services.AddSaml2();
            //#endregion




            // Configure Abp and Dependency Injection
            services.AddAbpWithoutCreatingServiceProvider<HIPMSWebMvcModule>(
                // Configure Log4Net logging
                options => options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseAbpLog4Net().WithConfig(
                        _hostingEnvironment.IsDevelopment()
                            ? "log4net.config"
                            : "log4net.Production.config"
                        )
                )
            );

            //Honey
            services.AddDbContext<HIPMSDbContext>(options =>
                options.
                   UseSqlServer(_appConfiguration.GetConnectionString("Default"),
            sqlServerOptions =>
            {
                sqlServerOptions.MigrationsAssembly(typeof(HIPMSDbContext).Assembly.FullName);
                sqlServerOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            }));
            services.Configure<IISServerOptions>(options =>
            {
                //honey set true
                options.AutomaticAuthentication = true;
            });
            services.Configure<FormOptions>(options => options.ValueCountLimit = 5000);
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
                //options.excludedhosts.add("example.com");
                //options.excludedhosts.add("www.example.com");
            });
            // Force minimum TLS version
            //services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
            //{
            //   options.ListenAnyIP(
            //   5000
            // , listenOptions => { listenOptions.UseHttps(); listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2; });
            //});

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            //ConfigureDbContext(app.ApplicationServices);
            app.UseAbp(); // Initializes ABP framework.

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseExceptionHandler("/Error");
            }//honey
            //app.UseCors(_defaultCorsPolicyName); // Enable CORS!

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseJwtTokenMiddleware();

            app.UseAuthorization();
            //added by honey
            app.UseSession();

            //Header remove from response
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

            #region trial
            //app.Use(async (context, next) =>
            //{
            //    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self';script-src 'self' https://localhost:44312/* 'unsafe-eval';");
            //    await next();
            //});
            //app.Use(async (context, next) =>
            //{
            //    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self';script-src 'self' 'sha256-aqNNdDLnnrDOnTNdkJpYlAxKVJtLt9CtFLklmInuUAE=' 'sha256-47DEQpj8HBSa+/TImW+5JCeuQeRkm5NMpJWZG3hSuFU=' 'sha256-Y5HGV3cmFL1QmdV9FMkQjm7MR7FR+stNxbf9+GKET60=' 'sha256-Qh5S8hSqg7vWApzFzBJpsbPKMwbBbcSyYkkmln4Cti8=' 'sha256-sTKknHRwbNaicIUcm0H7RuTDDuBQjpcHBb70/6FPOkM=' 'sha256-gpmUJlXB1D9R8UTB7os2uDHEpGmaiVhn/mlBndZSH38=' 'sha256-FSf2TGfpktM3i9Z2n4B1fFB83K3m/5xAcoyvgavQ5Pg=' 'unsafe-inline' 'unsafe-eval';");
            //    await next();
            //});
            #endregion

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapRazorPages();

                //endpoints.MapHub<AbpCommonHub>("/signalr");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("defaultWithArea", "{area}/{controller=Home}/{action=Index}/{id?}");
            });
            //Header remove from response
            app.Use(async (context, next) => { context.Response.Headers.Remove("X-Powered-By"); await next.Invoke(); });
            app.Use(async (context, next) => { context.Response.Headers.Remove("Server"); await next.Invoke(); });

            //app.Use(async (context, next) => { context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; script-src 'self'; style-src 'self'; font-src 'self'; img-src 'self'; frame-src 'self'"); await next.Invoke(); });

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
