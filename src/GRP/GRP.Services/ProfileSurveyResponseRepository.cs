using GRP.Core.Entities;
using GRP.Core.Repository;
using GRP.Services.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Services;

public class ProfileSurveyResponseRepository : DapperGenericRepository<ProfileSurveyResponse>, IProfileSurveyResponseRepository
{
    public ProfileSurveyResponseRepository(DapperDBSetting dbSetting) : base(dbSetting)
    {

    }
}
