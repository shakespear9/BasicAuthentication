using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Authentication.AuthorizationRequirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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


            services.AddAuthorization(config =>
            {

                //Configuration.GetConnectionString()
                //    var defaultAuthBuider = new AuthorizationPolicyBuilder();
                //    var defaultAuthPolicy = defaultAuthBuider
                //   .RequireAuthenticatedUser()
                //    .RequireClaim(ClaimTypes.DateOfBirth)
                //    .Build();

                //    config.DefaultPolicy = defaultAuthPolicy;


                //config.AddPolicy("Claim.DoB", policyBuilder =>
                //{
                //    policyBuilder.RequireClaim(ClaimTypes.DateOfBirth);
                //});



                // Register Requirement
                //config.AddPolicy("Claim.DoB", policyBuilder =>
                //{
                //    policyBuilder.AddRequirements(new CustomerRequireClaim(ClaimTypes.DateOfBirth));
                //});

                config.AddPolicy("Admin", policyBuilder => policyBuilder.RequireClaim(ClaimTypes.Role, "Admin"));
                //config.AddPolicy("Admin", policyBuilder => policyBuilder.RequireRole("Admin"));

                    


                config.AddPolicy("Claim.DoB", policyBuilder =>
              {
                  policyBuilder.RequireCustomClaim(ClaimTypes.DateOfBirth);
              });



            });


            // Add Handler to Service
            services.AddScoped<IAuthorizationHandler,CustomerRequireClaimHandler>();


            services.AddControllersWithViews();

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
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});
                endpoints.MapDefaultControllerRoute(); 

            });
        }
    }
}
