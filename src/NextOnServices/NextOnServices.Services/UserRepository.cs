using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(NextOnServicesDbContext context) : base(context)
    {

    }
}
