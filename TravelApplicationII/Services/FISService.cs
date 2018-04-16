using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TravelApplication.DAL.Repositories;
using TravelApplication.Models;

namespace TravelApplication.Services
{
    public class FISService : IFISService
    {
        IFISRepository fisRepository = new FISRepository();

        public async Task<List<CostCenter>> GetCostCenters()
        {
            List<CostCenter> result = await fisRepository.GetCostCenters().ConfigureAwait(false);
            return result;
        }

        public async Task<List<Project>> GetProjectsByCostCenterName(string costCenterName)
        {
            List<Project> result = await fisRepository
                                        .GetProjectsByCostCenterName(costCenterName)
                                        .ConfigureAwait(false);

            return result;
        }
    }
}