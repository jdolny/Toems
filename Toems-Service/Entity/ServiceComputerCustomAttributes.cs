using System.Collections.Generic;
using System.Linq;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceComputerCustomAttributes
    {
        private readonly UnitOfWork _uow;

        public ServiceComputerCustomAttributes()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult AddOrUpdate(List<EntityCustomComputerAttribute> attributes)
        {
            var first = attributes.FirstOrDefault();
            if (first == null) return new DtoActionResult { ErrorMessage = "No Attributes Were In The List", Id = 0 };
            var allSame = attributes.All(x => x.ComputerId == first.ComputerId);
            if (!allSame) return new DtoActionResult { ErrorMessage = "The List Must Be For A Single Computer.", Id = 0 };
            var actionResult = new DtoActionResult();

            foreach (var ca in attributes)
            {
                if(_uow.CustomAttributeRepository.GetById(ca.CustomAttributeId) == null)
                    continue;

                var existingRelationship =
                    _uow.CustomComputerAttributeRepository.GetFirstOrDefault(x => x.ComputerId == first.ComputerId && x.CustomAttributeId == ca.CustomAttributeId);

                if (existingRelationship == null && !string.IsNullOrEmpty(ca.Value))
                {
                    //add it
                    _uow.CustomComputerAttributeRepository.Insert(ca);
                    _uow.Save();
                }

                else if(existingRelationship != null && string.IsNullOrEmpty(ca.Value))
                {
                    //delete it
                    _uow.CustomComputerAttributeRepository.Delete(existingRelationship.Id);
                    _uow.Save();
                }

                else if (existingRelationship != null && !string.IsNullOrEmpty(ca.Value))
                {
                    //update it
                    existingRelationship.Value = ca.Value;
                    _uow.CustomComputerAttributeRepository.Update(ca,ca.Id);
                    _uow.Save();
                }
            }


            actionResult.Success = true;
            return actionResult;
        }
    }
}