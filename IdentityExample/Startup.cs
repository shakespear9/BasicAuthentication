using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityExample.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;

namespace IdentityExample
{
    public class Startup
    {
        private IConfiguration _configuration { get; }

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

         


            //services.AddDbContext<AppDbContext>(config =>
            //{
            //    config.UseInMemoryDatabase("Memory");
            //});

            services.AddDbContext<AppDbContext>(config =>
            {
                config.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
            });



            //AddIdentity registers the services
            // If using your own class you can specify at Generic of AddIdentity and also Add Generic of your own class to AppDbContext to Identity Which Class we are going to use as User
            services.AddIdentity<User, Role>(config =>
            {

                config.Password.RequiredLength = 4;
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
                config.SignIn.RequireConfirmedEmail = true;
              

            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();  /// Token for use to reset password, email etc.




            //services.AddIdentity<IdentityUser, IdentityRole>(config =>
            //{

            //    config.Password.RequiredLength = 4;
            //    config.Password.RequireDigit = false;
            //    config.Password.RequireNonAlphanumeric = false;
            //    config.Password.RequireUppercase = false;

            //})
            //    .AddEntityFrameworkStores<AppDbContext>()
            //    .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "Identity.Cookie";
                config.LoginPath = "/Home/Login";
            });


            var mailKitOptions = _configuration.GetSection("Email").Get<MailKitOptions>();

            services.AddMailKit(config => config.UseMailKit(mailKitOptions)); // .Get use to Type Conversion to specific generic type

               services.AddControllersWithViews();

         

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)

        {

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
                context.Database.EnsureCreated();
            }

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
