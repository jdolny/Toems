using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Entity;

namespace Toems_Service.Workflows
{
    public class GenerateClientGroupPolicy
    {
        public DtoActionResult Execute(int groupId)
        {
            var list = new List<DtoClientPolicy>();
            var inactivePolicyIds = new List<int>();
            var groupPolicies = new ServiceGroup().GetAssignedPolicies(groupId,new DtoSearchFilter());
            string warningMessage = null;

            if (groupPolicies.Count == 0)
            {
                //All Policies have been removed from the group.  Remove the active entry if it exists.
                var currentActiveGroupPolicy = new ServiceGroup().GetActiveGroupPolicy(groupId);
                if(currentActiveGroupPolicy != null)
                new ServiceActiveGroupPolicy().Delete(currentActiveGroupPolicy.Id);
                return new DtoActionResult() {Success = true };
            }

            foreach (var groupPolicy in groupPolicies)
            {
                var policyJson = new ServicePolicy().GetActivePolicy(groupPolicy.PolicyId);
                if (policyJson == null) //Policy hasn't been activated yet
                {
                    inactivePolicyIds.Add(groupPolicy.PolicyId);
                    continue;
                }

                var deserializedPolicy = JsonConvert.DeserializeObject<DtoClientPolicy>(policyJson.PolicyJson);
                deserializedPolicy.Order = groupPolicy.PolicyOrder;
                list.Add(deserializedPolicy);
            }
            if (inactivePolicyIds.Count > 0) //some of the polices are inactive
            {
                var listInactivePolicies = inactivePolicyIds.Select(policyId => new ServicePolicy().GetPolicy(policyId)).ToList();
                warningMessage =
                    "Warning: Inactive Policies Are Applied.  The Following Assigned Policies Will Not Function Until They Are Activated: " +
                    String.Join(", ", listInactivePolicies.Select(x => x.Name));
                
                if (list.Count == 0) //All Of this groups policies are inactive.  Remove the active group entry and stop processing
                {
                    var currentActiveGroupPolicy = new ServiceGroup().GetActiveGroupPolicy(groupId);
                    if (currentActiveGroupPolicy != null)
                        new ServiceActiveGroupPolicy().Delete(currentActiveGroupPolicy.Id);
                    return new DtoActionResult() {ErrorMessage = warningMessage};
                }

              
            }

            var json = JsonConvert.SerializeObject(list);
            var clientGroupPolicy = new EntityActiveGroupPolicy();
            clientGroupPolicy.GroupId = groupId;
            clientGroupPolicy.PolicyJson = json;

            new ServiceActiveGroupPolicy().InsertOrUpdate(clientGroupPolicy);

            //verify info was saved correctly and can be deserialized back to each invdividual policy
            var activeGroupPolicies = new ServiceActiveGroupPolicy().Get(clientGroupPolicy.Id);
            try
            {
                JsonConvert.DeserializeObject<List<DtoClientPolicy>>(activeGroupPolicies.PolicyJson);
                if (string.IsNullOrEmpty(warningMessage))
                    return new DtoActionResult() {Success = true, Id = clientGroupPolicy.Id};
                else
                    return new DtoActionResult() {ErrorMessage = warningMessage};
            }
            catch (Exception)
            {
                //back out any changes
                new ServiceActiveGroupPolicy().Delete(clientGroupPolicy.Id);
                return new DtoActionResult { ErrorMessage = "Could Not Verify Group Policy Deserialization", Id = 0 };
                //todo: add logging
                
            }
        }
    }
}
