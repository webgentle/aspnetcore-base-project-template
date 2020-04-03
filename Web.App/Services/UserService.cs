using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Web.App.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _context;
        public UserService(IHttpContextAccessor context)
        {
            _context = context;
        }

        public string GetUserId()
        {
            return _context.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public bool IsAuthenticated()
        {
            return _context.HttpContext.User.Identity.IsAuthenticated;
        }
    }
}
