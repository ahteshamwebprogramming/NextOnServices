using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public class QuestionOptionRepository : DapperGenericRepository<QuestionOption>, IQuestionOptionRepository
{
    public QuestionOptionRepository(DapperDBSetting dbSetting) : base(dbSetting)
    {
    }
}

