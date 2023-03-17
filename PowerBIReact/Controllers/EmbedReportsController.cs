using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PowerBIReact.Helpers;

namespace PowerBIReact.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmbedReportsController : ControllerBase
    {
        private readonly EmbedConfiguration _embedConfigurationOptions;

        public EmbedReportsController(IOptions<EmbedConfiguration> embedConfigurationOptions)
        {
            _embedConfigurationOptions = embedConfigurationOptions.Value;
        }
        /// <summary>
        /// This API will used to get embed token for report
        /// </summary>
        /// <param name="ReportId"></param>
        /// <param name="WorkspaceId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("EmbedInfo")]
        public async Task<IActionResult> EmbedInfo(string ReportId, string WorkspaceId)
        {

            try
            {
                string configValidationResult = EmbedConfigValidator.ValidateConfig(_embedConfigurationOptions);
                if (configValidationResult != null)
                {
                    HttpContext.Response.StatusCode = 400;
                    return Ok(configValidationResult);
                }
                //call this method to get embed token
                var result = await EmbedInfo(ReportId, WorkspaceId, _embedConfigurationOptions);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

            

        //this method will used to get embed token
        private async Task<string> EmbedInfo(string ReportId, String WorkspaceId, EmbedConfiguration _embedConfigurationOptions)
        {
            try
            {
                string accessToken = AadService.GetAccessToken(_embedConfigurationOptions);
                string embedInfo = PbiEmbedService.GetEmbedParam(accessToken, _embedConfigurationOptions, ReportId, WorkspaceId);
                return embedInfo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
