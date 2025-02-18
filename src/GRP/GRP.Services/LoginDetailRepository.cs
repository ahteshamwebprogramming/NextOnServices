using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRP.Core.Entities;
using GRP.Core.Repository;
using GRP.Services.DBContext;

namespace GRP.Services;

public class LoginDetailRepository : GenericRepository<LoginDetail>, ILoginDetailRepository
{
    public LoginDetailRepository(GRPDbContext context) : base(context)
    {

    }
}
