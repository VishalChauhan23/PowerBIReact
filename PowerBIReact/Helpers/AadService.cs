using Microsoft.Identity.Client;
using System.Security;

namespace PowerBIReact.Helpers
{
    public class AadService
    {  /// <summary>
       /// Generates and returns Access token
       /// </summary>
       /// <param name="appSettings">Contains appsettings.json configuration values</param>
       /// <returns></returns>
        public static string GetAccessToken(EmbedConfiguration appSettings)
        {
            AuthenticationResult authenticationResult = null;
            if (appSettings.AuthenticationMode.Equals("masteruser", StringComparison.InvariantCultureIgnoreCase))
            {
                // Create a public client to authorize the app with the AAD app
                IPublicClientApplication clientApp = PublicClientApplicationBuilder.Create(appSettings.ClientId)
                    .WithAuthority(appSettings.AuthorityUri)
                        .Build();
                var userAccounts = clientApp.GetAccountsAsync().Result;
                try
                {

                    authenticationResult = clientApp.AcquireTokenSilent(appSettings.Scope, userAccounts.FirstOrDefault()).ExecuteAsync().Result;
                }
                catch (MsalUiRequiredException)
                {
                    try
                    {
                        SecureString password = new SecureString();
                        foreach (var key in appSettings.PbiPassword)
                        {
                            password.AppendChar(key);
                        }
                        authenticationResult = clientApp.AcquireTokenByUsernamePassword(appSettings.Scope, appSettings.PbiUsername, password).ExecuteAsync().Result;
                    }
                    catch (MsalException)
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        if (ex is MsalUiRequiredException || ex.InnerException is MsalUiRequiredException)
                        {
                            SecureString password = new SecureString();
                            foreach (var key in appSettings.PbiPassword)
                            {
                                password.AppendChar(key);
                            }
                            authenticationResult = clientApp.AcquireTokenByUsernamePassword(appSettings.Scope, appSettings.PbiUsername, password).ExecuteAsync().Result;
                        }
                    }
                    catch (MsalException)
                    {
                        throw;
                    }
                }
            }


            try
            {
                return authenticationResult.AccessToken;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
