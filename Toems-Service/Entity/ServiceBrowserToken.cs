using System;
using System.Linq;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceBrowserToken
    {
        private readonly UnitOfWork _uow;

        public ServiceBrowserToken()
        {
            _uow = new UnitOfWork();
        }

        public EntityBrowserToken Create(int userId)
        {
            var token = new EntityBrowserToken
            {
                Token = Guid.NewGuid().ToString(),
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(1),
                UserId = userId
            };
            _uow.BrowserTokenRepository.Insert(token);
            _uow.Save();

            return token;
        }

        public bool Use(string token)
        {
            var tokenEntity = _uow.BrowserTokenRepository.Get(t => t.Token == token).FirstOrDefault();
            if (tokenEntity == null) return false;
            if (tokenEntity.ExpiresAtUtc < DateTime.UtcNow) return false;
            _uow.BrowserTokenRepository.Delete(tokenEntity.Id);
            _uow.Save();
            return true;
        }
    }
}