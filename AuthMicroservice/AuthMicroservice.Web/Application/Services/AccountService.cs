using AuthMicroservice.Domain.Base;
using AuthMicroservice.Infrastructure;
using AuthMicroservice.Web.Definitions.Identity;
using AutoMapper;
using Calabonga.Microservices.Core.Exceptions;
using Calabonga.Microservices.Core.Extensions;
using Calabonga.Microservices.Core.Validators;
using Calabonga.OperationResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using System.Security.Claims;

namespace AuthMicroservice.Web.Application.Services
{
    public class AccountService : IAccountService
    {
        
        private readonly ILogger<AccountService> _logger;
        private readonly ApplicationUserClaimsPrincipalFactory _claimsFactory;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountService(
            IUserStore<ApplicationUser> userStore,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<RoleManager<ApplicationRole>> loggerRole,
            IEnumerable<IRoleValidator<ApplicationRole>> roleValidators,
            ILogger<AccountService> logger,
            ILogger<UserManager<ApplicationUser>> loggerUser,
            ApplicationUserClaimsPrincipalFactory claimsFactory,
            IHttpContextAccessor httpContext,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _logger = logger;
            _claimsFactory = claimsFactory;
            _httpContext = httpContext;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <inheritdoc />
        public Guid GetCurrentUserId()
        {
            var identity = _httpContext.HttpContext?.User.Identity;
            var identitySub = identity?.GetSubjectId();
            return identitySub?.ToGuid() ?? Guid.Empty;
        }
  
        /// <summary>
        /// Returns ClaimPrincipal by user identity
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public async Task<ClaimsPrincipal> GetPrincipalByIdAsync(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new MicroserviceException();
            }
            var userManager = _userManager;
            var user = await userManager.FindByIdAsync(identifier);
            if (user == null)
            {
                throw new MicroserviceUserNotFoundException();
            }

            var defaultClaims = await _claimsFactory.CreateAsync(user);
            return defaultClaims;
        }

        /// <summary>
        /// Returns ClaimPrincipal by user identity
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<ClaimsPrincipal> GetPrincipalByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new MicroserviceException();
            }
            var userManager = _userManager;
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new MicroserviceUserNotFoundException();
            }

            var defaultClaims = await _claimsFactory.CreateAsync(user);
            return defaultClaims;
        }

        /// <summary>
        /// Returns user by his identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<ApplicationUser> GetByIdAsync(Guid id)
        {
            var userManager = _userManager;
            return userManager.FindByIdAsync(id.ToString());
        }

        /// <summary>
        /// Returns current user account information or null when user does not logged in
        /// </summary>
        /// <returns></returns>
        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var userManager = _userManager;
            var userId = GetCurrentUserId().ToString();
            var user = await userManager.FindByIdAsync(userId);
            return user;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ApplicationUser>> GetUsersByEmailsAsync(IEnumerable<string> emails)
        {
            var userManager = _userManager;
            var result = new List<ApplicationUser>();
            foreach (var email in emails)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user != null && !result.Contains(user))
                {
                    result.Add(user);
                }
            }
            return await Task.FromResult(result);
        }

        /// <summary>
        /// Check roles for current user
        /// </summary>
        /// <param name="roleNames"></param>
        /// <returns></returns>
        public async Task<PermissionValidationResult> IsInRolesAsync(string[] roleNames)
        {
            var userManager = _userManager;
            var userId = GetCurrentUserId().ToString();
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                var resultUserNotFound = new PermissionValidationResult();
                resultUserNotFound.AddError(AppData.Exceptions.UnauthorizedException);
                return await Task.FromResult(resultUserNotFound);
            }
            foreach (var roleName in roleNames)
            {
                var ok = await userManager.IsInRoleAsync(user, roleName);
                if (ok)
                {
                    return new PermissionValidationResult();
                }
            }

            var result = new PermissionValidationResult();
            result.AddError(AppData.Exceptions.UnauthorizedException);
            return result;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string roleName)
        {
            var userManager = _userManager;
            return await userManager.GetUsersInRoleAsync(roleName);
        }

        #region privates

        private async Task AddClaimsToUser(UserManager<ApplicationUser> userManager, ApplicationUser user, string role)
        {
            await userManager.AddClaimAsync(user, new Claim(OpenIddictConstants.Claims.Name, user.UserName));
            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.GivenName, user.FirstName ?? "John"));
            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Surname, user.LastName ?? "Doe"));
            await userManager.AddClaimAsync(user, new Claim(OpenIddictConstants.Claims.Email, user.Email));
            await userManager.AddClaimAsync(user, new Claim(OpenIddictConstants.Claims.Role, role));
        }

        #endregion
    }
}
