using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;

namespace NextOnServices.Services.DBContext;

public class UnitOfWork : IUnitOfWork
{
    private readonly NextOnServicesDbContext _context;
    private readonly DapperDBSetting _dbSetting;

    public UnitOfWork(NextOnServicesDbContext context, DapperDBSetting dbSetting)
    {
        _context = context;
        _dbSetting = dbSetting;
        User = new UserRepository(_context);
        Project = new ProjectRepository(dbSetting);
        GProject = new GProjectRepository(_context);
        Client = new ClientRepository(dbSetting);

        CountryMaster = new CountryMasterRepository(_context);
        StatusMaster = new StatusMasterRepository(_context);
        ProjectsUrl = new ProjectsUrlRepository(dbSetting);
        ProjectMapping = new ProjectMappingRepository(dbSetting);
        Suppliers = new SuppliersRepository(dbSetting);
        SupplierPanelSize = new SupplierPanelSizeRepository(dbSetting);
        SupplierLogin = new SupplierLoginRepository(dbSetting);
        GenOperations = new GenOperationsRepository(dbSetting);
        SupplierProjects = new SupplierProjectsRepository(dbSetting);
        SupplierProjectMessages = new SupplierProjectMessageRepository(dbSetting);
        SupplierProjectMessageAttachments = new SupplierProjectMessageAttachmentRepository(dbSetting);
        QuestionsMaster = new QuestionsMasterRepository(dbSetting);
        QuestionOption = new QuestionOptionRepository(dbSetting);
        HashingSetting = new HashingSettingRepository(dbSetting);
        TorfacMarketplaceSetting = new TorfacMarketplaceSettingRepository(dbSetting);
        LucidMarketplaceSetting = new LucidMarketplaceSettingRepository(dbSetting);
        LucidMarketplaceSubscription = new LucidMarketplaceSubscriptionRepository(dbSetting);
        LucidMarketplaceSyncLog = new LucidMarketplaceSyncLogRepository(dbSetting);
        LucidMarketplaceOpportunity = new LucidMarketplaceOpportunityRepository(dbSetting);
        LucidMarketplaceOpportunityQualification = new LucidMarketplaceOpportunityQualificationRepository(dbSetting);
        LucidMarketplaceOpportunityQuota = new LucidMarketplaceOpportunityQuotaRepository(dbSetting);
        LucidMarketplaceProjectMap = new LucidMarketplaceProjectMapRepository(dbSetting);
        LucidMarketplaceEntryLink = new LucidMarketplaceEntryLinkRepository(dbSetting);
        LucidMarketplaceRespondentAttempt = new LucidMarketplaceRespondentAttemptRepository(dbSetting);
        LucidMarketplaceRespondentOutcome = new LucidMarketplaceRespondentOutcomeRepository(dbSetting);
        LucidMarketplaceReconciliationRun = new LucidMarketplaceReconciliationRunRepository(dbSetting);
        LucidMarketplaceReconciliationItem = new LucidMarketplaceReconciliationItemRepository(dbSetting);
        ZampliaSetting = new ZampliaSettingRepository(dbSetting);
        ZampliaSyncLog = new ZampliaSyncLogRepository(dbSetting);
        ZampliaSurvey = new ZampliaSurveyRepository(dbSetting);
        ZampliaSurveyQualification = new ZampliaSurveyQualificationRepository(dbSetting);
        ZampliaSurveyQuota = new ZampliaSurveyQuotaRepository(dbSetting);
        ZampliaProjectMap = new ZampliaProjectMapRepository(dbSetting);
        ZampliaEntryLink = new ZampliaEntryLinkRepository(dbSetting);
        ZampliaRespondentAttempt = new ZampliaRespondentAttemptRepository(dbSetting);
        ZampliaReconciliationRun = new ZampliaReconciliationRunRepository(dbSetting);
        ZampliaReconciliationItem = new ZampliaReconciliationItemRepository(dbSetting);
    }
    public IUserRepository User
    { get; private set; }
    public IProjectRepository Project
    { get; private set; }
    public IGProjectRepository GProject
    { get; private set; }
    public IClientRepository Client
    { get; private set; }

