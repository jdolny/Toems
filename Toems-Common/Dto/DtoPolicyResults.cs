using System.Collections.Generic;
using Toems_Common.Entity;

namespace Toems_Common.Dto
{
    public class DtoPolicyResults
    {
        public List<EntityPolicyHistory> Histories;
        public List<EntityCustomInventory> CustomInventories; 
        public DtoPolicyResults()
        {
            Histories = new List<EntityPolicyHistory>();
            CustomInventories = new List<EntityCustomInventory>();
        }
    }
}
