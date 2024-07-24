using EPiServer.ServiceLocation;
using EPiServer.Shell;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using OptimizelyAzureAD.Models.Pages;
using System.Text;

namespace OptimizelyAzureAD.Authentication
{
    public class CustomCookieOptionsMonitor : IOptionsMonitor<CookieAuthenticationOptions>
    {
        private readonly IOptionsFactory<CookieAuthenticationOptions> _optionsFactory;
        private CookieAuthenticationOptions _currentValue;

        public CustomCookieOptionsMonitor(IOptionsFactory<CookieAuthenticationOptions> optionsFactory)
        {
            _optionsFactory = optionsFactory;
        }

        public CookieAuthenticationOptions CurrentValue
        {
            get
            {
                string loginURL = GetGlobalLoginPageURL();
                string accessDeniedURL = GetAccessDeniedPageURL();
                if (_currentValue == null)
                {
                    _currentValue = _optionsFactory.Create(Options.DefaultName);
                    if (!string.IsNullOrEmpty(loginURL))
                    {
                        // Overwrite the LoginPath dynamically
                        _currentValue.LoginPath = new Microsoft.AspNetCore.Http.PathString(loginURL);
                    }
                    if (!string.IsNullOrEmpty(accessDeniedURL))
                    {
                        // Overwrite the LoginPath dynamically
                        _currentValue.LoginPath = new Microsoft.AspNetCore.Http.PathString(accessDeniedURL);
                    }
                }
                return _currentValue;
            }
        }

        public CookieAuthenticationOptions Get(string name)
        {
            var options = _optionsFactory.Create(name);
            string loginURL = GetGlobalLoginPageURL();
            string accessDeniedURL = GetAccessDeniedPageURL();

            if (!string.IsNullOrEmpty(loginURL))
            {
                // Overwrite the LoginPath for specific named options if needed
                options.LoginPath = new Microsoft.AspNetCore.Http.PathString(loginURL);
            }

            if (!string.IsNullOrEmpty(accessDeniedURL))
            {
                // Overwrite the LoginPath for specific named options if needed
                options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString(accessDeniedURL);
            }
            return options;
        }

        public IDisposable OnChange(Action<CookieAuthenticationOptions, string> listener) => null;

        private string GetGlobalLoginPageURL()
        {
            var startPageContentLink = SiteDefinition.Current.StartPage;
            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var startPage = contentLoader.Get<StartPage>(startPageContentLink);
            var globalLoginPage = startPage.GlobalLoginPage;
            if(globalLoginPage != null)
            {
                return GetUrlAbsolute(globalLoginPage);
            } 
            else
            {
                return string.Empty;
            }
        }

        private string GetAccessDeniedPageURL()
        {
            var startPageContentLink = SiteDefinition.Current.StartPage;
            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var startPage = contentLoader.Get<StartPage>(startPageContentLink);
            var accessDenied = startPage.AccessDeniedPage;
            if (accessDenied != null)
            {
                return GetUrlAbsolute(accessDenied);
            }
            else
            {
                return string.Empty;
            }
        }

        //public string GetExternalUrl(PageReference pageReference)
        //{
        //    var internalUrl = UrlResolver.Current.GetUrl(pageReference);
        //    var url = new UrlBuilder(internalUrl);
        //    Global.UrlRewriteProvider.ConvertToExternal(url, null, Encoding.UTF8);

        //    string externalUrl = HttpContext.Current == null
        //        ? UriSupport.AbsoluteUrlBySettings(url.ToString())
        //        : HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + url;

        //    return externalUrl;
        //}

        public static string GetUrlAbsolute(ContentReference contentLink, UrlResolver urlResolver = null)
        {
            if (urlResolver == null)
            {
                urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
            }

            var url = urlResolver.GetVirtualPath(contentLink).VirtualPath;

            return url;
        }
    }
}