    public ICountryMasterRepository CountryMaster
    { get; private set; }
    public IStatusMasterRepository StatusMaster { get; private set; }
    public IProjectMappingRepository ProjectMapping { get; private set; }
    public IProjectsUrlRepository ProjectsUrl { get; private set; }
    public ISuppliersRepository Suppliers { get; private set; }
    public ISupplierPanelSizeRepository SupplierPanelSize { get; private set; }
    public ISupplierLoginRepository SupplierLogin { get; private set; }
    public IGenOperationsRepository GenOperations { get; private set; }
    public ISupplierProjectsRepository SupplierProjects { get; private set; }
    public ISupplierProjectMessageRepository SupplierProjectMessages { get; private set; }
    public ISupplierProjectMessageAttachmentRepository SupplierProjectMessageAttachments { get; private set; }
    public IQuestionsMasterRepository QuestionsMaster { get; private set; }
    public IQuestionOptionRepository QuestionOption { get; private set; }
    public IHashingSettingRepository HashingSetting { get; private set; }
    public ITorfacMarketplaceSettingRepository TorfacMarketplaceSetting { get; private set; }
    public ILucidMarketplaceSettingRepository LucidMarketplaceSetting { get; private set; }
    public ILucidMarketplaceSubscriptionRepository LucidMarketplaceSubscription { get; private set; }
    public ILucidMarketplaceSyncLogRepository LucidMarketplaceSyncLog { get; private set; }
    public ILucidMarketplaceOpportunityRepository LucidMarketplaceOpportunity { get; private set; }
    public ILucidMarketplaceOpportunityQualificationRepository LucidMarketplaceOpportunityQualification { get; private set; }
    public ILucidMarketplaceOpportunityQuotaRepository LucidMarketplaceOpportunityQuota { get; private set; }
    public ILucidMarketplaceProjectMapRepository LucidMarketplaceProjectMap { get; private set; }
    public ILucidMarketplaceEntryLinkRepository LucidMarketplaceEntryLink { get; private set; }
    public ILucidMarketplaceRespondentAttemptRepository LucidMarketplaceRespondentAttempt { get; private set; }
    public ILucidMarketplaceRespondentOutcomeRepository LucidMarketplaceRespondentOutcome { get; private set; }
    public ILucidMarketplaceReconciliationRunRepository LucidMarketplaceReconciliationRun { get; private set; }
    public ILucidMarketplaceReconciliationItemRepository LucidMarketplaceReconciliationItem { get; private set; }
    public IZampliaSettingRepository ZampliaSetting { get; private set; }
    public IZampliaSyncLogRepository ZampliaSyncLog { get; private set; }
    public IZampliaSurveyRepository ZampliaSurvey { get; private set; }
    public IZampliaSurveyQualificationRepository ZampliaSurveyQualification { get; private set; }
    public IZampliaSurveyQuotaRepository ZampliaSurveyQuota { get; private set; }
    public IZampliaProjectMapRepository ZampliaProjectMap { get; private set; }
    public IZampliaEntryLinkRepository ZampliaEntryLink { get; private set; }
    public IZampliaRespondentAttemptRepository ZampliaRespondentAttempt { get; private set; }
    public IZampliaReconciliationRunRepository ZampliaReconciliationRun { get; private set; }
    public IZampliaReconciliationItemRepository ZampliaReconciliationItem { get; private set; }
    public void Dispose()
    {
        try { _context.Dispose(); }
        catch { }

        // Dispose Dapper-based repositories so their connections are returned to the pool
        foreach (var repo in new IDisposable[]
        {
            Project as IDisposable, Client as IDisposable, ProjectsUrl as IDisposable,
            ProjectMapping as IDisposable, Suppliers as IDisposable, SupplierPanelSize as IDisposable,
            SupplierLogin as IDisposable, GenOperations as IDisposable, SupplierProjects as IDisposable,
            SupplierProjectMessages as IDisposable, SupplierProjectMessageAttachments as IDisposable,
            QuestionsMaster as IDisposable, QuestionOption as IDisposable, HashingSetting as IDisposable,
            TorfacMarketplaceSetting as IDisposable,
            LucidMarketplaceSetting as IDisposable, LucidMarketplaceSubscription as IDisposable,
            LucidMarketplaceSyncLog as IDisposable, LucidMarketplaceOpportunity as IDisposable,
            LucidMarketplaceOpportunityQualification as IDisposable, LucidMarketplaceOpportunityQuota as IDisposable,
            LucidMarketplaceProjectMap as IDisposable, LucidMarketplaceEntryLink as IDisposable,
            LucidMarketplaceRespondentAttempt as IDisposable, LucidMarketplaceRespondentOutcome as IDisposable,
            LucidMarketplaceReconciliationRun as IDisposable, LucidMarketplaceReconciliationItem as IDisposable,
            ZampliaSetting as IDisposable, ZampliaSyncLog as IDisposable, ZampliaSurvey as IDisposable,
            ZampliaSurveyQualification as IDisposable, ZampliaSurveyQuota as IDisposable,
            ZampliaProjectMap as IDisposable, ZampliaEntryLink as IDisposable,
            ZampliaRespondentAttempt as IDisposable, ZampliaReconciliationRun as IDisposable,
            ZampliaReconciliationItem as IDisposable
        })
        {
            try { repo?.Dispose(); }
            catch { }
        }
    }

    public int Save()
    {
        return _context.SaveChanges();
    }
}
