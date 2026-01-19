using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ServiceCore.EntityServices
{
    public class ServiceSmartGroupQuery(EntityContext ectx)
    {

        public DtoActionResult Add(EntitySmartGroupQuery query)
        {        
            var validationResult = Validate(query, true);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                ectx.Uow.SmartGroupQueryRepository.Insert(query);
                ectx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = query.Id;
            }
            else
            {
                actionResult.ErrorMessage = validationResult.ErrorMessage;
            }

            return actionResult;
        }

        public DtoActionResult Delete(int queryId)
        {
            var u = Get(queryId);
            if (u == null) return new DtoActionResult {ErrorMessage = "Query Not Found", Id = 0};
            ectx.Uow.SmartGroupQueryRepository.Delete(queryId);
            ectx.Uow.Save();
            var actionResult = new DtoActionResult();
         
                actionResult.Success = true;
                actionResult.Id = u.Id;
            
      
            return actionResult;
        }

        public EntitySmartGroupQuery Get(int queryId)
        {
            return ectx.Uow.SmartGroupQueryRepository.GetById(queryId);
        }

        public DtoActionResult Update(EntitySmartGroupQuery query)
        {
            var u = Get(query.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Query Not Found", Id = 0};
          
            var validationResult = Validate(query, false);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                ectx.Uow.SmartGroupQueryRepository.Update(query, query.Id);
                ectx.Uow.Save();
                actionResult.Success = true;
                actionResult.Id = query.Id;
            }
            else
            {
                actionResult.ErrorMessage = validationResult.ErrorMessage;
            }

            return actionResult;
        }

        private DtoValidationResult Validate(EntitySmartGroupQuery query, bool isNew)
        {
            //todo: add validation
            var validationResult = new DtoValidationResult { Success = true };
           

            return validationResult;
        }
    }
}