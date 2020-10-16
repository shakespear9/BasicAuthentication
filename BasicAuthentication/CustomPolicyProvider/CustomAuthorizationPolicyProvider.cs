using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic.CustomerPolicyProvider
{

    public class SecurityLevelAttribute : AuthorizeAttribute
    {
        public SecurityLevelAttribute(int level)
        {
            Policy = $"{DynamicPolicies.SecurityLevel}.{level}";
        }
    }


    // {type}   1
    /// <summary>
    /// 2# Get All Dynamicpolicies that specify
    /// if found and match will call DynamicAuthorizationPolicyFactory
    /// </summary>
    public static class DynamicPolicies
    {
        public static IEnumerable<string> Get()
        {
            yield return SecurityLevel;
            yield return Rank;
            
        }


        public const string SecurityLevel = "SecurityLevel";
        public const string Rank = "Rank";
    }



    // 3
    /// <summary>
    /// Build the AuthorizationPolicy and Add Handler if needed 
    /// </summary>
    public static class DynamicAuthorizationPolicyFactory
    {
        public static AuthorizationPolicy Create(string policyName)
        {
            var parts = policyName.Split('.');
            var type = parts.First();
            var value = parts.Last();

            switch (type)
            {
                case DynamicPolicies.Rank:
                    return new AuthorizationPolicyBuilder()
                        .RequireClaim("Rank", value)
                        .Build();
                case DynamicPolicies.SecurityLevel:
                    return new AuthorizationPolicyBuilder()
                        .AddRequirements(new SecurityLevelRequirement(Convert.ToInt32(value)))
                        .Build();

                     

                default:
                    return null;
            }
        }
    }


    // 4
    public class SecurityLevelRequirement : IAuthorizationRequirement
    {
        public int Level { get; }
        public SecurityLevelRequirement(int level)
        {
            Level = level;
        }
    }


    // 4
    public class SecurityLevelHandler : AuthorizationHandler<SecurityLevelRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SecurityLevelRequirement requirement)
        {

            var claimValue = Convert.ToInt32(context.User.Claims
                .FirstOrDefault(c => c.Type == DynamicPolicies.SecurityLevel)
                ?.Value ?? "0");

            if(claimValue >= requirement.Level)
            {
                context.Succeed(requirement);
            }

          
            return Task.CompletedTask;

              
        }
    }


    // 2
    public class CustomAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public CustomAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
        }


        /// <summary>
        /// 1# {type}.{value} register policy to policy provider
        /// </summary>
        /// <param name="policyName">policyName = policyName in authorize attribute</param>
        /// <returns></returns>
        public override Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            foreach (var customPolicy in DynamicPolicies.Get())
            {

                /// loop in policy list and find which policy start with specific authorize attribute {type.value}
                if (policyName.StartsWith(customPolicy))
                {
                    //var policy = new AuthorizationPolicyBuilder().Build();
                    var policy = DynamicAuthorizationPolicyFactory.Create(policyName);

                    return Task.FromResult(policy);
                }
            }


            return base.GetPolicyAsync(policyName);
        }
    }
}
