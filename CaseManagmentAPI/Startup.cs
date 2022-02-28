
using Application.Companies;
using Application.Roles;
using Application.Users;
using AutoMapper;
using CaseManagmentAPI.Application.Mapping;
using CaseManagmentAPI.Services;
using Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;

namespace AuthServer
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
            var connectionString = Configuration["ConnectionStrings:DefaultConnection"];



            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

                var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentity<AppUser, Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //services.AddIdentityServer();
            //var builder = services.AddIdentityServer(options =>
            //{
            //    options.Events.RaiseErrorEvents = true;
            //    options.Events.RaiseInformationEvents = true;
            //    options.Events.RaiseFailureEvents = true;
            //    options.Events.RaiseSuccessEvents = true;
            //    options.EmitStaticAudienceClaim = true;
            //})
            //  .AddDeveloperSigningCredential()
            //       .AddConfigurationStore(options =>
            //       {
            //           options.ConfigureDbContext = builder =>
            //               builder.UseSqlServer(connectionString,
            //                   sql => sql.MigrationsAssembly(migrationsAssembly));
            //       })
            //    // this adds the operational data from DB (codes, tokens, consents)
            //    .AddOperationalStore(options =>
            //    {
            //        options.ConfigureDbContext = builder =>
            //            builder.UseSqlServer(connectionString,
            //                sql => sql.MigrationsAssembly(migrationsAssembly));

            //        // this enables automatic token cleanup. this is optional.
            //        options.EnableTokenCleanup = true;
            //        options.TokenCleanupInterval = 3600;
            //    })
            //    .AddAspNetIdentity<AppUser>();

           // services.AddTransient<IProfileService, IdentityClaimsProfileService>();

            services.AddCors(options => options.AddPolicy("AllowAll", p =>
              p.WithOrigins("http://localhost:4200")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials()
               ));

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            })
                .SetCompatibilityVersion(CompatibilityVersion.Latest);



            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });


            services.AddScoped<TokenService>();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenKey"]));

            services.AddAuthentication(
            options =>
            {

                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
            ).AddJwtBearer(

            opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    RequireExpirationTime = false
                };
            });

            services.AddHttpsRedirection(options => options.HttpsPort = 443);


            services.AddMediatR(typeof(CreateNewPassword.Handler).Assembly);

            services.AddMediatR(typeof(CreateResetPassword.ResetPasswordHandler).Assembly);

            services.AddMediatR(typeof(CreateRole.CreateRoleCommandHandler).Assembly);

            services.AddMediatR(typeof(GetCompany.GetCompanyByIdQuery).Assembly);
            services.AddMediatR(typeof(CreateCompany.Handler).Assembly);

            //services.AddMediatR(typeof(Startup));
          //  services.AddMediatR(Assembly.GetExecutingAssembly());





            services.AddAutoMapper(typeof(ApplicationProfile).Assembly);
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ApplicationProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if (error != null)
                    {
                        //context.Response.AddApplicationError(error.Error.Message);
                        await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(false);
                    }
                });
            });

            //var serilog = new LoggerConfiguration()
            //    .MinimumLevel.Verbose()
            //    .Enrich.FromLogContext()
            //    .WriteTo.File(@"authserver_log.txt");

            //loggerFactory.WithFilter(new FilterLoggerSettings
            //    {
            //        { "IdentityServer4", LogLevel.Debug },
            //        { "Microsoft", LogLevel.Warning },
            //        { "System", LogLevel.Warning },
            //    }).AddSerilog(serilog.CreateLogger());

            app.UseStaticFiles();
            app.UseCors("AllowAll");
            //app.UseIdentityServer();
            app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Strict });

            //app.usecookiepolicy(new cookiepolicyoptions
            //{
            //    httponly = httponlypolicy.none,
            //    minimumsamesitepolicy = samesitemode.none,
            //    secure = cookiesecurepolicy.always
            //});

            

            app.UseAuthentication();

            app.UseAuthorization();

            //app.usemvc(routes =>
            //{
            //    routes.maproute(
            //        name: "default",
            //        template: "{controller=home}/{action=index}/{id?}");
            //});
            app.UseMvcWithDefaultRoute();

        }
    }
}
