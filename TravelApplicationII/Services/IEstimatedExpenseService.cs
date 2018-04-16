using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelApplication.Models;

namespace TravelApplication.Services
{
    public interface IEstimatedExpenseService
    {
        Task<int> SaveEstimatedExpenseRequest(EstimatedExpense estimatedExpense);
        EstimatedExpense GetEstimatedExpenseByTravelRequestId(int travelRequestId);
    }
}
