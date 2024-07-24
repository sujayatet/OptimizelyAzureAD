﻿using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OptimizelyAzureAD.Models.Pages;
using OptimizelyAzureAD.Models.ViewModels;

namespace OptimizelyAzureAD.Controllers
{
    public class AzureLoginPageController : PageControllerBase<AzureLoginPage>
    {
        public IActionResult Index(AzureLoginPage currentPage, string returnUrl = "/")
        {
            var model = PageViewModel.Create(currentPage);

            // Example value to store in session
            HttpContext.Session.SetString("returnURL", returnUrl);

            return View(model);
        }

        [HttpPost]
        public IActionResult Login(string provider, string returnUrl = "/")
        {
            // Retrieve the value from session
            var expectedURL = HttpContext.Session.GetString("returnURL");
            if (!string.IsNullOrEmpty(expectedURL))
            {
                returnUrl = expectedURL;
            }

            if (provider == "AzureAd")
            {
                string redirectUrl = Url.Action(nameof(LoginCallback), "AzureLoginPage", new {returnUrl});
                return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, "azure");
            }
            if (provider == "AzureAd2")
            {
                string redirectUrl = Url.Action(nameof(LoginCallback), "AzureLoginPage", new {returnUrl});
                return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, "azure-ad2");
            }
            if (provider == "Saml2Instance1")
            {
                var redirectUrl = Url.Action(nameof(LoginCallback), "AzureLoginPage", new { returnUrl });
                var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
                return Challenge(properties, "Saml2Instance1");
            }
            return View();
        }

        public IActionResult LoginCallback(string returnURL = "/")
        {
            if (User.Identity.IsAuthenticated)
            {
                return LocalRedirect(returnURL);
            }
            else
            {
                return LocalRedirect(returnURL);
            }
        }
    }
}

