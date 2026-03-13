using Toems_Common.Entity;
using Toems_ServiceCore.EntityServices;

namespace Toems_ServiceCore.Infrastructure
{
    public class AuthorizationServices(ServiceContext ctx)
    {
        public bool IsAuthorized(int userId, string requiredRight)
        {
            var user =ctx.User.GetUser(userId);
            var _currentUserRights = ctx.User.GetEffectiveUserRights(userId).Select(right => right.Right).ToList();
            if (ctx.User.IsAdmin(user.Id)) return true;
            return _currentUserRights.Any(right => right == requiredRight);
        }
    }
}