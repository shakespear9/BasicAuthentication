using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Authentication.AuthorizationRequirements;
using Basic.Controllers;
using Basic.CustomerPolicyProvider;
using Basic.Transformer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BasicAuthentication
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("CookieAuth") // Cookie Handler
                .AddCookie("CookieAuth", config =>
                 {
                     config.Cookie.Name = "Grandmas.Cookie";
                     config.LoginPath = "/Home/Authenticate";
                    //config.LoginPath
                });

            //Configuration.GetConnectionString()


            // Authorization Provider
            services.AddAuthorization(config =>
            {

                /// Original
                //var defaultAuthBuider = new AuthorizationPolicyBuilder();
                //    var defaultAuthPolicy = defaultAuthBuider
                //   .RequireAuthenticatedUser()
                //    .RequireClaim(ClaimTypes.DateOfBirth)
                //    .Build();    = 

                //    config.DefaultPolicy = defaultAuthPolicy;


                /// Mimic #1
                //config.AddPolicy("Claim.DoB", policyBuilder =>
                //{
                //    policyBuilder.RequireClaim(ClaimTypes.DateOfBirth);
                //});


                /// Mimic #2
                // Register Requirement
                //config.AddPolicy("Claim.DoB", policyBuilder =>
                //{
                //    policyBuilder.AddRequirements(new CustomerRequireClaim(ClaimTypes.DateOfBirth));
                //});

                config.AddPolicy("Admin", policyBuilder => policyBuilder.RequireClaim(ClaimTypes.Role, "Admin"));

                //config.AddPolicy("Admin", policyBuilder => policyBuilder.RequireRole("Admin"));




                //config.AddPolicy("Admin Access", policy => policy.RequireRole)



                config.AddPolicy("Claim.DoB", policyBuilder =>
              {
                  policyBuilder.RequireCustomClaim(ClaimTypes.DateOfBirth);
              });



            });



            // custom PolicyProvider
            services.AddSingleton<IAuthorizationPolicyProvider,CustomAuthorizationPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, SecurityLevelHandler>();

            // Add Handler to Service
            services.AddScoped<IAuthorizationHandler,CustomerRequireClaimHandler>();

            // Register OperationAuthorizationHandler
            services.AddScoped<IAuthorizationHandler, CookieJarAuthorizationHandler>();

            // Regis IClaim proc every time when User Authenticate
            services.AddScoped<IClaimsTransformation,ClaimsTransformation>();

            //services.addSing





            services.AddControllersWithViews(config => {
                var defaultAuthBuider = new AuthorizationPolicyBuilder();
                var defaultAuthPolicy = defaultAuthBuider
               .RequireAuthenticatedUser()
                .Build();

                // global Authorization Atrribute/Filter

                //config.Filters.Add(new AuthorizeFilter(defaultAuthPolicy));

            });

            services.AddRazorPages()
                .AddRazorPagesOptions(config => 
                {
                    config.Conventions.AuthorizePage("/Razor/Secured");
                
                
                });

            }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        // Middleware
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            
             
            app.UseRouting();


            // Who you are ?
            app.UseAuthentication();


            // Are you allowed ?
            app.UseAuthorization();

            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}
