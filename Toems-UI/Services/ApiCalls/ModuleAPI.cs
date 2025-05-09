using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_ApiCalls
{
    public class ModuleAPI : BaseAPI<EntityModule>
    {

        public ModuleAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }

        public IEnumerable<EntityModuleCategory> GetModuleCategories(string moduleGuid)
        {
            Request.Method = Method.Get;
            Request.AddParameter("moduleGuid", moduleGuid);
            Request.Resource = string.Format("{0}/GetModuleCategories/", Resource);
            return _apiRequest.Execute<List<EntityModuleCategory>>(Request);
        }

        public DtoActionResult Restore(int moduleId, EnumModule.ModuleType moduleType)
        {
            Request.Method = Method.Get;
            Request.AddParameter("moduleId", moduleId);
            Request.AddParameter("moduleType", moduleType);
            Request.Resource = string.Format("{0}/Restore/", Resource);
            return _apiRequest.Execute<DtoActionResult>(Request);
        }

        public DtoActionResult Archive(int moduleId, EnumModule.ModuleType moduleType)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/Archive/", Resource);
            Request.AddParameter("moduleId", moduleId);
            Request.AddParameter("moduleType", moduleType);

            var response = _apiRequest.Execute<DtoActionResult>(Request);
            if (response != null)
            {
                if (response.Id == 0)
                    response.Success = false;
            }
            else
            {
                return new DtoActionResult()
                {
                    ErrorMessage = "Unknown Exception.  Check The Exception Logs For More Info.",
                    Success = false
                };
            }
            return response;
        }

        public IEnumerable<EntityPolicy> GetModulePolicies(string moduleGuid)
        {
            Request.Method = Method.Get;
            Request.AddParameter("moduleGuid", moduleGuid);
            Request.Resource = string.Format("{0}/GetModulePolicies/", Resource);
            return _apiRequest.Execute<List<EntityPolicy>>(Request);
        }

        public IEnumerable<EntityImage> GetModuleImages(string moduleGuid)
        {
            Request.Method = Method.Get;
            Request.AddParameter("moduleGuid", moduleGuid);
            Request.Resource = string.Format("{0}/GetModuleImages/", Resource);
            return _apiRequest.Execute<List<EntityImage>>(Request);
        }

        public IEnumerable<EntityGroup> GetModuleGroups(string moduleGuid)
        {
            Request.Method = Method.Get;
            Request.AddParameter("moduleGuid", moduleGuid);
            Request.Resource = string.Format("{0}/GetModuleGroups/", Resource);
            return _apiRequest.Execute<List<EntityGroup>>(Request);
        }

        public IEnumerable<EntityComputer> GetModuleComputers(string moduleGuid)
        {
            Request.Method = Method.Get;
            Request.AddParameter("moduleGuid", moduleGuid);
            Request.Resource = string.Format("{0}/GetModuleComputers/", Resource);
            return _apiRequest.Execute<List<EntityComputer>>(Request);
        }

        public string IsModuleActive(string moduleGuid)
        {
            Request.Method = Method.Get;
            Request.AddParameter("moduleGuid", moduleGuid);
            Request.Resource = string.Format("{0}/IsModuleActive/", Resource);
            var response = _apiRequest.Execute<DtoApiStringResponse>(Request);
            if (response == null)
                return "Unknown Exception Occured";
            else
            {
                return response.Value;
            }
        }
    }
}