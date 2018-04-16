using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelApplication.Models;

namespace TravelApplication.Services
{
    public interface IFISService
    {
        Task<List<CostCenter>> GetCostCenters();

        Task<List<Project>> GetProjectsByCostCenterName(string costCenterName);
    }
}
