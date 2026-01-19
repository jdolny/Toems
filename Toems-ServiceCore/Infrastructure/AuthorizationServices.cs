using Toems_Common.Entity;
using Toems_ServiceCore.EntityServices;

namespace Toems_ServiceCore.Infrastructure
{
    public class AuthorizationServices(ServiceUser userService)
    {
        public bool IsAuthorized(int userId, string requiredRight)
        {
            var user =userService.GetUser(userId);
            var _currentUserRights = userService.GetEffectiveUserRights(userId).Select(right => right.Right).ToList();
            if (userService.IsAdmin(user.Id)) return true;
            return _currentUserRights.Any(right => right == requiredRight);
        }
    }
}