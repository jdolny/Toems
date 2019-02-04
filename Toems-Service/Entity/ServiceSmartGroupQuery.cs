using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_DataModel;

namespace Toems_Service.Entity
{
    public class ServiceSmartGroupQuery
    {
        private readonly UnitOfWork _uow;

        public ServiceSmartGroupQuery()
        {
            _uow = new UnitOfWork();
        }

        public DtoActionResult Add(EntitySmartGroupQuery query)
        {        
            var validationResult = Validate(query, true);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                _uow.SmartGroupQueryRepository.Insert(query);
                _uow.Save();
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
            _uow.SmartGroupQueryRepository.Delete(queryId);
            _uow.Save();
            var actionResult = new DtoActionResult();
         
                actionResult.Success = true;
                actionResult.Id = u.Id;
            
      
            return actionResult;
        }

        public EntitySmartGroupQuery Get(int queryId)
        {
            return _uow.SmartGroupQueryRepository.GetById(queryId);
        }

        public DtoActionResult Update(EntitySmartGroupQuery query)
        {
            var u = Get(query.Id);
            if (u == null) return new DtoActionResult {ErrorMessage = "Query Not Found", Id = 0};
          
            var validationResult = Validate(query, false);
            var actionResult = new DtoActionResult();
            if (validationResult.Success)
            {
                _uow.SmartGroupQueryRepository.Update(query, query.Id);
                _uow.Save();
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