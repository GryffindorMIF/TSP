using System;
using EShop.Business.Interfaces;
using EShop.Business.Services;
using EShop.Data;
using EShop.Models.EFModels.User;
using EShop.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EShop
{
    public class Startup
    {
        private readonly string _contentRootPath;
        private RequestFileLogger _logger;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            _contentRootPath = env.ContentRootPath; // Get dynamic database path (depends on local machine)
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Construct connection string based on dynamic database path
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            if (connectionString.Contains("%CONTENTROOTPATH%"))
                connectionString = connectionString.Replace("%CONTENTROOTPATH%", _contentRootPath);
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.ConfigureApplicationCookie(options => { options.AccessDeniedPath = "/Account/Login"; });

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

            services.AddSession(options => { options.IdleTimeout = TimeSpan.FromSeconds(300); });


            var corsBuilder = new CorsPolicyBuilder();
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
            corsBuilder.AllowAnyOrigin(); // For anyone access.
            //corsBuilder.WithOrigins("http://localhost:44355");
            corsBuilder.AllowCredentials();

            services.AddCors(options => { options.AddPolicy("EShopCorsPolicy", corsBuilder.Build()); });

            // Add application services. (For dependency injection)
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IShoppingCartService, ShoppingCartService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IAddressManager, AddressManager>();
            services.AddTransient<INavigationService, NavigationService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IAttributeService, AttributeService>();
            services.AddTransient<IDataPortingService, DataPortingService>();
            services.AddTransient<ICardInfoService, CardInfoService>();
            services.AddSingleton<IDataPortingTrackerService, DataPortingTrackerService>();
            services.AddSingleton(Configuration);
            services.AddMvc();

            // Set SecurityStampValidator options to immediately update after a change in account's status
            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                // enables immediate logout, after updating the user's stat.
                //TODO: Test whether it impacts performance that we have to increase this
                options.ValidationInterval = TimeSpan.Zero;
            });

            services.AddLogging();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IApplicationLifetime applicationLifetime, IConfiguration configuration)
        {
            applicationLifetime.ApplicationStopping.Register(OnShutdown);
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

            app.UseCors("EShopCorsPolicy");

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseSession();

            bool requestLogging;
            if (bool.TryParse(configuration["RequestLoggingConfig:Enabled"], out requestLogging) && requestLogging)
            {
                _logger = RequestFileLogger.GetRequestFileLogger(configuration);
                app.Use(async (context, next) =>
                {
                    _logger.WriteLine(context.User.Identity.Name, context.Request.Method, context.Request.Path);
                    // Do work that doesn't write to the Response.
                    await next.Invoke();
                    // Do logging or other work that doesn't write to the Response.
                });
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void OnShutdown()
        {
            _logger.Dispose();
            Console.WriteLine("AAAShutDown");
        }
    }
}