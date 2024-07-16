namespace OptimizelyAzureAD.Models.Pages
{
    [ContentType(DisplayName = "AzureLoginPage",
        GUID = "0bc38c4e-c37d-4e05-b80b-6f54b9393beb",
        Description = "Page with Azure AD login button")]
    [AvailableContentTypes(
    Availability = Availability.Specific,
    IncludeOn = new[] { typeof(StartPage) })]
    public class AzureLoginPage : SitePageData
    {
    }
}
