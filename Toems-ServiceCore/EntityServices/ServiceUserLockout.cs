using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceUserLockout(EntityContext ectx, ServiceUser userService)
    {
        public bool AccountIsLocked(int userId)
        {
            var userLockout = Get(userId);

            if (userLockout == null) return false;

            if (userLockout.LockedUntil == null) return false;

            if (DateTime.UtcNow > userLockout.LockedUntil)
            {
                //Lockout has expired reset lock
                DeleteUserLockouts(userId);
                return false;
            }
            return true;
        }

        public bool DeleteUserLockouts(int userId)
        {
            ectx.Uow.UserLockoutRepository.DeleteRange(x => x.UserId == userId);
            ectx.Uow.Save();
            return true;
        }

        public EntityUserLockout Get(int userId)
        {
            return ectx.Uow.UserLockoutRepository.GetFirstOrDefault(x => x.UserId == userId);
        }

        public void ProcessBadLogin(int userId)
        {
            var userLockout = Get(userId);
            if (userLockout == null)
            {
                ectx.Uow.UserLockoutRepository.Insert(new EntityUserLockout {UserId = userId, BadLoginCount = 1});
            }
            else
            {
                userLockout.BadLoginCount += 1;
                if (userLockout.BadLoginCount == 15)
                {
                    userLockout.LockedUntil = DateTime.UtcNow.AddMinutes(15);
                    userService.SendLockOutEmail(userId);
                }

                ectx.Uow.UserLockoutRepository.Update(userLockout, userLockout.Id);
            }
            ectx.Uow.Save();
        }
    }
}