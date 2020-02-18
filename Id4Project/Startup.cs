using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Id4Project.AuthorizationRequirements;
using Id4Project.Controllers;
using Id4Project.CustomerPolicyProvider;
using Id4Project.Transformer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Id4Project
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth", config =>
                {
                    config.Cookie.Name = "Grandmas.Cookie";
                    config.LoginPath = "/Home/Authenticate";
                });

            services.AddAuthorization(config =>
            {
                // var defaultPolicyBuilder = new AuthorizationPolicyBuilder();
                // var defaultPolicy = defaultPolicyBuilder
                //     .RequireAuthenticatedUser()
                //     .RequireClaim(ClaimTypes.DateOfBirth) // not authorized
                //     .Build();
                // config.DefaultPolicy = defaultPolicy ;

                // config.AddPolicy("ClaimDob", policyBuilder => 
                // {
                //     policyBuilder.RequireClaim(ClaimTypes.DateOfBirth);
                // });
                config.AddPolicy("Admin", policyBuilder => policyBuilder.RequireClaim(ClaimTypes.Role, "Admin"));
                config.AddPolicy("Claim.DOB", policyBuilder =>
                {
                    policyBuilder.RequireCustomClaim(ClaimTypes.DateOfBirth);
                });
                config.AddPolicy("Claim.Name", policyBuilder =>
                {
                    policyBuilder.RequireCustomClaim(ClaimTypes.Name);
                });
            });
            services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, SecurityLevelHandler>();
            services.AddScoped<IAuthorizationHandler, CustomRequirementClaimHandler>();
            services.AddScoped<IAuthorizationHandler, CookieJarAuthorizationHandler>();
            services.AddScoped<IClaimsTransformation, ClaimsTransformation>();



            services.AddControllersWithViews();
            // (config => {
            //     var defaultPolicyBuilder = new AuthorizationPolicyBuilder();
            //     var defaultPolicy = defaultPolicyBuilder
            //         .RequireAuthenticatedUser()
            //         .Build();

            //     // global authorization filter
            //     config.Filters.Add(new AuthorizeFilter(defaultPolicy));
            // });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
