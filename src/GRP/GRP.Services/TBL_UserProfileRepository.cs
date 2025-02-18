using GRP.Core.Entities;
using GRP.Core.Repository;
using GRP.Services.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Services;

public class TBL_UserProfileRepository : GenericRepository<TBL_UserProfile>, ITBL_UserProfileRepository
{
    public TBL_UserProfileRepository(GRPDbContext context) : base(context)
    {

    }
}
