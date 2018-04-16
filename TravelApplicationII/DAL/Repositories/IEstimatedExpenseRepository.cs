using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelApplication.Models;

namespace TravelApplication.DAL.Repositories
{
    public interface IEstimatedExpenseRepository
    {
        Task<int> SaveEstimatedExpenseRequest(EstimatedExpense request);
        EstimatedExpense GetTravelRequestDetail(int travelRequestId);
        EstimatedExpense GetTravelRequestDetailNew(DbConnection dbConn, string travelRequestId);
    }
}
