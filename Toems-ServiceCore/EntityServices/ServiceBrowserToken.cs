using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceBrowserToken(ServiceContext ctx)
    {
        public EntityBrowserToken Create(int userId)
        {
            var token = new EntityBrowserToken
            {
                Token = Guid.NewGuid().ToString(),
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(1),
                UserId = userId
            };
            ctx.Uow.BrowserTokenRepository.Insert(token);
            ctx.Uow.Save();

            return token;
        }

        public bool Use(string token)
        {
            var tokenEntity = ctx.Uow.BrowserTokenRepository.Get(t => t.Token == token).FirstOrDefault();
            if (tokenEntity == null) return false;
            if (tokenEntity.ExpiresAtUtc < DateTime.UtcNow) return false;
            ctx.Uow.BrowserTokenRepository.Delete(tokenEntity.Id);
            ctx.Uow.Save();
            return true;
        }
    }
}