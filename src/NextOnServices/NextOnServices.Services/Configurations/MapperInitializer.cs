using AutoMapper;
using NextOnServices.Core.Entities;
using NextOnServices.Infrastructure.Models.Account;
using NextOnServices.Infrastructure.Models.APIProjects;
using NextOnServices.Infrastructure.Models.Client;
using NextOnServices.Infrastructure.Models.Masters;
using NextOnServices.Infrastructure.Models.Projects;
using NextOnServices.Infrastructure.Models.Questionnaire;
using NextOnServices.Infrastructure.Models.Settings;
using NextOnServices.Infrastructure.Models.Supplier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Services.Configurations;

public class MapperInitializer : Profile
{
    public MapperInitializer()
    {
        CreateMap<User, UserDTO>().ReverseMap();
        CreateMap<Project, ProjectDTO>().ReverseMap();
        CreateMap<Client, ClientDTO>().ReverseMap();
        CreateMap<CountryMaster, CountryMasterDTO>().ReverseMap();
        CreateMap<StatusMaster, StatusMasterDTO>().ReverseMap();
        CreateMap<ProjectMapping, ProjectMappingDTO>().ReverseMap();
        CreateMap<ProjectsUrl, ProjectsUrlDTO>().ReverseMap();
        CreateMap<Supplier, SupplierDTO>().ReverseMap();
        CreateMap<SupplierPanelSize, SupplierPanelSizeDTO>().ReverseMap();
        CreateMap<SupplierLogin, SupplierLoginDTO>().ReverseMap();
        CreateMap<QuestionsMaster, QuestionsMasterDTO>().ReverseMap();
        CreateMap<QuestionOption, QuestionOptionDTO>().ReverseMap();
        CreateMap<HashingSetting, HashingSettingDTO>().ReverseMap();
        CreateMap<LucidMarketplaceSetting, LucidMarketplaceSettingDTO>().ReverseMap();
        CreateMap<LucidMarketplaceSubscription, LucidMarketplaceSubscriptionDTO>().ReverseMap();
        CreateMap<LucidMarketplaceSyncLog, LucidMarketplaceSyncLogDTO>().ReverseMap();
        CreateMap<LucidMarketplaceOpportunity, LucidMarketplaceOpportunityDTO>().ReverseMap();
        CreateMap<LucidMarketplaceOpportunityQualification, LucidMarketplaceOpportunityQualificationDTO>().ReverseMap();
        CreateMap<LucidMarketplaceOpportunityQuota, LucidMarketplaceOpportunityQuotaDTO>().ReverseMap();
        CreateMap<LucidMarketplaceProjectMap, LucidMarketplaceProjectMapDTO>().ReverseMap();
        CreateMap<LucidMarketplaceEntryLink, LucidMarketplaceEntryLinkDTO>().ReverseMap();
        CreateMap<LucidMarketplaceRespondentAttempt, LucidMarketplaceRespondentAttemptDTO>().ReverseMap();
        CreateMap<LucidMarketplaceRespondentOutcome, LucidMarketplaceRespondentOutcomeDTO>().ReverseMap();
        CreateMap<LucidMarketplaceReconciliationRun, LucidMarketplaceReconciliationRunDTO>().ReverseMap();
        CreateMap<LucidMarketplaceReconciliationItem, LucidMarketplaceReconciliationItemDTO>().ReverseMap();
        CreateMap<ZampliaSetting, ZampliaSettingDTO>().ReverseMap();
        CreateMap<ZampliaSyncLog, ZampliaSyncLogDTO>().ReverseMap();
        CreateMap<ZampliaSurvey, ZampliaSurveyDTO>().ReverseMap();
        CreateMap<ZampliaSurveyQualification, ZampliaSurveyQualificationDTO>().ReverseMap();
        CreateMap<ZampliaSurveyQuota, ZampliaSurveyQuotaDTO>().ReverseMap();
        CreateMap<ZampliaProjectMap, ZampliaProjectMapDTO>().ReverseMap();
        CreateMap<ZampliaEntryLink, ZampliaEntryLinkDTO>().ReverseMap();
        CreateMap<ZampliaRespondentAttempt, ZampliaRespondentAttemptDTO>().ReverseMap();
        CreateMap<ZampliaReconciliationRun, ZampliaReconciliationRunDTO>().ReverseMap();
        CreateMap<ZampliaReconciliationItem, ZampliaReconciliationItemDTO>().ReverseMap();
    }
}
