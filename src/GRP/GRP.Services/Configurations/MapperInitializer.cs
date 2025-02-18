using AutoMapper;
using GRP.Core.Entities;
using GRP.Infrastructure.Models.Account;
using GRP.Infrastructure.Models.Masters;
using GRP.Infrastructure.Models.Survey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Services.Configurations;

public class MapperInitializer : Profile
{
    public MapperInitializer()
    {
        CreateMap<LoginDetail, LoginDetailDTO>().ReverseMap();
        CreateMap<User, UserDTO>().ReverseMap();
        CreateMap<ProfileInfoCategory, ProfileInfoCategoryDTO>().ReverseMap();
        CreateMap<ProfileInfoSurvey, ProfileInfoSurveyDTO>().ReverseMap();
        CreateMap<QuestionTypeMaster, QuestionTypeMasterDTO>().ReverseMap();
        CreateMap<QuestionTypeSelectFramework, QuestionTypeSelectFrameworkDTO>().ReverseMap();
        CreateMap<TBL_UserProfile, TBL_UserProfileDTO>().ReverseMap();
        CreateMap<Survey, SurveyDTO>().ReverseMap();
        CreateMap<PointsHistory, PointsHistoryDTO>().ReverseMap();
        CreateMap<PointsTransaction, PointsTransactionDTO>().ReverseMap();
        CreateMap<ProfileSurveyResponse, ProfileSurveyResponseDTO>().ReverseMap();
        CreateMap<SurveyRedirectDetails, SurveyRedirectDetailsDTO>().ReverseMap();
        CreateMap<SurveyCriteria, SurveyCriteriaDTO>().ReverseMap();
        CreateMap<CountryMaster, CountryMasterDTO>().ReverseMap();
       
    }
}
