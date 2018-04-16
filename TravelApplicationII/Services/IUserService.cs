using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelApplication.Controllers.WebAPI;
using TravelApplication.Models;

namespace TravelApplication.Services
{
    public interface IUserService
    {
        UserRole ValidateAndGetRoles(UserModel userModel);
        bool IsValidADUser(string username, string password);


    }
}
