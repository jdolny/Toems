using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceComputerCustomAttributes(ServiceContext ctx)
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
                if(ctx.Uow.CustomAttributeRepository.GetById(ca.CustomAttributeId) == null)
                    continue;

                var existingRelationship =
                    ctx.Uow.CustomComputerAttributeRepository.GetFirstOrDefault(x => x.ComputerId == first.ComputerId && x.CustomAttributeId == ca.CustomAttributeId);

                if (existingRelationship == null && !string.IsNullOrEmpty(ca.Value))
                {
                    //add it
                    ctx.Uow.CustomComputerAttributeRepository.Insert(ca);
                    ctx.Uow.Save();
                }

                else if(existingRelationship != null && string.IsNullOrEmpty(ca.Value))
                {
                    //delete it
                    ctx.Uow.CustomComputerAttributeRepository.Delete(existingRelationship.Id);
                    ctx.Uow.Save();
                }

                else if (existingRelationship != null && !string.IsNullOrEmpty(ca.Value))
                {
                    //update it
                    existingRelationship.Value = ca.Value;
                    ctx.Uow.CustomComputerAttributeRepository.Update(ca,ca.Id);
                    ctx.Uow.Save();
                }
            }


            actionResult.Success = true;
            return actionResult;
        }
    }
}