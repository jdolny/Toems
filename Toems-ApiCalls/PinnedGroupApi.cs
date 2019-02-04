using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class PinnedGroupAPI : BaseAPI<EntityPinnedGroup>
    {
        public PinnedGroupAPI(string resource) : base(resource)
        {

        }

        public DtoActionResult Delete(int groupId, int userId)
        {
            Request.Method = Method.DELETE;
            Request.Resource = string.Format("{0}/Delete/", Resource);
            Request.AddParameter("groupId", groupId);
            Request.AddParameter("userId", userId);
            var response = new ApiRequest().Execute<DtoActionResult>(Request);
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
    }
}