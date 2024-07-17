**##Optimizely help docs **

https://docs.developers.optimizely.com/content-management-system/docs/installing-optimizely-net-5

**##Create dummy optimizely alloy project** 

dotnet new epi-alloy-mvc

**###integration of optimizely with Azure AD with Openid**

https://docs.developers.optimizely.com/content-management-system/docs/integrate-azure-ad-using-openid-connect

https://www.youtube.com/watch?v=cV7pdsYaKIM

**### Need to add "appsettings.json" explicitly to the project with below format**

appsettings.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Warning",
      "EPiServer": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "Azure": {
    "AD": {
      "Instance": "https://login.microsoftonline.com/",
      "Domain": "**********************",
      "TenantID": "**********************",
      "ClientID": "**********************",
      "CallbackPath": "/signin-oidc1",
      "ClientKey": "**********************"
    },
    "AD2": {
      "Instance": "https://login.microsoftonline.com/",
      "Domain": "**********************",
      "TenantID": "**********************",
      "ClientID": "**********************",
      "CallbackPath": "/signin-oidc",
      "ClientKey": "**********************"
    }
  },
  "AllowedHosts": "*",
  "EPiServer": {
    "CMS": {
      "MappedRoles": {
        "Items": {
          "CmsEditors": {
            "MappedRoles": [ "WebEditors", "WebAdmins" ]
          },
          "CmsAdmins": {
            "MappedRoles": [ "WebAdmins" ]
          }
        }
      },
      "Routing": {
        "StrictLanguageRouting": false
      },
      "Localization": {
        "FallbackCulture": "sv"
      },
      "Scheduler": {
        "Enabled": "false"
      },
      "LicensePath": {
        "Path": "C:\\Repos\\License.config"
      }
    },
    "Framework": {}
  }
}

**#Note:**

if we are using Roles assigned with Azure application with Azure Tenent, just make sure remove "emit_as_roles" from azure applucation menifest, which allow to get assigned roles while executing challange.
