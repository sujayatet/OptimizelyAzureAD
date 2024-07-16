using OptimizelyAzureAD.Extensions;
using EPiServer.Cms.Shell;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Scheduler;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using EPiServer.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using EPiServer.Shell.Security;

namespace OptimizelyAzureAD;

public class Startup
{
    private readonly IWebHostEnvironment _webHostingEnvironment;
    public IConfiguration Configuration { get; }

    public Startup(IWebHostEnvironment webHostingEnvironment, IConfiguration configuration)
    {
        _webHostingEnvironment = webHostingEnvironment;
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        if (_webHostingEnvironment.IsDevelopment())
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(_webHostingEnvironment.ContentRootPath, "App_Data"));

            services.Configure<SchedulerOptions>(options => options.Enabled = false);
        }

        var azureAdConfig = Configuration.GetSection("Azure:AD");
        var azureAd2Config = Configuration.GetSection("Azure:AD2");

        services
             .AddAuthentication(options =>
             {
                 options.DefaultAuthenticateScheme = "azure-cookie";
                 options.DefaultChallengeScheme = "azure";
             })
             .AddCookie("azure-cookie", options =>
             {
                 options.Events.OnSignedIn = async ctx =>
                 {
                     if (ctx.Principal?.Identity is ClaimsIdentity claimsIdentity)
                     {
                         // Syncs user and roles so they are available to the CMS
                         var synchronizingUserService = ctx
                               .HttpContext
                               .RequestServices
                               .GetRequiredService<ISynchronizingUserService>();

                         await synchronizingUserService.SynchronizeAsync(claimsIdentity);
                     }
                 };
             })
             .AddOpenIdConnect("azure", options =>
             {
                 options.SignInScheme = "azure-cookie";
                 options.SignOutScheme = "azure-cookie";
                 options.ResponseType = OpenIdConnectResponseType.Code;
                 options.CallbackPath = "/signin-oidc1";
                 options.UsePkce = true;

                 options.Authority = "https://login.microsoftonline.com/" + azureAdConfig["TenantID"] + "/v2.0";
                 options.ClientId = azureAdConfig["ClientID"];
                 options.ClientSecret = azureAdConfig["ClientKey"];

                 options.Scope.Clear();
                 options.Scope.Add(OpenIdConnectScope.OfflineAccess); // if you need refresh tokens
                 options.Scope.Add(OpenIdConnectScope.Email);
                 options.Scope.Add(OpenIdConnectScope.OpenIdProfile);
                 options.MapInboundClaims = false;

                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     RoleClaimType = "roles",
                     NameClaimType = "preferred_username",
                     ValidateIssuer = true
                 };

                 options.Events.OnRedirectToIdentityProvider = ctx =>
                 {
                     // Prevent redirects from being cached.
                     ctx.Response.Headers.CacheControl = "no-cache,no-store";

                     // Prevent redirect loop
                     if (ctx.Response.StatusCode == 401)
                     {
                         ctx.HandleResponse();
                     }

                     return Task.CompletedTask;
                 };

                 options.Events.OnAuthenticationFailed = context =>
                 {
                     context.HandleResponse();
                     context.Response.BodyWriter.WriteAsync(Encoding.ASCII.GetBytes(context.Exception.Message));
                     return Task.CompletedTask;
                 };

                 options.Events.OnTokenValidated = context =>
                 {
                     var user = context.Principal.Identity;
                     if (user != null)
                     {
                         var claims = ((System.Security.Claims.ClaimsIdentity)context.Principal.Identity).Claims;
                         foreach (var claim in claims)
                         {
                             Console.WriteLine($"{claim.Type}: {claim.Value}");
                         }
                     }
                     var identity = context.Principal.Identity as ClaimsIdentity;
                     if (identity != null)
                     {
                         var roleClaim = identity.Claims.FirstOrDefault(c => c.Type == "roles");
                         if (roleClaim != null)
                         {
                             identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                         }
                     }
                     return Task.CompletedTask;
                 };
             });

        services
             .AddAuthentication(options =>
             {
                 options.DefaultAuthenticateScheme = "azure-ad2-cookie";
                 options.DefaultChallengeScheme = "azure-ad2";
             })
             .AddCookie("azure-ad2-cookie", options =>
             {
                 options.Events.OnSignedIn = async ctx =>
                 {
                     if (ctx.Principal?.Identity is ClaimsIdentity claimsIdentity)
                     {
                         // Syncs user and roles so they are available to the CMS
                         var synchronizingUserService = ctx
                               .HttpContext
                               .RequestServices
                               .GetRequiredService<ISynchronizingUserService>();

                         await synchronizingUserService.SynchronizeAsync(claimsIdentity);
                     }
                 };
             })
             .AddOpenIdConnect("azure-ad2", options =>
             {
                 options.SignInScheme = "azure-ad2-cookie";
                 options.SignOutScheme = "azure-ad2-cookie";
                 options.ResponseType = OpenIdConnectResponseType.Code;
                 options.CallbackPath = "/signin-oidc";
                 options.UsePkce = true;

                 options.Authority = "https://login.microsoftonline.com/" + azureAd2Config["TenantID"] + "/v2.0";
                 options.ClientId = azureAd2Config["ClientID"];
                 options.ClientSecret = azureAd2Config["ClientKey"];

                 options.Scope.Clear();
                 options.Scope.Add(OpenIdConnectScope.OfflineAccess); // if you need refresh tokens
                 options.Scope.Add(OpenIdConnectScope.Email);
                 options.Scope.Add(OpenIdConnectScope.OpenIdProfile);
                 options.MapInboundClaims = false;

                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     RoleClaimType = "roles",
                     NameClaimType = "preferred_username",
                     ValidateIssuer = true
                 };

                 options.Events.OnRedirectToIdentityProvider = ctx =>
                 {
                     // Prevent redirects from being cached.
                     ctx.Response.Headers.CacheControl = "no-cache,no-store";

                     // Prevent redirect loop
                     if (ctx.Response.StatusCode == 401)
                     {
                         ctx.HandleResponse();
                     }

                     return Task.CompletedTask;
                 };

                 options.Events.OnAuthenticationFailed = context =>
                 {
                     context.HandleResponse();
                     context.Response.BodyWriter.WriteAsync(Encoding.ASCII.GetBytes(context.Exception.Message));
                     return Task.CompletedTask;
                 };

                 options.Events.OnTokenValidated = context =>
                 {
                     var user = context.Principal.Identity;
                     if (user != null)
                     {
                         var claims = ((System.Security.Claims.ClaimsIdentity)context.Principal.Identity).Claims;
                         foreach (var claim in claims)
                         {
                             Console.WriteLine($"{claim.Type}: {claim.Value}");
                         }
                     }
                     var identity = context.Principal.Identity as ClaimsIdentity;
                     if (identity != null)
                     {
                         var roleClaim = identity.Claims.FirstOrDefault(c => c.Type == "roles");
                         if (roleClaim != null)
                         {
                             identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                         }
                     }
                     return Task.CompletedTask;
                 };
             });

        services.AddCms();
        services.AddAlloy();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // Required by Wangkanai.Detection
        app.UseDetection();
        //app.UseSession();

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapContent();
            endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
        });
    }
}