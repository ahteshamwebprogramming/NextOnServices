using Microsoft.EntityFrameworkCore;
using NextOnServices.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Services.DBContext;

public partial class NextOnServicesDbContext : DbContext
{
    public NextOnServicesDbContext()
    {
    }

    public NextOnServicesDbContext(DbContextOptions<NextOnServicesDbContext> options)
        : base(options)
    {
    }
    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<CompleteRedirect> CompleteRedirects { get; set; }
    public virtual DbSet<CountryMaster> CountryMasters { get; set; }

    public virtual DbSet<Ipmaster> Ipmasters { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectMapping> ProjectMappings { get; set; }

    public virtual DbSet<ProjectOptionMapping> ProjectOptionMappings { get; set; }

    public virtual DbSet<ProjectParameter> ProjectParameters { get; set; }

    public virtual DbSet<ProjectQuestionTerminate> ProjectQuestionTerminates { get; set; }

    public virtual DbSet<ProjectQuestionsMapping> ProjectQuestionsMappings { get; set; }

    public virtual DbSet<ProjectSurveyResponse> ProjectSurveyResponses { get; set; }

    public virtual DbSet<ProjectsUrl> ProjectsUrls { get; set; }

    public virtual DbSet<QuestionOption> QuestionOptions { get; set; }

    public virtual DbSet<QuestionsMaster> QuestionsMasters { get; set; }

    public virtual DbSet<RecontactProject> RecontactProjects { get; set; }

    public virtual DbSet<RecontactProjectsId> RecontactProjectsIds { get; set; }

    public virtual DbSet<StatusMaster> StatusMasters { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<SupplierPanelSize> SupplierPanelSizes { get; set; }

    public virtual DbSet<SupplierProjects> SupplierProjects { get; set; }



    public virtual DbSet<TblIpmapping> TblIpmappings { get; set; }

    public virtual DbSet<TblRecontactResp> TblRecontactResps { get; set; }

    public virtual DbSet<TblRedirectsForRecontact> TblRedirectsForRecontacts { get; set; }

    public virtual DbSet<TblRevenue> TblRevenues { get; set; }

    public virtual DbSet<TblToken> TblTokens { get; set; }

    public virtual DbSet<TempOutput> TempOutputs { get; set; }

    public virtual DbSet<Testing> Testings { get; set; }

    public virtual DbSet<Urlvariable> Urlvariables { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=182.18.138.217;Initial Catalog=NextOnServicesCore_BK;User ID=sa;Password=CzWR6nbSsE44c$;Encrypt=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.Property(e => e.ClientId).HasColumnName("ClientId");
            entity.Property(e => e.Cemail)
                .HasMaxLength(500)
                .HasColumnName("CEmail");
            entity.Property(e => e.Cnumber)
                .HasMaxLength(50)
                .HasColumnName("CNumber");
            entity.Property(e => e.Company).HasMaxLength(500);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.Cperson)
                .HasMaxLength(500)
                .HasColumnName("CPerson");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Cstatus).HasColumnName("CStatus");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
        });

        modelBuilder.Entity<CompleteRedirect>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Complete__3214EC071FABD2C3");

            entity.Property(e => e.Code).HasMaxLength(100);
        });

        modelBuilder.Entity<Ipmaster>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__IPMaster__3213E83FD53D47AE");

            entity.ToTable("IPMaster");

            entity.HasIndex(e => e.Ip, "IX_IPMaster_IP");

            entity.HasIndex(e => e.Id, "IX_IPMaster_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.CountryCode)
                .HasMaxLength(10)
                .HasColumnName("Country_Code");
            entity.Property(e => e.CreatedDate).HasMaxLength(30);
            entity.Property(e => e.Ip)
                .HasMaxLength(30)
                .HasColumnName("IP");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasIndex(e => e.Status, "IX_Projects_Status");

            entity.Property(e => e.ProjectId).HasColumnName("ProjectId");
            entity.Property(e => e.BlockDevice)
                .HasMaxLength(10)
                .HasDefaultValueSql("('00000')");


            entity.Property(e => e.Cpi).HasColumnName("CPI");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Edate)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EDate");
            entity.Property(e => e.Ipcount).HasColumnName("IPCount");
            entity.Property(e => e.Irate)
                .HasMaxLength(50)
                .HasColumnName("IRate");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.Loi)
                .HasMaxLength(50)
                .HasColumnName("LOI");
            entity.Property(e => e.Ltype).HasColumnName("LType");
            entity.Property(e => e.Pid)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PID");
            entity.Property(e => e.Pmanager).HasColumnName("PManager");
            entity.Property(e => e.Pname)
                .HasMaxLength(500)
                .HasColumnName("PName");
            entity.Property(e => e.ProjectFrom).HasMaxLength(50);
            entity.Property(e => e.ProjectIdFromApi)
                .HasMaxLength(150)
                .HasColumnName("ProjectIdFromAPI");
            entity.Property(e => e.Quota).HasMaxLength(50);
            entity.Property(e => e.SampleSize).HasMaxLength(50);
            entity.Property(e => e.Sdate)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SDate");
            entity.Property(e => e.Status).HasDefaultValueSql("((1))");
        });

        modelBuilder.Entity<ProjectMapping>(entity =>
        {
            entity.ToTable("ProjectMapping");

            entity.HasIndex(e => e.Sid, "IX_ProjectMapping_SID09");

            entity.HasIndex(e => e.ProjectId, "Mindex");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AddHashing).HasDefaultValueSql("((0))");
            entity.Property(e => e.Block).HasDefaultValueSql("((0))");
            entity.Property(e => e.ClientBrowser).HasMaxLength(50);
            entity.Property(e => e.ClientIp)
                .HasMaxLength(50)
                .HasColumnName("ClientIP");
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CountryId).HasColumnName("CountryID");
            entity.Property(e => e.Cpi).HasColumnName("CPI");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Default).HasColumnName("DEFAULT");
            entity.Property(e => e.Failure).HasColumnName("FAILURE");
            entity.Property(e => e.HashingType).HasMaxLength(50);
            entity.Property(e => e.IsSent).HasDefaultValueSql("((0))");
            entity.Property(e => e.IsUsed).HasDefaultValueSql("((0))");
            entity.Property(e => e.Mlink).HasColumnName("MLink");
            entity.Property(e => e.Nid)
                .HasMaxLength(50)
                .HasColumnName("NID");
            entity.Property(e => e.Notes)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Olink).HasColumnName("OLink");
            entity.Property(e => e.OverQuota1).HasColumnName("OVERQUOTA1");
            entity.Property(e => e.ParameterName).HasMaxLength(50);
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.QualityTermination).HasColumnName("QUALITYTERMINATION");
            entity.Property(e => e.Rc)
                .HasDefaultValueSql("((0))")
                .HasColumnName("RC");
            entity.Property(e => e.Scode)
                .HasMaxLength(50)
                .HasColumnName("SCode");
            entity.Property(e => e.Sid)
                .HasMaxLength(50)
                .HasColumnName("SID");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Success).HasColumnName("SUCCESS");
            entity.Property(e => e.SupplierId).HasColumnName("SUpplierID");
        });

        modelBuilder.Entity<ProjectOptionMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectO__3214EC07B7F65E92");

            entity.ToTable("ProjectOptionMapping");

            entity.Property(e => e.Cid).HasColumnName("CID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.OptionsId).HasMaxLength(255);
            entity.Property(e => e.Pid).HasColumnName("PID");
            entity.Property(e => e.Qid).HasColumnName("QID");
        });

        modelBuilder.Entity<ProjectParameter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectP__3214EC0701008E75");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ParameterKey).HasMaxLength(100);
            entity.Property(e => e.ParameterValue).HasMaxLength(100);
        });

        modelBuilder.Entity<ProjectQuestionTerminate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectQ__3214EC07A6D936A9");

            entity.Property(e => e.Cid).HasColumnName("CID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Logic).HasMaxLength(100);
            entity.Property(e => e.Pid).HasColumnName("PID");
            entity.Property(e => e.Qid).HasColumnName("QID");
        });

        modelBuilder.Entity<ProjectQuestionsMapping>(entity =>
        {
            entity.ToTable("ProjectQuestionsMapping");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Cid).HasColumnName("CID");
            entity.Property(e => e.Crdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CRDate");
            entity.Property(e => e.Pid).HasColumnName("PID");
            entity.Property(e => e.Qids)
                .HasMaxLength(50)
                .HasColumnName("QIDs");
            entity.Property(e => e.QuestionQid).HasColumnName("QuestionQID");
        });

        modelBuilder.Entity<ProjectSurveyResponse>(entity =>
        {
            entity.ToTable("ProjectSurveyResponse");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Crdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CRDate");
            entity.Property(e => e.Pid).HasColumnName("PID");
            entity.Property(e => e.Qid).HasColumnName("QID");
            entity.Property(e => e.Qoptions).HasMaxLength(500);
            entity.Property(e => e.Uid)
                .HasMaxLength(50)
                .HasColumnName("UID");
        });

        modelBuilder.Entity<ProjectsUrl>(entity =>
        {
            entity.ToTable("ProjectsUrl");

            entity.HasIndex(e => e.Pid, "IX_ProjectsUrl_PID");

            entity.HasIndex(e => new { e.Pid, e.Cid }, "IX_ProjectsUrl_PID_CID");

            entity.HasIndex(e => new { e.Pid, e.Cid }, "IX_ProjectsUrl_PID_CID_S");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Cid).HasColumnName("CID");
            entity.Property(e => e.Cpi).HasColumnName("CPI");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Notes)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.OriginalUrl).HasColumnName("OriginalURL");
            entity.Property(e => e.ParameterName).HasMaxLength(50);
            entity.Property(e => e.ParameterValue).HasMaxLength(50);
            entity.Property(e => e.Pid).HasColumnName("PID");
            entity.Property(e => e.Quota).HasMaxLength(50);
            entity.Property(e => e.Token).HasDefaultValueSql("((0))");
        });

        modelBuilder.Entity<QuestionOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ScrQuestionOptions");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CrDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OptionCode).HasMaxLength(50);
            entity.Property(e => e.OptionLabel).HasMaxLength(500);
            entity.Property(e => e.Qid).HasColumnName("QID");
        });

        modelBuilder.Entity<QuestionsMaster>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ScrQuestions");

            entity.ToTable("QuestionsMaster");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.QuestionId)
                .HasMaxLength(50)
                .HasColumnName("QuestionID");
            entity.Property(e => e.QuestionLabel).HasMaxLength(500);
        });

        modelBuilder.Entity<RecontactProject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Recontac__3214EC0731CC48CB");

            entity.Property(e => e.Cpi).HasColumnName("CPI");
            entity.Property(e => e.Ir).HasColumnName("IR");
            entity.Property(e => e.Loi).HasColumnName("LOI");
            entity.Property(e => e.Murl).HasColumnName("MURL");
            entity.Property(e => e.Notes).IsUnicode(false);
            entity.Property(e => e.Rccnt).HasColumnName("RCcnt");
            entity.Property(e => e.Rcq).HasColumnName("RCQ");
            entity.Property(e => e.RecontactDescription).HasMaxLength(200);
            entity.Property(e => e.RecontactProjectId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Recontactname).HasColumnName("recontactname");
            entity.Property(e => e.Sid).HasColumnName("SID");
            entity.Property(e => e.Url).HasColumnName("URL");
        });

        modelBuilder.Entity<RecontactProjectsId>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Recontac__3214EC07B867B2F0");

            entity.Property(e => e.Isused).HasColumnName("isused");
            entity.Property(e => e.Pmid).HasColumnName("PMID");
            entity.Property(e => e.RecontactProjectsId1).HasColumnName("RecontactProjectsId");
            entity.Property(e => e.Uid).HasColumnName("UID");
            entity.Property(e => e.Uidnew).HasColumnName("UIDnew");
        });

        modelBuilder.Entity<StatusMaster>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__statusMa__3214EC07768CFFDD");

            entity.ToTable("statusMaster");

            entity.Property(e => e.Pstatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PStatus");
            entity.Property(e => e.Pvalue).HasColumnName("PValue");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AuthorizationKey).HasMaxLength(500);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Default).HasColumnName("DEFAULT");
            entity.Property(e => e.Email).HasMaxLength(500);
            entity.Property(e => e.Failure).HasColumnName("FAILURE");
            entity.Property(e => e.HashingUrl).HasColumnName("HashingURL");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.Name).HasMaxLength(500);
            entity.Property(e => e.Number).HasMaxLength(50);
            entity.Property(e => e.OverQuota1).HasColumnName("OVERQUOTA1");
            entity.Property(e => e.Psize)
                .HasMaxLength(50)
                .HasColumnName("PSize");
            entity.Property(e => e.QualityTermination).HasColumnName("QUALITYTERMINATION");
            entity.Property(e => e.Sstatus).HasColumnName("SStatus");
            entity.Property(e => e.Success).HasColumnName("SUCCESS");
        });

        modelBuilder.Entity<SupplierPanelSize>(entity =>
        {
            entity.ToTable("SupplierPanelSize");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CountryId).HasColumnName("CountryID");
            entity.Property(e => e.Psize).HasColumnName("PSize");
            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
        });

        modelBuilder.Entity<SupplierProjects>(entity =>
        {
            entity.HasIndex(e => e.ID, "IX_SupplierProjects_ID");

            entity.HasIndex(e => e.PMID, "IX_SupplierProjects_PMID");

            entity.HasIndex(e => new { e.SID, e.Status, e.StartDate, e.EndDate }, "IX_SupplierProjects_SID_Stat098us_StartDate_EndDate");

            entity.HasIndex(e => new { e.SID, e.Status }, "IX_SupplierProjects_SID_Status098");

            entity.HasIndex(e => new { e.SID, e.Status }, "IX_SupplierProjects_SID_Status98");

            entity.HasIndex(e => e.UID, "IX_SupplierProjects_UID");

            entity.HasIndex(e => e.SID, "M1index");

            entity.HasIndex(e => e.Status, "Mindex2");

            entity.Property(e => e.ID).HasColumnName("ID");
            entity.Property(e => e.ActStatus).HasMaxLength(50);
            entity.Property(e => e.ClientBrowser).HasMaxLength(50);
            entity.Property(e => e.ClientIP)
                .HasMaxLength(50)
                .HasColumnName("ClientIP");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Device).HasMaxLength(50);
            entity.Property(e => e.ENC)
                .HasMaxLength(255)
                .HasColumnName("ENC");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.IsSent).HasDefaultValueSql("((0))");
            entity.Property(e => e.PMID)
                .HasMaxLength(50)
                .HasColumnName("PMID");
            entity.Property(e => e.SID)
                .HasMaxLength(50)
                .HasColumnName("SID");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UID)
                .HasMaxLength(50)
                .HasColumnName("UID");
        });

        modelBuilder.Entity<CountryMaster>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("PK_TblCountries");

            entity.ToTable("CountryMaster");

            entity.Property(e => e.Alpha2)
                .HasMaxLength(255)
                .HasColumnName("ALPHA2");
            entity.Property(e => e.Alpha3)
                .HasMaxLength(255)
                .HasColumnName("ALPHA3");
            entity.Property(e => e.Country).HasMaxLength(255);
            entity.Property(e => e.Ctype).HasColumnName("CType");
        });

        modelBuilder.Entity<TblIpmapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblIPMap__3214EC07CA9DC88D");

            entity.ToTable("tblIPMapping");

            entity.Property(e => e.Countryid).HasColumnName("countryid");
            entity.Property(e => e.Isactive).HasColumnName("isactive");
            entity.Property(e => e.ProUrlid).HasColumnName("ProURLId");
            entity.Property(e => e.Stat).HasColumnName("stat");
        });

        modelBuilder.Entity<TblRecontactResp>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblRecon__3213E83F0D7B1E85");

            entity.ToTable("tblRecontactResp");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActStatus).HasMaxLength(50);
            entity.Property(e => e.ClientBrowser).HasMaxLength(50);
            entity.Property(e => e.ClientIp)
                .HasMaxLength(30)
                .HasColumnName("ClientIP");
            entity.Property(e => e.Device).HasMaxLength(20);
            entity.Property(e => e.EndDate)
                .HasMaxLength(16)
                .IsUnicode(false);
            entity.Property(e => e.IsSent).HasDefaultValueSql("((0))");
            entity.Property(e => e.MaskingUrl).HasColumnName("MaskingURL");
            entity.Property(e => e.NewPmid).HasColumnName("NewPMID");
            entity.Property(e => e.NewUid).HasColumnName("newUID");
            entity.Property(e => e.Pmid).HasColumnName("PMID");
            entity.Property(e => e.Rpi).HasColumnName("RPI");
            entity.Property(e => e.Sid).HasColumnName("SID");
            entity.Property(e => e.StartDate)
                .HasMaxLength(16)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Uid).HasColumnName("UID");
        });

        modelBuilder.Entity<TblRedirectsForRecontact>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblRedir__3213E83F7D1CA01F");

            entity.ToTable("tblRedirectsForRecontact");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FError).HasColumnName("F_Error");
            entity.Property(e => e.Notes).HasMaxLength(200);
            entity.Property(e => e.Rpid).HasColumnName("RPID");
            entity.Property(e => e.STerm).HasColumnName("S_Term");
        });

        modelBuilder.Entity<TblRevenue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblReven__3213E83F42CF8FB9");

            entity.ToTable("tblRevenue");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Clientid).HasColumnName("clientid");
            entity.Property(e => e.Countryid).HasColumnName("countryid");
            entity.Property(e => e.Cpi).HasColumnName("CPI");
            entity.Property(e => e.Projectid).HasColumnName("projectid");
            entity.Property(e => e.Sid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sid");
            entity.Property(e => e.Supplierid).HasColumnName("supplierid");
        });

        modelBuilder.Entity<TblToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblToken__3214EC270D898578");

            entity.ToTable("tblTokens");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IsUsed).HasDefaultValueSql("((0))");
            entity.Property(e => e.ProjectUrlid).HasColumnName("ProjectURLId");
            entity.Property(e => e.Sid)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SID");
            entity.Property(e => e.Token)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Tstatus)
                .HasDefaultValueSql("((0))")
                .HasColumnName("TStatus");
            entity.Property(e => e.Uid).HasColumnName("UID");
        });

        modelBuilder.Entity<TempOutput>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tempOutput");

            entity.Property(e => e.ActIr).HasColumnName("ActIR");
            entity.Property(e => e.ActLoi).HasColumnName("ActLOI");
            entity.Property(e => e.Client)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CLIENT");
            entity.Property(e => e.ClientId).HasColumnName("ClientID");
            entity.Property(e => e.Co).HasColumnName("CO");
            entity.Property(e => e.Country)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("COUNTRY");
            entity.Property(e => e.Cpi).HasColumnName("CPI");
            entity.Property(e => e.Date)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Fe).HasColumnName("FE");
            entity.Property(e => e.Flag)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Ic).HasColumnName("IC");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Irate).HasColumnName("IRate");
            entity.Property(e => e.Loi).HasColumnName("LOI");
            entity.Property(e => e.Oq).HasColumnName("OQ");
            entity.Property(e => e.Pm)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("PM");
            entity.Property(e => e.Pname)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PNAME");
            entity.Property(e => e.Pno)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("PNO");
            entity.Property(e => e.St).HasColumnName("ST");
            entity.Property(e => e.Startdate)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("startdate");
            entity.Property(e => e.Status).HasColumnName("STATUS");
            entity.Property(e => e.Tid)
                .ValueGeneratedOnAdd()
                .HasColumnName("tid");
            entity.Property(e => e.Total).HasColumnName("TOTAL");
            entity.Property(e => e.Tr).HasColumnName("TR");
        });

        modelBuilder.Entity<Testing>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__testing__3213E83FB068670A");

            entity.ToTable("testing");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Checkk)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CHECKk");
        });

        modelBuilder.Entity<Urlvariable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__URLVaria__3213E83FE7D47FD0");

            entity.ToTable("URLVariables");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Rpid).HasColumnName("RPID");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.VariableName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.VariableValue).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).HasColumnName("UserId");
            entity.Property(e => e.ContactNumber).HasMaxLength(50);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmailId)
                .HasMaxLength(500)
                .HasColumnName("EmailID");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasDefaultValueSql("((1))")
                .HasColumnName("status");
            entity.Property(e => e.UserCode).HasMaxLength(500);
            entity.Property(e => e.UserName).HasMaxLength(500);
            entity.Property(e => e.UserType)
                .HasMaxLength(5)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

}
