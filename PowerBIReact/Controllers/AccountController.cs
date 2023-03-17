using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PowerBIReact.Helpers;
using PowerBIReact.Models.DB;
using PowerBIReact.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace PowerBIReact.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private JWTSettings JWTSettings { get; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PowerBiEmbeddedDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
             IOptions<JWTSettings> jwtSettings,
             UserManager<ApplicationUser> userManager,
             PowerBiEmbeddedDbContext context,
             SignInManager<ApplicationUser> signInManager
       )
        {
            JWTSettings = jwtSettings?.Value;
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }


        #region Login
        [Route("~/api/Login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    IActionResult response = Unauthorized();
                    var res = await AuthenticateUser(login);
                    if (res!= null && !string.IsNullOrEmpty(res.Username))
                    {
                        return StatusCode((int)HttpStatusCode.OK , res);
                    }

                    return StatusCode((int)HttpStatusCode.NotFound);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
        #endregion

        #region LogOut
        [Route("~/api/LogOut")]
        [HttpPost]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();

            return Ok();
        }
        #endregion


        #region Common Methods

        private string GenerateJSONWebToken(LoginRequest login, string[] roles)
        {

            // authentication successful so generate jwt token
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTSettings.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
              new Claim("Username",login.Username.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token");
            claimsIdentity.AddClaims(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            var token = new JwtSecurityToken(issuer: JWTSettings.Issuer,
                audience: JWTSettings.Issuer,
                 claims: claimsIdentity.Claims,
                 notBefore: DateTime.Now,

                 expires: DateTime.Now.AddMinutes(JWTSettings.TokenExpiry_Minutes),


                 signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private async Task<LoginResponse> AuthenticateUser(LoginRequest login)
        {
            try
            {
                LoginResponse response = new LoginResponse();

                string userName = login.Username;

                login.Username = userName;

                var user = await UserNameExists(userName);

                if (user)
                {
                    var userDetails = await _userManager.FindByEmailAsync(userName);

                    if (userDetails == null)
                        userDetails = await _userManager.FindByNameAsync(userName);

                    login.Password = login.Password.Trim();
                    var result = await PasswordSignIn(login);

                    var roles = new List<string>();

                    if (userDetails != null)
                    {
                        var userRoles = await _userManager.GetRolesAsync(userDetails);
                        roles = userRoles.ToList();
                    }

                    if (result.Succeeded && userDetails.IsActive)
                    {
                        response.Token = GenerateJSONWebToken(login, roles.ToArray());
                        response.Username = userDetails.UserName;
                        response.Email = userDetails.Email;
                        response.Id = userDetails.Id;
                        response.UserRoles = roles.ToArray();
                        response.IsActive = userDetails.IsActive;
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private async Task<bool> UserNameExists(string username)
        {
            try
            {
                var email = await _userManager.FindByEmailAsync(username);
                var user = await _userManager.FindByNameAsync(username);

                if (user == null && email == null)
                    return false;
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private async Task<Microsoft.AspNetCore.Identity.SignInResult> PasswordSignIn(LoginRequest login)
        {

            try
            {

                var user = await _userManager.FindByEmailAsync(login.Username);

                if (user == null)
                    user = await _userManager.FindByNameAsync(login.Username);

                var result = await _signInManager
              .PasswordSignInAsync(user, login.Password, false, true);

                return result;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
