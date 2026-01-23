using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public class QuestionsMasterRepository : DapperGenericRepository<QuestionsMaster>, IQuestionsMasterRepository
{
    public QuestionsMasterRepository(DapperDBSetting dbSetting) : base(dbSetting)
    {
    }
}

