using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Core.Repository;

public interface IUnitOfWork : IDisposable
{
    public ILoginDetailRepository LoginDetail { get; }
    public IUserRepository User { get; }
    public ITBL_UserProfileRepository TBL_UserProfile { get; }
    public IProfileInfoCategoryRepository ProfileInfoCategory { get; }
    public IProfileInfoSurveyRepository ProfileInfoSurvey { get; }
    public IQuestionTypeMasterRepository QuestionTypeMaster { get; }
    public IQuestionTypeSelectFrameworkRepository QuestionTypeSelectFramework { get; }
    public ISurveyRepository Survey { get; }
    public IPointsHistoryRepository PointsHistory { get; }
    public IPointsTransactionRepository PointsTransaction { get; }
    public IProfileSurveyResponseRepository ProfileSurveyResponse { get; }
    public ISurveyRedirectDetailsRepository SurveyRedirectDetails { get; }
    public ISurveyCriteriaRepository SurveyCriteria { get; }
    public ICountryMasterRepository CountryMaster { get; }

    int Save();
}
