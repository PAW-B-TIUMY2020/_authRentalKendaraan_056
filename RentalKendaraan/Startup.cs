using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalKendaraan.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentalKendaraan_20180140056.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace RentalKendaraan
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<RentKendaraanContext>(options => 
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //service.AddDefaultIdentity<IdentityUser>()
            //  .AddEntityFrameworkStores<RentKendaraanContext>();
            services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultUI()
                .AddEntityFrameworkStores<RentKendaraanContext>().AddDefaultTokenProviders();

            services.AddAuthorization(Options =>
            {
                Options.AddPolicy("readonlypolicy",
                    builder => builder.RequireRole("Admin", "Manager", "Kasir"));
                Options.AddPolicy("writepolicy",
                    builder => builder.RequireRole("Admin", "Kasir"));
                Options.AddPolicy("editpolicy",
                    builder => builder.RequireRole("Admin", "Kasir"));
                Options.AddPolicy("deletepolicy",
                    builder => builder.RequireRole("Admin", "Kasir"));
            });

            services.AddScoped<Peminjaman>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
