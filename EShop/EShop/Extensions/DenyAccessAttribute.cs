using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EShop.Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DenyAccessAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public DenyAccessAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context != null)
            {
                var user = context.HttpContext.User;
                var roles = Roles.Split(',');

                foreach (var role in roles)
                    if (user.IsInRole(role.Trim()))
                        context.Result = new NotFoundResult();
            }
            else
            {
                throw new ArgumentNullException();
            }
        }
    }
}