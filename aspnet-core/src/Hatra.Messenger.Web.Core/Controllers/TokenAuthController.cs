using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Abp.Application.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using Abp.UI;
using Hatra.Messenger.Authentication.External;
using Hatra.Messenger.Authentication.JwtBearer;
using Hatra.Messenger.Authorization;
using Hatra.Messenger.Authorization.Users;
using Hatra.Messenger.EntityFrameworkCore.Repositories;
using Hatra.Messenger.Identity;
using Hatra.Messenger.Models.TokenAuth;
using Hatra.Messenger.MultiTenancy;
using Hatra.Messenger.SMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace Hatra.Messenger.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TokenAuthController : MessengerControllerBase
    {
        private readonly LogInManager _logInManager;
        private readonly ITenantCache _tenantCache;
        private readonly AbpLoginResultTypeHelper _abpLoginResultTypeHelper;
        private readonly TokenAuthConfiguration _configuration;
        private readonly IExternalAuthConfiguration _externalAuthConfiguration;
        private readonly IExternalAuthManager _externalAuthManager;
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly ISmsAppService _smsAppService;
        private readonly UserManager _userManager;
        private readonly SignInManager _signInManager;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private const string TempKey = "TEMP_USER_";
        public TokenAuthController(
            LogInManager logInManager,
            ITenantCache tenantCache,
            AbpLoginResultTypeHelper abpLoginResultTypeHelper,
            TokenAuthConfiguration configuration,
            IExternalAuthConfiguration externalAuthConfiguration,
            IExternalAuthManager externalAuthManager,
            UserRegistrationManager userRegistrationManager, ISmsAppService smsAppService, UserManager userManager, SignInManager signInManager, IRefreshTokenRepository refreshTokenRepository)
        {
            _logInManager = logInManager;
            _tenantCache = tenantCache;
            _abpLoginResultTypeHelper = abpLoginResultTypeHelper;
            _configuration = configuration;
            _externalAuthConfiguration = externalAuthConfiguration;
            _externalAuthManager = externalAuthManager;
            _userRegistrationManager = userRegistrationManager;
            _smsAppService = smsAppService;
            _userManager = userManager;
            _signInManager = signInManager;
            _refreshTokenRepository = refreshTokenRepository;
        }

        [HttpPost]
        public async Task<AuthenticateResultModel> Authenticate([FromBody] AuthenticateModel model)
        {
            var loginResult = await GetLoginResultAsync(
                model.UserNameOrEmailAddress,
                model.Password,
                GetTenancyNameOrNull()
            );
            var device = GetDevice();
            var ip = GetIpAddress();
            var accessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity));
            //var refreshToken =await CreateRefreshTokenAsync(loginResult.User.Id,device,ip);
            return new AuthenticateResultModel
            {
                AccessToken = accessToken,
                EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds,
                RefreshExpireInSeconds = (int)_configuration.RefreshExpiration.TotalSeconds,
                UserId = loginResult.User.Id,
                //RefreshToken = refreshToken
            };
        }

        [HttpGet]
        public List<ExternalLoginProviderInfoModel> GetExternalAuthenticationProviders()
        {
            return ObjectMapper.Map<List<ExternalLoginProviderInfoModel>>(_externalAuthConfiguration.Providers);
        }

        [HttpPost]
        public async Task<ExternalAuthenticateResultModel> ExternalAuthenticate([FromBody] ExternalAuthenticateModel model)
        {
            var externalUser = await GetExternalUserInfo(model);

            var loginResult = await _logInManager.LoginAsync(new UserLoginInfo(model.AuthProvider, model.ProviderKey, model.AuthProvider), GetTenancyNameOrNull());

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    {
                        var accessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity));
                        return new ExternalAuthenticateResultModel
                        {
                            AccessToken = accessToken,
                            EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                            ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds
                        };
                    }
                case AbpLoginResultType.UnknownExternalLogin:
                    {
                        var newUser = await RegisterExternalUserAsync(externalUser);
                        if (!newUser.IsActive)
                        {
                            return new ExternalAuthenticateResultModel
                            {
                                WaitingForActivation = true
                            };
                        }

                        // Try to login again with newly registered user!
                        loginResult = await _logInManager.LoginAsync(new UserLoginInfo(model.AuthProvider, model.ProviderKey, model.AuthProvider), GetTenancyNameOrNull());
                        if (loginResult.Result != AbpLoginResultType.Success)
                        {
                            throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                                loginResult.Result,
                                model.ProviderKey,
                                GetTenancyNameOrNull()
                            );
                        }

                        return new ExternalAuthenticateResultModel
                        {
                            AccessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity)),
                            ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds
                        };
                    }
                default:
                    {
                        throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                            loginResult.Result,
                            model.ProviderKey,
                            GetTenancyNameOrNull()
                        );
                    }
            }
        }

        [HttpPost]
        public async Task<LoginResultModel> LoginRequest([FromBody] LoginRequestModel model)
        {
            var user = await _userManager.FindByPhoneNumber(model.PhoneNumber);

            if (user == null)
            {
                var userName = $"{TempKey}{model.PhoneNumber}";
                var email = $"{userName}@hatra.com";
                user = await _userRegistrationManager.RegisterAsync(TempKey, TempKey, email, userName, userName, true, model.PhoneNumber);
            }

            var token = await _userManager.GenerateUserTokenAsync(user, "PasswordlessLoginProvider",
                "passwordless-auth");

            var res = await _smsAppService.SendVerifyCodeAsync(user.PhoneNumber, token);

            return new LoginResultModel(res, String.Empty);
        }

        [HttpPost]
        public async Task<AuthenticateResultModel> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByPhoneNumber(model.PhoneNumber);

            if (user == null)
            {
                throw new KeyNotFoundException();
            }

            var isValid = await _userManager.VerifyUserTokenAsync(user, "PasswordlessLoginProvider", "passwordless-auth", model.Token);
            if (!isValid)
            {
                throw new UnauthorizedAccessException("The token " + model.Token + " is not valid for " + model.PhoneNumber);
            }

            await _userManager.UpdateSecurityStampAsync(user);
            await _signInManager.SignInAsync(user, true);
            await _logInManager.SaveSuccessfulLoginAttemptAsync(user.Id, user.UserName);

            var device = GetDevice();
            var ip = GetIpAddress();
            var accessToken = CreateAccessToken(await CreateJwtClaimsAsync(user));
            var refreshToken =await CreateRefreshTokenAsync(user.Id,device,ip);
            return new AuthenticateResultModel
            {
                AccessToken = accessToken,
                EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds,
                RefreshExpireInSeconds = (int)_configuration.RefreshExpiration.TotalSeconds,
                UserId = user.Id,
                RefreshToken = refreshToken
            };

        }

        [HttpPost]
        [AbpAllowAnonymous]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticateResultModel>> Refresh([FromBody] RefreshRequest request)
        {
            if (!IsValidToken(request.AccessToken))
            {
                return BadRequest();
            }

            var device = GetDevice();
            var ip = GetIpAddress();
            var user = await _userManager.GetByRefreshTokenAsync(request.RefreshToken, device,ip);
            if (user == null)
            {
                return Unauthorized();
            }
            await _userManager.UpdateSecurityStampAsync(user);

            await _signInManager.SignInAsync(user, true);

            var accessToken = CreateAccessToken(await CreateJwtClaimsAsync(user));
            var refreshToken =await CreateRefreshTokenAsync(user.Id,device,ip);
            return new AuthenticateResultModel
            {
                AccessToken = accessToken,
                EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds,
                RefreshExpireInSeconds = (int)_configuration.RefreshExpiration.TotalSeconds,
                UserId = user.Id,
                RefreshToken = refreshToken
            };

        }

        private string GetIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            if (HttpContext.Connection.RemoteIpAddress != null)
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            return ":::0";
        }
        private string GetDevice()
        {
            if (Request.Headers.ContainsKey("X-Device"))
                return Request.Headers["X-Device"];
            return "-";
        }

        private bool IsValidToken(string token)
        {
            try
            {
                new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = _configuration.SigningCredentials.Key
                }, out SecurityToken validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<User> RegisterExternalUserAsync(ExternalAuthUserInfo externalUser)
        {
            var user = await _userRegistrationManager.RegisterAsync(
                externalUser.Name,
                externalUser.Surname,
                externalUser.EmailAddress,
                externalUser.EmailAddress,
                Authorization.Users.User.CreateRandomPassword(),
                true, null
            );

            user.Logins = new List<UserLogin>
            {
                new UserLogin
                {
                    LoginProvider = externalUser.Provider,
                    ProviderKey = externalUser.ProviderKey,
                    TenantId = user.TenantId
                }
            };

            await CurrentUnitOfWork.SaveChangesAsync();

            return user;
        }

        private async Task<ExternalAuthUserInfo> GetExternalUserInfo(ExternalAuthenticateModel model)
        {
            var userInfo = await _externalAuthManager.GetUserInfo(model.AuthProvider, model.ProviderAccessCode);
            if (userInfo.ProviderKey != model.ProviderKey)
            {
                throw new UserFriendlyException(L("CouldNotValidateExternalUser"));
            }

            return userInfo;
        }

        private string GetTenancyNameOrNull()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }

            return _tenantCache.GetOrNull(AbpSession.TenantId.Value)?.TenancyName;
        }

        private async Task<AbpLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
        {
            var loginResult = await _logInManager.LoginAsync(usernameOrEmailAddress, password, tenancyName);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    return loginResult;
                default:
                    throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(loginResult.Result, usernameOrEmailAddress, tenancyName);
            }
        }

        private string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
        {
            var now = DateTime.UtcNow;

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(expiration ?? _configuration.Expiration),
                signingCredentials: _configuration.SigningCredentials
            );
            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        private async Task<string> CreateRefreshTokenAsync(long userId,string device,string ip)
        {
            var now = DateTime.UtcNow;
            var exp = now.Add(_configuration.RefreshExpiration);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                notBefore: now,
                expires:exp,
                signingCredentials: _configuration.SigningCredentials
            );
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            await _refreshTokenRepository.InsertOrUpdateAsync(userId, token, ip, device, exp);
            return token;
        }

        private static List<Claim> CreateJwtClaims(ClaimsIdentity identity)
        {
            var claims = identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == ClaimTypes.NameIdentifier);

            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            });

            return claims;
        }

        private async Task<List<Claim>> CreateJwtClaimsAsync(User user)
        {
            var claims = new List<Claim>();
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Email,user.EmailAddress),
                new Claim(ClaimTypes.GivenName,user.FullName),
                new Claim("AspNet.Identity.SecurityStamp",user.SecurityStamp),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            });
            if (user.TenantId.HasValue)
            {
                claims.Add(new Claim("http://www.aspnetboilerplate.com/identity/claims/tenantId", user.TenantId.ToString()));
            }
            if (roles != null && roles.Any())
            {
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            }
            return claims;
        }


        private string GetEncryptedAccessToken(string accessToken)
        {
            return SimpleStringCipher.Instance.Encrypt(accessToken, AppConsts.DefaultPassPhrase);
        }


    }
}
