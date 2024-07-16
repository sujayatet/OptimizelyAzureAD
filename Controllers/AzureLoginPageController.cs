using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OptimizelyAzureAD.Models.Pages;
using OptimizelyAzureAD.Models.ViewModels;

namespace OptimizelyAzureAD.Controllers
{
    public class AzureLoginPageController : PageControllerBase<AzureLoginPage>
    {
        public IActionResult Index(AzureLoginPage currentPage)
        {
            var model = PageViewModel.Create(currentPage);
            return View(model);
        }

        [HttpPost]
        public IActionResult Login(string provider, string returnUrl = "/")
        {
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

