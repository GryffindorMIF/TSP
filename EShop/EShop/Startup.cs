using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EShop.Data;
using EShop.Models;
using EShop.Business;
using System.Diagnostics;

namespace EShop
{
    public class Startup
    {
        private string _contentRootPath;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            _contentRootPath = env.ContentRootPath;// Get dynamic database path (depends on local machine)
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Construct connection string based on dynamic database path
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            if (connectionString.Contains("%CONTENTROOTPATH%"))
            {
                connectionString = connectionString.Replace("%CONTENTROOTPATH%", _contentRootPath);
            }
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>(o =>
                {
                    // Custom password requirements
                    o.Password.RequireDigit = false;
                    o.Password.RequireLowercase = false;
                    o.Password.RequireUppercase = false;
                    o.Password.RequireNonAlphanumeric = false;
                    o.Password.RequiredLength = 5;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromSeconds(300);
            });

            // Add application services. (For dependency injection)
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IShoppingCartService, ShoppingCartService>();
            services.AddTransient<INavigationService, NavigationService>();
            services.AddMvc();

            // Set SecurityStampValidator options to immediately update after a change in account's status
            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                // enables immediate logout, after updating the user's stat.
                //TODO: Test whether it impacts performance that we have to increase this
                options.ValidationInterval = TimeSpan.Zero;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
