using BilleSpace.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace BilleSpace.Authorization
{
    public class OnlyReceptionist : AuthorizeAttribute, IAuthorizationFilter
    {
        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            var dbContext = context.HttpContext
            .RequestServices
            .GetService(typeof(BilleSpaceDbContext)) as BilleSpaceDbContext;

            var NameIdentifier = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var pass = dbContext.Receptionists.Any(rec => rec.UserNameIdentifier == NameIdentifier);

            if (!pass)
            {
                context.Result = new UnauthorizedResult();
            }
        }

    }
}
