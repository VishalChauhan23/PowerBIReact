namespace PowerBIReact.Helpers
{
    public class EmbedConfigValidator
    {
        /// <summary>
        /// Validates whether all the configuration parameters are set in appsettings.json file
        /// </summary>
        /// <param name="appSettings">Contains appsettings.json configuration values</param>
        /// <returns></returns>
        public static string ValidateConfig(EmbedConfiguration appSettings)
        {
            string message = null;
            bool isAuthModeMasterUser = appSettings.AuthenticationMode.Equals("masteruser", StringComparison.InvariantCultureIgnoreCase);
            bool isAuthModeServicePrincipal = appSettings.AuthenticationMode.Equals("serviceprincipal", StringComparison.InvariantCultureIgnoreCase);

            if (string.IsNullOrWhiteSpace(appSettings.AuthenticationMode))
            {
                message = "Authentication mode is not set in appsettings.json file";
            }
            else if (string.IsNullOrWhiteSpace(appSettings.AuthorityUri))
            {
                message = "Authority is not set in appsettings.json file";
            }
            else if (string.IsNullOrWhiteSpace(appSettings.ClientId))
            {
                message = "Client Id is not set in appsettings.json file";
            }
            else if (isAuthModeServicePrincipal && string.IsNullOrWhiteSpace(appSettings.TenantId))
            {
                message = "Tenant Id is not set in appsettings.json file";
            }
            else if (appSettings.Scope is null || appSettings.Scope.Length == 0)
            {
                message = "Scope is not set in appsettings.json file";
            }

            else if (isAuthModeMasterUser && string.IsNullOrWhiteSpace(appSettings.PbiUsername))
            {
                message = "Master user email is not set in appsettings.json file";
            }
            else if (isAuthModeMasterUser && string.IsNullOrWhiteSpace(appSettings.PbiPassword))
            {
                message = "Master user password is not set in appsettings.json file";
            }

            return message;
        }

        /// <summary>
        /// Checks whether a string is a valid guid
        /// </summary>
        /// <param name="configParam">String value</param>
        /// <returns>Boolean value indicating validity of the guid</returns>
        private static bool IsValidGuid(string configParam)
        {
            Guid result = Guid.Empty;
            return Guid.TryParse(configParam, out result);
        }
    }
}
