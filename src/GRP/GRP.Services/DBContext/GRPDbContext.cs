using Microsoft.EntityFrameworkCore;
using GRP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Services.DBContext;

public partial class GRPDbContext : DbContext
{
    public GRPDbContext()
    {
    }

    public GRPDbContext(DbContextOptions<GRPDbContext> options)
        : base(options)
    {
    }
    public virtual DbSet<LoginDetail> LoginDetails { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<TBL_UserProfile> TBL_UserProfiles { get; set; }
    public virtual DbSet<ProfileInfoCategory> ProfileInfoCategories { get; set; }
    public virtual DbSet<ProfileInfoSurvey> ProfileInfoSurveys { get; set; }

    public virtual DbSet<QuestionTypeMaster> QuestionTypeMasters { get; set; }

    public virtual DbSet<QuestionTypeSelectFramework> QuestionTypeSelectFrameworks { get; set; }
    public virtual DbSet<Survey> Surveys { get; set; }

    public virtual DbSet<PointsHistory> PointsHistories { get; set; }
    public virtual DbSet<ProfileSurveyResponse> ProfileSurveyResponses { get; set; }
    public virtual DbSet<PointsTransaction> PointsTransactions { get; set; }
    public virtual DbSet<SurveyRedirectDetails> SurveyRedirectDetailss { get; set; }
    public virtual DbSet<SurveyCriteria> SurveyCriterias { get; set; }
    public virtual DbSet<CountryMaster> CountryMasters { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=182.18.138.217;Initial Catalog=GRPCore;User ID=sa;Password=CzWR6nbSsE44c$;Encrypt=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<LoginDetail>(entity =>
        {
            entity.HasKey(e => e.LoginId).HasName("PK__LoginDet__4DDA2818004EBE17");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Username).HasMaxLength(100);
        });
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C7C75E63C");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
            entity.Property(e => e.EmailId).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProfileInfoCategory>(entity =>
        {
            entity.HasKey(e => e.ProfileInfoCategoryId).HasName("PK__ProfileI__EDBC261CDED5C10A");

            entity.ToTable("ProfileInfoCategory");

            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProfileInfoSurvey>(entity =>
        {
            entity.HasKey(e => e.ProfileInfoSurveyId).HasName("PK__ProfileI__21B4EE5F72F58B77");

            entity.ToTable("ProfileInfoSurvey");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.QuestionLabel).HasMaxLength(255);
        });

        modelBuilder.Entity<QuestionTypeMaster>(entity =>
        {
            entity.HasKey(e => e.QuestionTypeMasterId).HasName("PK__Question__BFFA68994C9CE5F9");

            entity.ToTable("QuestionTypeMaster");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.QuestionType).HasMaxLength(100);
        });

        modelBuilder.Entity<QuestionTypeSelectFramework>(entity =>
        {
            entity.HasKey(e => e.QuestionTypeSelectFrameworkId).HasName("PK__Question__9D9C0030521D58B2");

            entity.ToTable("QuestionTypeSelectFramework");

            entity.Property(e => e.Label).HasMaxLength(200);
            entity.Property(e => e.Value).HasMaxLength(200);
        });


       

        modelBuilder.Entity<Survey>(entity =>
        {
            entity.HasKey(e => e.SurveyId).HasName("PK__Survey__A5481F7DC06A4AB5");

            entity.ToTable("Survey");

            entity.Property(e => e.Age)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.AgeFrom).HasMaxLength(50);
            entity.Property(e => e.AgeTo).HasMaxLength(50);
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ClientDetail).HasMaxLength(100);
            entity.Property(e => e.CompleteURL).HasColumnName("CompleteURL");
            entity.Property(e => e.Country).HasMaxLength(200);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.QuotafullURL).HasColumnName("QuotafullURL");
            entity.Property(e => e.SurveyDescription).HasMaxLength(500);
            entity.Property(e => e.SurveyIdHost).HasMaxLength(50);
            entity.Property(e => e.SurveyName).HasMaxLength(100);
            entity.Property(e => e.SurveyUrl).HasMaxLength(1000);
            entity.Property(e => e.TerminateURL).HasColumnName("TerminateURL");
        });
        modelBuilder.Entity<PointsHistory>(entity =>
        {
            entity.HasKey(e => e.PointsHistoryId).HasName("PK__PointsHi__F21932FF429CBE7F");

            entity.ToTable("PointsHistory");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Source).HasMaxLength(100);
            entity.Property(e => e.TransType).HasMaxLength(50);
        });
        modelBuilder.Entity<PointsTransaction>(entity =>
        {
            entity.HasKey(e => e.PointsTransactionId).HasName("PK__PointsTr__3BCEF116C301C4E3");

            entity.ToTable("PointsTransaction");
        });
        modelBuilder.Entity<ProfileSurveyResponse>(entity =>
        {
            entity.HasKey(e => e.ProfileSurveyResponsesId).HasName("PK__ProfileS__791CA0A508E656BF");

            entity.Property(e => e.AnswerId).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<SurveyRedirectDetails>(entity =>
        {
            entity.HasKey(e => e.SurveyRedirectDetailsId).HasName("PK__SurveyRe__DC5F40DED836C8D1");

            entity.Property(e => e.ActualStatus).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.RespondentId).HasMaxLength(100);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
        });
        modelBuilder.Entity<SurveyCriteria>(entity =>
        {
            entity.HasKey(e => e.SurveyCriteriaId).HasName("PK__SurveyCr__D5B416BAC0F663A7");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Options).HasMaxLength(255);
        });
        modelBuilder.Entity<CountryMaster>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("PK__CountryM__10D1609F84D7D690");

            entity.ToTable("CountryMaster");

            entity.Property(e => e.Country).HasMaxLength(100);
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

}
