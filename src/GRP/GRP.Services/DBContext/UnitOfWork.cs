using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using GRP.Core.Entities;
using GRP.Core.Repository;

namespace GRP.Services.DBContext;

public class UnitOfWork : IUnitOfWork
{
    private readonly GRPDbContext _context;
    private readonly DapperDBSetting _dbSetting;

    public UnitOfWork(GRPDbContext context, DapperDBSetting dbSetting)
    {
        _context = context;
        _dbSetting = dbSetting;
        LoginDetail = new LoginDetailRepository(_context);
        User = new UserRepository(_context);
        TBL_UserProfile = new TBL_UserProfileRepository(_context);

        //Project = new ProjectRepository(dbSetting);
        ProfileInfoCategory = new ProfileInfoCategoryRepository(dbSetting);
        ProfileInfoSurvey = new ProfileInfoSurveyRepository(dbSetting);
        QuestionTypeMaster = new QuestionTypeMasterRepository(dbSetting);
        QuestionTypeSelectFramework = new QuestionTypeSelectFrameworkRepository(dbSetting);
        Survey = new SurveyRepository(dbSetting);
        PointsHistory = new PointsHistoryRepository(dbSetting);
        PointsTransaction = new PointsTransactionRepository(dbSetting);
        ProfileSurveyResponse = new ProfileSurveyResponseRepository(dbSetting);
        SurveyRedirectDetails = new SurveyRedirectDetailsRepository(dbSetting);
        SurveyCriteria = new SurveyCriteriaRepository(dbSetting);
        CountryMaster = new CountryMasterRepository(dbSetting);
       
    }
    public ILoginDetailRepository LoginDetail { get; private set; }
    public IUserRepository User { get; private set; }
    public ITBL_UserProfileRepository TBL_UserProfile { get; private set; }
    public IProfileInfoCategoryRepository ProfileInfoCategory { get; private set; }
    public IProfileInfoSurveyRepository ProfileInfoSurvey { get; private set; }
    public IQuestionTypeMasterRepository QuestionTypeMaster { get; private set; }
    public IQuestionTypeSelectFrameworkRepository QuestionTypeSelectFramework { get; private set; }
    public ISurveyRepository Survey { get; private set; }
    public IPointsHistoryRepository PointsHistory { get; private set; }
    public IPointsTransactionRepository PointsTransaction { get; private set; }
    public IProfileSurveyResponseRepository ProfileSurveyResponse { get; private set; }
    public ISurveyRedirectDetailsRepository SurveyRedirectDetails { get; private set; }
    public ISurveyCriteriaRepository SurveyCriteria { get; private set; }
    public ICountryMasterRepository CountryMaster { get; private set; }
  
    public void Dispose()
    {
        try { _context.Dispose(); }
        catch { }
    }

    public int Save()
    {
        return _context.SaveChanges();
    }
}
