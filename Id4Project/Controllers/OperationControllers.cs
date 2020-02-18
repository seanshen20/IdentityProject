using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Id4Project.Controllers
{
    public class OperationControllers : Controller
    {
        private readonly IAuthorizationService _service;
        public OperationControllers(IAuthorizationService service)
        {
            _service = service;

        }
        public async Task<IActionResult> Open()
        {
            var cookieJar = new CookieJar(); // db
            var requirement = CookieJarAuthOperation.open;
            var result = await _service.AuthorizeAsync(User, cookieJar, requirement);


            return View();
        }
    }

    public class CookieJarAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, CookieJar>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, CookieJar cookieJar)
        {
            if (requirement.Name == CookieJarOperations.Look)
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    context.Succeed(requirement);
                }
            }
            else if (requirement.Name == CookieJarOperations.ComeNear)
            {
                if (context.User.HasClaim("Friend", "Good"))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }

    public static class CookieJarAuthOperation
    {
        public static OperationAuthorizationRequirement open => new OperationAuthorizationRequirement()
        {
            Name = CookieJarOperations.Open
        };
    }
    public static class CookieJarOperations
    {
        public static string Open = "Open";
        public static string TakeCookie = "TakeCookie";
        public static string ComeNear = "ComeNear";
        public static string Look = "Look";
    }

    public class CookieJar
    {
        public string Name { get; set; }
    }
}