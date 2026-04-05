using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Core.Repository;

public interface IUnitOfWork : IDisposable
{
    public IUserRepository User { get; }
    public IProjectRepository Project { get; }
    public IGProjectRepository GProject { get; }
    public IClientRepository Client { get; }
   
    public ICountryMasterRepository CountryMaster { get; }
    public IStatusMasterRepository StatusMaster { get; }
    public IProjectMappingRepository ProjectMapping { get; }
    public IProjectsUrlRepository ProjectsUrl { get; }
    public ISuppliersRepository Suppliers { get; }
    public ISupplierPanelSizeRepository SupplierPanelSize { get; }
    public ISupplierLoginRepository SupplierLogin { get; }
    public IGenOperationsRepository GenOperations { get; }
    public ISupplierProjectsRepository SupplierProjects { get; }
    public ISupplierProjectMessageRepository SupplierProjectMessages { get; }
    public ISupplierProjectMessageAttachmentRepository SupplierProjectMessageAttachments { get; }
    public IQuestionsMasterRepository QuestionsMaster { get; }
    public IQuestionOptionRepository QuestionOption { get; }
    public IHashingSettingRepository HashingSetting { get; }
    public ILucidMarketplaceSettingRepository LucidMarketplaceSetting { get; }
    public ILucidMarketplaceSubscriptionRepository LucidMarketplaceSubscription { get; }
    public ILucidMarketplaceSyncLogRepository LucidMarketplaceSyncLog { get; }
    public ILucidMarketplaceOpportunityRepository LucidMarketplaceOpportunity { get; }
    public ILucidMarketplaceOpportunityQualificationRepository LucidMarketplaceOpportunityQualification { get; }
    public ILucidMarketplaceOpportunityQuotaRepository LucidMarketplaceOpportunityQuota { get; }
    public ILucidMarketplaceProjectMapRepository LucidMarketplaceProjectMap { get; }
    public ILucidMarketplaceEntryLinkRepository LucidMarketplaceEntryLink { get; }
    public ILucidMarketplaceRespondentAttemptRepository LucidMarketplaceRespondentAttempt { get; }
    public ILucidMarketplaceRespondentOutcomeRepository LucidMarketplaceRespondentOutcome { get; }
    public ILucidMarketplaceReconciliationRunRepository LucidMarketplaceReconciliationRun { get; }
    public ILucidMarketplaceReconciliationItemRepository LucidMarketplaceReconciliationItem { get; }
    public IZampliaSettingRepository ZampliaSetting { get; }
    public IZampliaSyncLogRepository ZampliaSyncLog { get; }
    public IZampliaSurveyRepository ZampliaSurvey { get; }
    public IZampliaSurveyQualificationRepository ZampliaSurveyQualification { get; }
    public IZampliaSurveyQuotaRepository ZampliaSurveyQuota { get; }
    public IZampliaProjectMapRepository ZampliaProjectMap { get; }
    public IZampliaEntryLinkRepository ZampliaEntryLink { get; }
    public IZampliaRespondentAttemptRepository ZampliaRespondentAttempt { get; }
    public IZampliaReconciliationRunRepository ZampliaReconciliationRun { get; }
    public IZampliaReconciliationItemRepository ZampliaReconciliationItem { get; }
    int Save();
}
