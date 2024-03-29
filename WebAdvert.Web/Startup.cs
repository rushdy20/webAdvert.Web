using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebAdvert.Web.ServiceClients;
using WebAdvert.Web.Services;
using AutoMapper;
using Polly;
using Polly.Extensions.Http;

namespace WebAdvert.Web
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
            services.AddCognitoIdentity();
             services.AddControllersWithViews();
             services.ConfigureApplicationCookie(options => { options.LoginPath = "/Accounts/Login"; });
             services.AddTransient<IFileUploader, S3FileUploader>();
            
             services.AddHttpClient<IAdvertApiClient, AdvertApiClient>().AddPolicyHandler(GetRetryPolicy())
                 .AddPolicyHandler(GetCircuitBrakerPattern());
             services.AddAutoMapper(typeof(Startup));


            //services.AddCognitoIdentity(config =>
            //{
            //    config.Password = new PasswordOptions
            //    {
            //        RequireDigit = false,
            //        RequiredLength = 6,
            //        RequireLowercase = false,
            //        RequiredUniqueChars = 0,
            //        RequireNonAlphanumeric = false,
            //        RequireUppercase = false
            //    };
            //});
            services.AddTransient<IFileUploader, S3FileUploader>();

        }

        private IAsyncPolicy<HttpResponseMessage> GetCircuitBrakerPattern()
        {
            return HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));
        }

        private IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions.HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound).WaitAndRetryAsync(5,
                    retryAttemp => TimeSpan.FromSeconds(Math.Pow(2, retryAttemp)));
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
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
