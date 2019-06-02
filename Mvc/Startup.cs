﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mvc
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // SERVICES !!!
    // This method gets called by the runtime. Use this method to add services 
    // to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.Configure<CookiePolicyOptions>(options =>
      {
        // This lambda determines whether user consent for non-essential 
        // cookies is needed for a given request.
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.None;
      });

      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

      // each kind of authentication is called scheme
      // const for string = "Cookie"
      services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
          options.AccessDeniedPath = "/Home/ErrorForbidden";
          options.LoginPath = "/Home/ErrorNotLoggedIn";
        });

      services.AddAuthorization(options =>
      {
        options.AddPolicy("MustBeAdmin", policy =>
          policy.RequireAuthenticatedUser().RequireRole("admin"));
      });
    }

    // MIDDLEWARES !!!
    // This method gets called by the runtime. Use this method to configure the 
    // HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for 
        // production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseCookiePolicy();

      // Must be right before UseMvc - using middleware the order matters
      app.UseAuthentication();

      app.UseMvc(routes =>
      {
        routes.MapRoute(
          name: "default",
          template: "{controller=Home}/{action=Index}/{id?}"
        );
      });
    }
  }
}