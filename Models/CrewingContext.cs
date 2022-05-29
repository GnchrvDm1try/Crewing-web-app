using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Crewing.Models
{
    public partial class CrewingContext : DbContext
    {
        public CrewingContext()
        {
        }

        public CrewingContext(DbContextOptions<CrewingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Agreement> Agreements { get; set; } = null!;
        public virtual DbSet<Client> Clients { get; set; } = null!;
        public virtual DbSet<Contract> Contracts { get; set; } = null!;
        public virtual DbSet<Document> Documents { get; set; } = null!;
        public virtual DbSet<Efficiency> Efficiencies { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<EmployeePost> EmployeePosts { get; set; } = null!;
        public virtual DbSet<Employer> Employers { get; set; } = null!;
        public virtual DbSet<Language> Languages { get; set; } = null!;
        public virtual DbSet<LanguageClient> LanguageClients { get; set; } = null!;
        public virtual DbSet<Post> Posts { get; set; } = null!;
        public virtual DbSet<Requirement> Requirements { get; set; } = null!;
        public virtual DbSet<Review> Reviews { get; set; } = null!;
        public virtual DbSet<Sailorpost> Sailorposts { get; set; } = null!;
        public virtual DbSet<Vacancy> Vacancies { get; set; } = null!;
        public virtual DbSet<Vessel> Vessels { get; set; } = null!;
        public virtual DbSet<Vesseltype> Vesseltypes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("Server=localhost; Port=5432; Database=Crewing; Username=postgres; Password=admin");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agreement>(entity =>
            {
                entity.HasKey(e => e.Agreementnumber)
                    .HasName("agreement_pkey");

                entity.ToTable("agreement");

                entity.Property(e => e.Agreementnumber).HasColumnName("agreementnumber");

                entity.Property(e => e.Conclusiondate).HasColumnName("conclusiondate");

                entity.Property(e => e.Employeeid).HasColumnName("employeeid");

                entity.Property(e => e.Vesselnumber)
                    .HasMaxLength(15)
                    .HasColumnName("vesselnumber");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Agreements)
                    .HasForeignKey(d => d.Employeeid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("agreement_employeeid_fkey");

                entity.HasOne(d => d.VesselnumberNavigation)
                    .WithMany(p => p.Agreements)
                    .HasForeignKey(d => d.Vesselnumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("agreement_vesselnumber_fkey");
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.ToTable("client");

                entity.HasIndex(e => e.Email, "clients_email_key")
                    .IsUnique();

                entity.HasIndex(e => e.Phonenumber, "clients_phonenumber_key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('clients_id_seq'::regclass)");

                entity.Property(e => e.Bio)
                    .HasMaxLength(1000)
                    .HasColumnName("bio");

                entity.Property(e => e.Birthdate).HasColumnName("birthdate");

                entity.Property(e => e.Chronicdiseases)
                    .HasMaxLength(200)
                    .HasColumnName("chronicdiseases");

                entity.Property(e => e.Dependencies)
                    .HasMaxLength(200)
                    .HasColumnName("dependencies");

                entity.Property(e => e.Education)
                    .HasMaxLength(200)
                    .HasColumnName("education");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .HasColumnName("email");

                entity.Property(e => e.Experience)
                    .HasMaxLength(500)
                    .HasColumnName("experience");

                entity.Property(e => e.Firstname)
                    .HasMaxLength(20)
                    .HasColumnName("firstname");

                entity.Property(e => e.Ismale).HasColumnName("ismale");

                entity.Property(e => e.Lastname)
                    .HasMaxLength(30)
                    .HasColumnName("lastname");

                entity.Property(e => e.Phonenumber)
                    .HasMaxLength(13)
                    .HasColumnName("phonenumber");

                entity.Property(e => e.Registrationdate).HasColumnName("registrationdate");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");
            });

            modelBuilder.Entity<Contract>(entity =>
            {
                entity.HasKey(e => e.Contractnumber)
                    .HasName("contract_pkey");

                entity.ToTable("contract");

                entity.Property(e => e.Contractnumber).HasColumnName("contractnumber");

                entity.Property(e => e.Clientid).HasColumnName("clientid");

                entity.Property(e => e.Conclusiondate).HasColumnName("conclusiondate");

                entity.Property(e => e.Employeeid).HasColumnName("employeeid");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");

                entity.Property(e => e.Vacancyid).HasColumnName("vacancyid");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.Contracts)
                    .HasForeignKey(d => d.Clientid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("contract_clientid_fkey");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Contracts)
                    .HasForeignKey(d => d.Employeeid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("contract_employeeid_fkey");

                entity.HasOne(d => d.Vacancy)
                    .WithMany(p => p.Contracts)
                    .HasForeignKey(d => d.Vacancyid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("contract_vacancyid_fkey");
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.ToTable("document");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Clientid).HasColumnName("clientid");

                entity.Property(e => e.Documentname)
                    .HasMaxLength(30)
                    .HasColumnName("documentname");

                entity.Property(e => e.Documentnumber)
                    .HasMaxLength(20)
                    .HasColumnName("documentnumber");

                entity.Property(e => e.Expiredate).HasColumnName("expiredate");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.Documents)
                    .HasForeignKey(d => d.Clientid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("document_clientid_fkey");
            });

            modelBuilder.Entity<Efficiency>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("efficiency");

                entity.Property(e => e.AgreementCount).HasColumnName("agreement_count");

                entity.Property(e => e.ClientCount).HasColumnName("client_count");

                entity.Property(e => e.ContractCount).HasColumnName("contract_count");

                entity.Property(e => e.Month).HasColumnName("month");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("employee");

                entity.HasIndex(e => e.Phonenumber, "employee_phonenumber_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Birthdate).HasColumnName("birthdate");

                entity.Property(e => e.Firstname)
                    .HasMaxLength(20)
                    .HasColumnName("firstname");

                entity.Property(e => e.Lastname)
                    .HasMaxLength(30)
                    .HasColumnName("lastname");

                entity.Property(e => e.Phonenumber)
                    .HasMaxLength(13)
                    .HasColumnName("phonenumber");
            });

            modelBuilder.Entity<EmployeePost>(entity =>
            {
                entity.HasKey(e => new { e.Employeeid, e.Postid })
                    .HasName("employee_post_pkey");

                entity.ToTable("employee_post");

                entity.Property(e => e.Employeeid).HasColumnName("employeeid");

                entity.Property(e => e.Postid).HasColumnName("postid");

                entity.Property(e => e.Hiringdate).HasColumnName("hiringdate");

                entity.Property(e => e.Salary)
                    .HasColumnType("money")
                    .HasColumnName("salary");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeePosts)
                    .HasForeignKey(d => d.Employeeid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("employee_post_employeeid_fkey");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.EmployeePosts)
                    .HasForeignKey(d => d.Postid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("employee_post_postid_fkey");
            });

            modelBuilder.Entity<Employer>(entity =>
            {
                entity.HasKey(e => e.Companyname)
                    .HasName("employer_pkey");

                entity.ToTable("employer");

                entity.HasIndex(e => e.Email, "employer_email_key")
                    .IsUnique();

                entity.HasIndex(e => e.Phonenumber, "employer_phonenumber_key")
                    .IsUnique();

                entity.Property(e => e.Companyname)
                    .HasMaxLength(30)
                    .HasColumnName("companyname");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .HasColumnName("email");

                entity.Property(e => e.Phonenumber)
                    .HasMaxLength(13)
                    .HasColumnName("phonenumber");

                entity.Property(e => e.Rating).HasColumnName("rating");
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.ToTable("language");

                entity.HasIndex(e => e.Name, "languages_name_key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('languages_id_seq'::regclass)");

                entity.Property(e => e.Name)
                    .HasMaxLength(30)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<LanguageClient>(entity =>
            {
                entity.HasKey(e => new { e.Clientid, e.Languageid })
                    .HasName("languages_clients_pkey");

                entity.ToTable("language_client");

                entity.Property(e => e.Clientid).HasColumnName("clientid");

                entity.Property(e => e.Languageid).HasColumnName("languageid");

                entity.Property(e => e.Level)
                    .HasMaxLength(50)
                    .HasColumnName("level");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.LanguageClients)
                    .HasForeignKey(d => d.Clientid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("languages_clients_clientid_fkey");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.LanguageClients)
                    .HasForeignKey(d => d.Languageid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("languages_clients_languageid_fkey");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("post");

                entity.HasIndex(e => e.Name, "post_name_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(25)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Requirement>(entity =>
            {
                entity.ToTable("requirement");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description)
                    .HasMaxLength(150)
                    .HasColumnName("description");

                entity.Property(e => e.Level)
                    .HasMaxLength(30)
                    .HasColumnName("level");

                entity.Property(e => e.Name)
                    .HasMaxLength(30)
                    .HasColumnName("name");

                entity.Property(e => e.Vacancyid).HasColumnName("vacancyid");

                entity.HasOne(d => d.Vacancy)
                    .WithMany(p => p.Requirements)
                    .HasForeignKey(d => d.Vacancyid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("requirement_vacancyid_fkey");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("review");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Clientid).HasColumnName("clientid");

                entity.Property(e => e.Comment).HasColumnName("comment");

                entity.Property(e => e.Companyname)
                    .HasMaxLength(30)
                    .HasColumnName("companyname");

                entity.Property(e => e.Datetime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("datetime");

                entity.Property(e => e.Estimation).HasColumnName("estimation");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.Clientid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("review_clientid_fkey");

                entity.HasOne(d => d.CompanynameNavigation)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.Companyname)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("review_companyname_fkey");
            });

            modelBuilder.Entity<Sailorpost>(entity =>
            {
                entity.ToTable("sailorpost");

                entity.HasIndex(e => e.Name, "sailorpost_name_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(20)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Vacancy>(entity =>
            {
                entity.ToTable("vacancy");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Agreementnumber).HasColumnName("agreementnumber");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Sailorpostid).HasColumnName("sailorpostid");

                entity.Property(e => e.Salary)
                    .HasColumnType("money")
                    .HasColumnName("salary");

                entity.Property(e => e.Term)
                    .HasMaxLength(50)
                    .HasColumnName("term");

                entity.Property(e => e.Workersamount).HasColumnName("workersamount");

                entity.HasOne(d => d.AgreementnumberNavigation)
                    .WithMany(p => p.Vacancies)
                    .HasForeignKey(d => d.Agreementnumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("vacancy_agreementnumber_fkey");

                entity.HasOne(d => d.Sailorpost)
                    .WithMany(p => p.Vacancies)
                    .HasForeignKey(d => d.Sailorpostid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("vacancy_sailorpostid_fkey");
            });

            modelBuilder.Entity<Vessel>(entity =>
            {
                entity.HasKey(e => e.Internationalnumber)
                    .HasName("vessels_pkey");

                entity.ToTable("vessel");

                entity.Property(e => e.Internationalnumber)
                    .HasMaxLength(15)
                    .HasColumnName("internationalnumber");

                entity.Property(e => e.Companyname)
                    .HasMaxLength(30)
                    .HasColumnName("companyname");

                entity.Property(e => e.Location)
                    .HasMaxLength(25)
                    .HasColumnName("location");

                entity.Property(e => e.Status)
                    .HasMaxLength(30)
                    .HasColumnName("status");

                entity.Property(e => e.Vesselname)
                    .HasMaxLength(30)
                    .HasColumnName("vesselname");

                entity.Property(e => e.Vesseltypeid).HasColumnName("vesseltypeid");

                entity.Property(e => e.Workersamount).HasColumnName("workersamount");

                entity.HasOne(d => d.CompanynameNavigation)
                    .WithMany(p => p.Vessels)
                    .HasForeignKey(d => d.Companyname)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("vessels_companyname_fkey");

                entity.HasOne(d => d.Vesseltype)
                    .WithMany(p => p.Vessels)
                    .HasForeignKey(d => d.Vesseltypeid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("vessels_vesseltypeid_fkey");
            });

            modelBuilder.Entity<Vesseltype>(entity =>
            {
                entity.ToTable("vesseltype");

                entity.HasIndex(e => e.Name, "vesseltype_name_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(30)
                    .HasColumnName("name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
