using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Taoxue.Mp.Sms.Website.Extensions;

namespace Taoxue.Mp.Sms.Website
{
    public class Startup
    {
        public Startup(IConfiguration configuration, ILoggerFactory logger)
        {
            Configuration = configuration;

            Logger = logger;
        }

        public IConfiguration Configuration { get; }

        public ILoggerFactory Logger { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddWxNoticeServer(Logger);

            var aas = System.AppDomain.CurrentDomain.GetAssemblies();

            var aass = Assembly.GetEntryAssembly().GetTypes();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }
        

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
