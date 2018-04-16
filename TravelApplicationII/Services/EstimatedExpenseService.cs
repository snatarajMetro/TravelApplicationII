using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TravelApplication.DAL.Repositories;
using TravelApplication.Models;

namespace TravelApplication.Services
{
    public class EstimatedExpenseService : IEstimatedExpenseService
    {
        IEstimatedExpenseRepository estimatedExpenseRepository = new EstimatedExpenseRepository();

        public EstimatedExpense GetEstimatedExpenseByTravelRequestId(int travelRequestId)
        {
            EstimatedExpense result = estimatedExpenseRepository.GetTravelRequestDetail(travelRequestId);
            return result;
        }

        public async Task<int> SaveEstimatedExpenseRequest(EstimatedExpense estimatedExpense)
        {
            try
            {
                var result = await estimatedExpenseRepository.SaveEstimatedExpenseRequest(estimatedExpense).ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            
        }
    }
}