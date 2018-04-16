using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelApplication.Models;

namespace TravelApplication.DAL.Repositories
{
    public interface IUserRepository
    {
        UserRole ValidateAndGetRoles(UserModel userModel);
    }
}
