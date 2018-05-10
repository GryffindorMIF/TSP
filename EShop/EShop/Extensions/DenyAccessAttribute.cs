using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EShop.Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DenyAccessAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public DenyAccessAttribute(params string[] roles) : base()
        {
            Roles = string.Join(",", roles);
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context != null)
            {
                var user = context.HttpContext.User;
                string[] roles = Roles.Split(',');

                foreach (var role in roles)
                {
                    if (user.IsInRole(role.Trim()))
                    {
                        context.Result = new NotFoundResult();
                    }
                }
            }
            else throw new ArgumentNullException();
        }
    }
}
