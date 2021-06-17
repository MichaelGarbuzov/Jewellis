using Jewellis.App_Custom.Services.ClientCurrency;
using Jewellis.App_Custom.Services.ClientShoppingCart;
using Jewellis.App_Custom.Services.ClientTheme;
using Jewellis.App_Custom.Services.UserCache;
using Jewellis.Data;
using Jewellis.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Jewellis
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();

            services.AddDbContext<JewellisDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetSection("UserSecrets").GetSection("ConnectionStrings")["JewellisDbContext"]);
            });

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = "/login";
                options.AccessDeniedPath = "/error/access-denied";
            });

            // Scoped services:
            services.AddClientTheme(options =>
            {
                options.DefaultTheme = "default";
                options.SupportedThemes = new[]
                {
                    new Theme("default", "Default"),
                    new Theme("dark", "Dark"),
                    new Theme("light", "Light")
                };
            });
            services.AddClientCurrency(options =>
            {
                options.DefaultCurrency = "USD";
                options.SupportedCurrencies = new[]
                {
                    new Currency("USD", '$'),
                    new Currency("EUR", '€'),
                    new Currency("ILS", '₪'),
                };
            });
            services.AddClientShoppingCart();

            // Transient services:
            services.AddUserCache();
            services.AddTransient<UserIdentityService>();
            services.AddTransient<AuthenticateService>();
            services.AddTransient<UsersService>();
            services.AddTransient<OrdersService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error/ExceptionHandler");
                app.UseStatusCodePagesWithReExecute("/Error/StatusCodeHandler");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
            });
        }
    }
}
