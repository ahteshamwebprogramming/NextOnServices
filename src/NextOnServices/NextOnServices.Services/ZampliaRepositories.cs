using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public class ZampliaSettingRepository : DapperGenericRepository<ZampliaSetting>, IZampliaSettingRepository
{
    public ZampliaSettingRepository(DapperDBSetting dbSetting) : base(dbSetting) { }
}

public class ZampliaSyncLogRepository : DapperGenericRepository<ZampliaSyncLog>, IZampliaSyncLogRepository
{
    public ZampliaSyncLogRepository(DapperDBSetting dbSetting) : base(dbSetting) { }
}

public class ZampliaSurveyRepository : DapperGenericRepository<ZampliaSurvey>, IZampliaSurveyRepository
{
    public ZampliaSurveyRepository(DapperDBSetting dbSetting) : base(dbSetting) { }
}

public class ZampliaSurveyQualificationRepository : DapperGenericRepository<ZampliaSurveyQualification>, IZampliaSurveyQualificationRepository
{
    public ZampliaSurveyQualificationRepository(DapperDBSetting dbSetting) : base(dbSetting) { }
}

public class ZampliaSurveyQuotaRepository : DapperGenericRepository<ZampliaSurveyQuota>, IZampliaSurveyQuotaRepository
{
    public ZampliaSurveyQuotaRepository(DapperDBSetting dbSetting) : base(dbSetting) { }
}

public class ZampliaProjectMapRepository : DapperGenericRepository<ZampliaProjectMap>, IZampliaProjectMapRepository
{
    public ZampliaProjectMapRepository(DapperDBSetting dbSetting) : base(dbSetting) { }
}

public class ZampliaEntryLinkRepository : DapperGenericRepository<ZampliaEntryLink>, IZampliaEntryLinkRepository
{
    public ZampliaEntryLinkRepository(DapperDBSetting dbSetting) : base(dbSetting) { }
}

public class ZampliaRespondentAttemptRepository : DapperGenericRepository<ZampliaRespondentAttempt>, IZampliaRespondentAttemptRepository
{
    public ZampliaRespondentAttemptRepository(DapperDBSetting dbSetting) : base(dbSetting) { }
}

public class ZampliaReconciliationRunRepository : DapperGenericRepository<ZampliaReconciliationRun>, IZampliaReconciliationRunRepository
{
    public ZampliaReconciliationRunRepository(DapperDBSetting dbSetting) : base(dbSetting) { }
}

public class ZampliaReconciliationItemRepository : DapperGenericRepository<ZampliaReconciliationItem>, IZampliaReconciliationItemRepository
{
    public ZampliaReconciliationItemRepository(DapperDBSetting dbSetting) : base(dbSetting) { }
}
