using NextOnServices.Core.Entities;

namespace NextOnServices.Core.Repository;

public interface IZampliaSettingRepository : IDapperRepository<ZampliaSetting> { }
public interface IZampliaSyncLogRepository : IDapperRepository<ZampliaSyncLog> { }
public interface IZampliaSurveyRepository : IDapperRepository<ZampliaSurvey> { }
public interface IZampliaSurveyQualificationRepository : IDapperRepository<ZampliaSurveyQualification> { }
public interface IZampliaSurveyQuotaRepository : IDapperRepository<ZampliaSurveyQuota> { }
public interface IZampliaProjectMapRepository : IDapperRepository<ZampliaProjectMap> { }
public interface IZampliaEntryLinkRepository : IDapperRepository<ZampliaEntryLink> { }
public interface IZampliaRespondentAttemptRepository : IDapperRepository<ZampliaRespondentAttempt> { }
public interface IZampliaReconciliationRunRepository : IDapperRepository<ZampliaReconciliationRun> { }
public interface IZampliaReconciliationItemRepository : IDapperRepository<ZampliaReconciliationItem> { }
