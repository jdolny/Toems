using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceBrowserToken(EntityContext ectx)
    {
        public EntityBrowserToken Create(int userId)
        {
            var token = new EntityBrowserToken
            {
                Token = Guid.NewGuid().ToString(),
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(1),
                UserId = userId
            };
            ectx.Uow.BrowserTokenRepository.Insert(token);
            ectx.Uow.Save();

            return token;
        }

        public bool Use(string token)
        {
            var tokenEntity = ectx.Uow.BrowserTokenRepository.Get(t => t.Token == token).FirstOrDefault();
            if (tokenEntity == null) return false;
            if (tokenEntity.ExpiresAtUtc < DateTime.UtcNow) return false;
            ectx.Uow.BrowserTokenRepository.Delete(tokenEntity.Id);
            ectx.Uow.Save();
            return true;
        }
    }
}