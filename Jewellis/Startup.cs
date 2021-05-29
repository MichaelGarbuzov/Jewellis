using Jewellis.App_Custom.Services.ClientCurrency;
using Jewellis.App_Custom.Services.ClientTheme;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Jewellis.Data;

namespace Jewellis
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
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();

            // Scoped services:
            services.AddClientTheme(options =>
            {
                options.DefaultTheme = "default";
                options.SupportedThemes = new[]
                {
                    new Theme("default", "1", "Default"),
                    new Theme("dark", "2", "Dark"),
                    new Theme("light", "3", "Light")
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

            services.AddDbContext<JewellisDbContext>(options => options.UseSqlServer(Configuration.GetSection("UserSecrets").GetSection("ConnectionStrings")["JewellisDbContext"]));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

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
