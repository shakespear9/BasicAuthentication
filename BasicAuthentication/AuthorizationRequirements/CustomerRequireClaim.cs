using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.AuthorizationRequirements
{

    /// something is required to pass your question of are you allowed to go here ?
    public class CustomerRequireClaim : IAuthorizationRequirement
    {
        public CustomerRequireClaim(string claimType)
        {
            ClaimType = claimType;
        }

        public string ClaimType { get; }
    }


    public class CustomerRequireClaimHandler : AuthorizationHandler<CustomerRequireClaim>
    {
        //public CustomerRequireClaimHandler()
        //{

        //}


        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context
            , CustomerRequireClaim requirement)


        {

            ///  Search IEnum<Claim> In user
            var hasClaim = context.User.Claims.Any(x => x.Type == requirement.ClaimType); 
            if (hasClaim)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }


    public static class AuthorizationPolicyBuiderExtensions
    {
        public static AuthorizationPolicyBuilder RequireCustomClaim(
          this  AuthorizationPolicyBuilder builder,
          string claimType)
        {

            builder.AddRequirements(new CustomerRequireClaim(claimType));
            return builder;
        }

    }
}
