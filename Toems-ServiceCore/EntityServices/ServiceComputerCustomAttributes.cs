using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceComputerCustomAttributes(EntityContext ectx)
    {
        public DtoActionResult AddOrUpdate(List<EntityCustomComputerAttribute> attributes)
        {
            var first = attributes.FirstOrDefault();
            if (first == null) return new DtoActionResult { ErrorMessage = "No Attributes Were In The List", Id = 0 };
            var allSame = attributes.All(x => x.ComputerId == first.ComputerId);
            if (!allSame) return new DtoActionResult { ErrorMessage = "The List Must Be For A Single Computer.", Id = 0 };
            var actionResult = new DtoActionResult();

            foreach (var ca in attributes)
            {
                if(ectx.Uow.CustomAttributeRepository.GetById(ca.CustomAttributeId) == null)
                    continue;

                var existingRelationship =
                    ectx.Uow.CustomComputerAttributeRepository.GetFirstOrDefault(x => x.ComputerId == first.ComputerId && x.CustomAttributeId == ca.CustomAttributeId);

                if (existingRelationship == null && !string.IsNullOrEmpty(ca.Value))
                {
                    //add it
                    ectx.Uow.CustomComputerAttributeRepository.Insert(ca);
                    ectx.Uow.Save();
                }

                else if(existingRelationship != null && string.IsNullOrEmpty(ca.Value))
                {
                    //delete it
                    ectx.Uow.CustomComputerAttributeRepository.Delete(existingRelationship.Id);
                    ectx.Uow.Save();
                }

                else if (existingRelationship != null && !string.IsNullOrEmpty(ca.Value))
                {
                    //update it
                    existingRelationship.Value = ca.Value;
                    ectx.Uow.CustomComputerAttributeRepository.Update(ca,ca.Id);
                    ectx.Uow.Save();
                }
            }


            actionResult.Success = true;
            return actionResult;
        }
    }
}