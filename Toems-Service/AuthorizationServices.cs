using System.Collections.Generic;
using System.Linq;
using Toems_Common.Entity;
using Toems_Service.Entity;

namespace Toems_Service
{
    public class AuthorizationServices
    {
        private readonly EntityToemsUser _toemsUser;
        private readonly List<string> _currentUserRights;
        private readonly string _requiredRight;
        private readonly ServiceUser _userServices;

        public AuthorizationServices(int userId, string requiredRight)
        {
            _userServices = new ServiceUser();
            _toemsUser = _userServices.GetUser(userId);
            _currentUserRights = _userServices.GetEffectiveUserRights(userId).Select(right => right.Right).ToList();
            _requiredRight = requiredRight;
        }

        public bool IsAuthorized()
        {
            if (_userServices.IsAdmin(_toemsUser.Id)) return true;
            return _currentUserRights.Any(right => right == _requiredRight);
        }
    }
}