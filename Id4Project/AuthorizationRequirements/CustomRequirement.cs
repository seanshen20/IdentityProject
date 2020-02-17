using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Id4Project.AuthorizationRequirements
{
    public class CustomRequirement : IAuthorizationRequirement
    {
        public string ClaimType { get; }

        public CustomRequirement(string claimType)
        {
            this.ClaimType = claimType;


        }
    }

    public class CustomRequirementClaimHandler : AuthorizationHandler<CustomRequirement>
    {
        public CustomRequirementClaimHandler()
        {

        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomRequirement requirement)
        {
            var hasClaim = context.User.Claims.Any(x => x.Type == requirement.ClaimType);
            if (hasClaim)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public static class AuthorizationPolicyBuilderExtensions
    {
        public static AuthorizationPolicyBuilder RequireCustomClaim(this AuthorizationPolicyBuilder builder, string claimType)
        {
            builder.AddRequirements(new CustomRequirement(claimType));
            return builder;
        }
    }
}